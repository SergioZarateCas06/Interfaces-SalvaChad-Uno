using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnoGame;
using UnoGame.Red;

namespace UnoServer
{
    // El servidor: espera a 4 jugadores, arranca la partida y, a partir de ahí, recibe las acciones
    // de cada cliente, las pasa por JuegoUno y le manda a todos el resultado.
    // Es el único que conoce la partida completa (las manos de todos): los clientes solo ven lo suyo.
    public class ServidorUno
    {
        class Cliente
        {
            public ConexionJson Conexion;
            public string Nombre;
            public int Indice;     // asiento 0..3
        }

        // Guarda el id que devuelve la API para esta partida (-1 mientras no se sabe o si falló)
        class RegistroPartida
        {
            public int Id = -1;
        }

        readonly object candado = new object();   // una sola acción de juego a la vez
        readonly Cliente[] asientos = new Cliente[JuegoUno.NumJugadores];
        readonly JuegoUno juego = new JuegoUno();
        readonly ApiCliente api = new ApiCliente();
        // Las llamadas a la API van en una cola que atiende otro hilo: así la partida no espera a la base de datos
        readonly BlockingCollection<Action> colaApi = new BlockingCollection<Action>();
        bool enPartida;
        RegistroPartida registroActual = new RegistroPartida();

        // ---------------- Arranque: el mismo esquema Bind / Listen / Accept del ejemplo ----------------

        public void Iniciar(int puerto)
        {
            Thread hiloApi = new Thread(ProcesarColaApi);
            hiloApi.IsBackground = true;
            hiloApi.Start();

            Socket escucha = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            escucha.Bind(new IPEndPoint(IPAddress.Any, puerto));   // Any: acepta conexiones por cualquier tarjeta de red
            escucha.Listen(10);

            Log("Servidor UNO escuchando en el puerto " + puerto);
            Log("Los jugadores deben conectarse a: " + IpsLocales());

            while (true)
            {
                Socket socket = escucha.Accept();   // espera a que alguien se conecte
                socket.NoDelay = true;
                // Un hilo por jugador, para atender a los 4 al mismo tiempo
                Thread hilo = new Thread(() => AtenderCliente(socket));
                hilo.IsBackground = true;
                hilo.Start();
            }
        }

        static string IpsLocales()
        {
            try
            {
                IEnumerable<string> ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                    .Select(a => a.ToString());
                return string.Join(" / ", ips);
            }
            catch (Exception)
            {
                return "(no se pudo averiguar la IP; usa ipconfig)";
            }
        }

        // ---------------- Un hilo por cliente ----------------

        void AtenderCliente(Socket socket)
        {
            ConexionJson conexion = new ConexionJson(socket);
            Cliente cliente = null;
            try
            {
                // El primer mensaje debe ser "unirse" con el nombre
                Mensaje primero = conexion.Recibir();
                if (primero == null || primero.Tipo != Tipos.Unirse || string.IsNullOrWhiteSpace(primero.Nombre))
                    return;

                lock (candado)
                {
                    string nombre = primero.Nombre.Trim();
                    string motivo = null;
                    if (enPartida)
                        motivo = "Ya hay una partida en curso.";
                    else if (asientos.All(a => a != null))
                        motivo = "La sala está llena.";
                    else if (asientos.Any(a => a != null && string.Equals(a.Nombre, nombre, StringComparison.OrdinalIgnoreCase)))
                        motivo = "Ya hay un jugador con ese nombre.";

                    if (motivo != null)
                    {
                        conexion.Enviar(new Mensaje { Tipo = Tipos.Rechazo, Texto = motivo });
                        return;
                    }

                    int libre = Array.FindIndex(asientos, a => a == null);
                    cliente = new Cliente { Conexion = conexion, Nombre = nombre, Indice = libre };
                    asientos[libre] = cliente;
                    Log(nombre + " se unió (asiento " + libre + ").");

                    conexion.Enviar(new Mensaje { Tipo = Tipos.Bienvenida, Indice = libre });
                    DifundirSala();

                    if (asientos.All(a => a != null))
                        IniciarPartida();
                }

                // A partir de aquí, el hilo se queda leyendo lo que mande este jugador
                while (true)
                {
                    Mensaje mensaje = conexion.Recibir();
                    if (mensaje == null) break;   // se desconectó
                    lock (candado)
                    {
                        Procesar(cliente, mensaje);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Conexión con " + (cliente != null ? cliente.Nombre : "un cliente") + " terminada: " + ex.Message);
            }
            finally
            {
                if (cliente != null) Desconectar(cliente);
                else conexion.Cerrar();
            }
        }

        // Se llama con el candado tomado
        void Procesar(Cliente cliente, Mensaje mensaje)
        {
            if (asientos[cliente.Indice] != cliente) return;   // conexión vieja de una partida anterior
            if (!enPartida)
            {
                Enviar(cliente, new Mensaje { Tipo = Tipos.Error, Texto = "La partida aún no comienza." });
                return;
            }

            string error = null;
            Card carta = null;
            switch (mensaje.Tipo)
            {
                case Tipos.Jugar:
                    if (mensaje.Carta == null || !mensaje.Carta.TryACarta(out carta))
                    {
                        error = "Carta no válida.";
                        break;
                    }
                    error = juego.Jugar(cliente.Indice, carta, ParseColor(mensaje.Color));
                    break;
                case Tipos.Robar:
                    error = juego.Robar(cliente.Indice);
                    break;
                case Tipos.ElegirColor:
                    error = juego.ElegirColor(cliente.Indice, ParseColor(mensaje.Color));
                    break;
                case Tipos.DecirUno:
                    error = juego.DecirUno(cliente.Indice);
                    break;
                default:
                    error = "Mensaje desconocido.";
                    break;
            }

            if (error != null)
            {
                Enviar(cliente, new Mensaje { Tipo = Tipos.Error, Texto = error });
                return;
            }
            DifundirResultado();
        }

        static CardColor ParseColor(string texto)
        {
            CardColor color;
            if (Enum.TryParse(texto, out color) && Enum.IsDefined(typeof(CardColor), color))
                return color;
            return CardColor.Ninguno;
        }

        // ---------------- Inicio y fin de la partida (siempre con el candado tomado) ----------------

        void IniciarPartida()
        {
            enPartida = true;
            string[] nombres = asientos.Select(a => a.Nombre).ToArray();
            juego.Iniciar(nombres);
            Log("Comienza la partida: " + string.Join(", ", nombres));

            // Se crea la partida en la base de datos (sin detener el juego si la API no responde)
            RegistroPartida registro = new RegistroPartida();
            registroActual = registro;
            List<string> lista = nombres.ToList();
            colaApi.Add(() =>
            {
                registro.Id = api.CrearPartida(lista).GetAwaiter().GetResult();
                Log("Partida registrada en la base de datos con id " + registro.Id + ".");
            });

            DifundirResultado();
        }

        // Cuenta lo que pasó, le manda a cada jugador su vista de la mesa y registra los movimientos
        void DifundirResultado()
        {
            List<string> eventos = juego.Eventos.ToList();
            List<MovimientoRegistro> movimientos = juego.Movimientos.ToList();
            juego.Eventos.Clear();
            juego.Movimientos.Clear();

            foreach (string texto in eventos)
                Difundir(new Mensaje { Tipo = Tipos.Evento, Texto = texto });

            // Cada jugador recibe su propia mano completa y, de los demás, solo cuántas cartas tienen
            foreach (Cliente c in asientos.Where(a => a != null).ToList())
                Enviar(c, new Mensaje { Tipo = Tipos.Estado, Estado = juego.EstadoPara(c.Indice) });

            RegistroPartida registro = registroActual;
            foreach (MovimientoRegistro mov in movimientos)
            {
                MovimientoRegistro copia = mov;
                colaApi.Add(() =>
                {
                    if (registro.Id == -1) return;   // la partida no pudo registrarse
                    api.RegistrarMovimiento(registro.Id, copia.NumeroTurno, copia.Jugador, copia.Accion, copia.Carta)
                       .GetAwaiter().GetResult();
                });
            }

            if (juego.Terminada)
                FinalizarPartida(registro);
        }

        void FinalizarPartida(RegistroPartida registro)
        {
            string ganador = juego.Nombres[juego.Ganador];
            Difundir(new Mensaje { Tipo = Tipos.Fin, Texto = ganador, Indice = juego.Ganador });
            colaApi.Add(() =>
            {
                if (registro.Id == -1) return;
                api.RegistrarGanador(registro.Id, ganador).GetAwaiter().GetResult();
            });
            Log("Fin de la partida. Ganador: " + ganador + ".");

            // La sala queda libre para una partida nueva; las conexiones viejas se cierran en un momento
            List<Cliente> anteriores = asientos.Where(a => a != null).ToList();
            ReiniciarSala();
            CerrarConRetraso(anteriores);
        }

        void ReiniciarSala()
        {
            enPartida = false;
            for (int i = 0; i < asientos.Length; i++)
                asientos[i] = null;
        }

        // Se espera un poco para que el último mensaje alcance a llegar antes de cerrar
        static void CerrarConRetraso(List<Cliente> clientes)
        {
            Task.Delay(2000).ContinueWith(t =>
            {
                foreach (Cliente c in clientes)
                    c.Conexion.Cerrar();
            });
        }

        void Desconectar(Cliente cliente)
        {
            lock (candado)
            {
                cliente.Conexion.Cerrar();
                if (asientos[cliente.Indice] != cliente) return;   // ya estaba fuera de la sala

                asientos[cliente.Indice] = null;
                Log(cliente.Nombre + " se desconectó.");

                if (enPartida && !juego.Terminada)
                {
                    // Si alguien se va a media partida, se cancela para todos
                    Difundir(new Mensaje { Tipo = Tipos.Abortada, Texto = cliente.Nombre + " se desconectó. Se cancela la partida." });
                    List<Cliente> restantes = asientos.Where(a => a != null).ToList();
                    ReiniciarSala();
                    CerrarConRetraso(restantes);
                    Log("Partida cancelada. Esperando jugadores...");
                }
                else if (!enPartida)
                {
                    DifundirSala();
                }
            }
        }

        // ---------------- Envío de mensajes ----------------

        void DifundirSala()
        {
            List<string> nombres = asientos.Where(a => a != null).Select(a => a.Nombre).ToList();
            Difundir(new Mensaje
            {
                Tipo = Tipos.Sala,
                Nombres = nombres,
                Texto = nombres.Count + "/" + JuegoUno.NumJugadores
            });
        }

        void Difundir(Mensaje mensaje)
        {
            foreach (Cliente c in asientos.Where(a => a != null).ToList())
                Enviar(c, mensaje);
        }

        // Si falla el envío a uno, no se afecta a los demás: su hilo de lectura notará la caída
        void Enviar(Cliente cliente, Mensaje mensaje)
        {
            try
            {
                cliente.Conexion.Enviar(mensaje);
            }
            catch (Exception ex)
            {
                Log("No se pudo enviar a " + cliente.Nombre + ": " + ex.Message);
            }
        }

        void ProcesarColaApi()
        {
            foreach (Action accion in colaApi.GetConsumingEnumerable())
            {
                try
                {
                    accion();
                }
                catch (Exception ex)
                {
                    Log("API no disponible: " + ex.Message);
                }
            }
        }

        static void Log(string texto)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + texto);
        }
    }
}
