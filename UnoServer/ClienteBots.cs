using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using UnoGame.Red;

namespace UnoServer
{
    // Jugadores automáticos que se conectan al servidor por TCP real, igual que lo hará UnoGame.
    // Sirven para probar el servidor sin necesidad de interfaz gráfica.
    //
    // Uso:  UnoServer.exe bots [ip] [puerto] [abandonarTrasEstados] [cuantos] [prefijo]
    //   ip, puerto             a dónde conectarse (por defecto 127.0.0.1 y 11111)
    //   abandonarTrasEstados   si es mayor que 0, el bot 3 se desconecta a propósito después de recibir
    //                          esa cantidad de estados (prueba de desconexión a media partida)
    //   cuantos                cuántos bots lanzar (por defecto 4; con menos se quedan en la sala de espera)
    //   prefijo                prefijo de los nombres (por defecto "Bot"); cámbialo para no repetir nombres
    //
    // Ejemplos:
    //   UnoServer.exe bots                              partida completa con 4 bots
    //   UnoServer.exe bots 127.0.0.1 11111 0 2          2 bots esperando en la sala
    //   UnoServer.exe bots 127.0.0.1 11111 25           el Bot3 se va a media partida
    public static class ClienteBots
    {
        // Milisegundos que espera un bot antes de cada acción.
        // Sirve para poder seguir la partida (y ver las animaciones del cliente) en vez de que pase al instante.
        const int PausaMs = 1200;

        // Punto de entrada del modo "bots": lee los argumentos, lanza un hilo por bot y espera a que todos terminen.
        // args[0] es la palabra "bots"; los demás son opcionales y se leen por posición.
        public static void Ejecutar(string[] args)
        {
            // Valores por defecto, que se sobrescriben si el usuario los escribió
            string ip = args.Length > 1 ? args[1] : "127.0.0.1";
            int puerto = Protocolo.PuertoPorDefecto;
            int abandonar = 0;
            int cuantos = 4;
            string prefijo = "Bot";
            int valor;
            // TryParse evita que el programa falle si el argumento no es un número: simplemente se ignora
            if (args.Length > 2 && int.TryParse(args[2], out valor)) puerto = valor;
            if (args.Length > 3 && int.TryParse(args[3], out valor)) abandonar = valor;
            if (args.Length > 4 && int.TryParse(args[4], out valor)) cuantos = valor;
            if (args.Length > 5) prefijo = args[5];

            List<Thread> hilos = new List<Thread>();
            for (int i = 0; i < cuantos; i++)
            {
                string nombre = prefijo + (i + 1);           // Bot1, Bot2, ...
                bool imprimirTodo = i == 0;                  // solo el primero cuenta lo que pasa, para no repetirlo 4 veces
                // Solo el tercer bot (i == 2) abandona, y solo si se pidió con el argumento
                int abandonaTras = (abandonar > 0 && i == 2) ? abandonar : 0;

                // Cada bot corre en su propio hilo, igual que cada jugador real es un programa aparte
                Thread hilo = new Thread(() => CorrerBot(nombre, ip, puerto, abandonaTras, imprimirTodo));
                hilo.IsBackground = true;   // si se cierra el programa, los hilos no lo mantienen vivo
                hilos.Add(hilo);
                hilo.Start();
                Thread.Sleep(300);   // así los asientos se asignan en orden
            }

            // Se espera a que todos los bots terminen (por fin de partida, cancelación o error)
            foreach (Thread hilo in hilos)
                hilo.Join();
            Console.WriteLine("Todos los bots terminaron.");
        }

        // Todo el ciclo de vida de UN bot: conectarse, unirse, escuchar mensajes y reaccionar.
        //   abandonaTras: 0 = nunca abandona; N = se desconecta al recibir el estado número N
        //   imprimirTodo: solo un bot imprime eventos y sala, para que la consola no se llene de repetidos
        static void CorrerBot(string nombre, string ip, int puerto, int abandonaTras, bool imprimirTodo)
        {
            // Semilla basada en el nombre: cada bot juega distinto, pero siempre igual entre ejecuciones
            Random azar = new Random(nombre.GetHashCode());
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(ip, puerto);
            }
            catch (Exception ex)
            {
                // Servidor apagado, IP o puerto equivocados, firewall...
                Log(nombre, "No se pudo conectar: " + ex.Message);
                return;
            }

            ConexionJson conexion = new ConexionJson(socket);   // envía y recibe un mensaje JSON por línea
            EstadoJuego ultimo = null;     // último estado de la mesa recibido
            int estadosRecibidos = 0;      // se usa para la prueba de abandono
            int erroresSeguidos = 0;       // evita que un bot atascado mande jugadas inválidas sin parar

            try
            {
                // El primer mensaje obligatorio: pedir entrar a la sala con un nombre
                conexion.Enviar(new Mensaje { Tipo = Tipos.Unirse, Nombre = nombre });

                // Bucle principal: lee lo que manda el servidor y reacciona según el tipo de mensaje
                while (true)
                {
                    Mensaje m = conexion.Recibir();   // bloquea hasta que llegue una línea
                    if (m == null)
                    {
                        // Recibir devuelve null cuando el servidor cierra la conexión
                        Log(nombre, "El servidor cerró la conexión.");
                        break;
                    }

                    switch (m.Tipo)
                    {
                        // El servidor aceptó al bot y le asignó un asiento (0 a 3)
                        case Tipos.Bienvenida:
                            Log(nombre, "Entró a la sala con el asiento " + m.Indice + ".");
                            break;

                        // Sala llena, nombre repetido o partida en curso: el bot termina
                        case Tipos.Rechazo:
                            Log(nombre, "RECHAZADO: " + m.Texto);
                            return;

                        // Quiénes están en la sala de espera (por ejemplo "2/4")
                        case Tipos.Sala:
                            if (imprimirTodo)
                                Log(nombre, "Sala " + m.Texto + ": " + string.Join(", ", m.Nombres));
                            break;

                        // Texto de lo que pasó en la partida ("Bot2 jugó Rojo 5.")
                        case Tipos.Evento:
                            if (imprimirTodo)
                                Log(nombre, m.Texto);
                            break;

                        // El servidor rechazó la última acción del bot
                        case Tipos.Error:
                            Log(nombre, "ERROR del servidor: " + m.Texto);
                            erroresSeguidos++;
                            if (erroresSeguidos > 3)
                            {
                                Log(nombre, "Demasiados errores seguidos; el bot se detiene.");
                                return;
                            }
                            // Si su jugada fue rechazada, se queda sin acción: roba para no trabar la partida
                            if (ultimo != null)
                                Actuar(conexion, ultimo, azar, true);
                            break;

                        // Foto de la mesa personalizada para este jugador. Llega después de cada acción de cualquiera.
                        case Tipos.Estado:
                            ultimo = m.Estado;
                            estadosRecibidos++;
                            erroresSeguidos = 0;   // si llega un estado nuevo, la acción anterior fue válida

                            // Solo para el primer bot y el primer estado: muestra el JSON completo,
                            // útil para ver qué información recibe un jugador (su mano, pero no las de los demás)
                            if (imprimirTodo && estadosRecibidos == 1)
                            {
                                JsonSerializerOptions bonito = new JsonSerializerOptions(Protocolo.Opciones);
                                bonito.WriteIndented = true;
                                Log(nombre, "Primer estado recibido (esto es todo lo que ve un jugador):\n"
                                    + JsonSerializer.Serialize(ultimo, bonito));
                            }

                            // Prueba de desconexión a media partida: el bot se va sin avisar
                            if (abandonaTras > 0 && estadosRecibidos >= abandonaTras)
                            {
                                Log(nombre, "Se desconecta a propósito.");
                                return;
                            }
                            // Si es su turno, Actuar manda una acción; si no, no hace nada
                            Actuar(conexion, ultimo, azar, false);
                            break;

                        // La partida terminó; m.Texto trae el nombre del ganador
                        case Tipos.Fin:
                            Log(nombre, "FIN de la partida. Ganador: " + m.Texto);
                            return;

                        // Otro jugador se desconectó y el servidor canceló la partida
                        case Tipos.Abortada:
                            Log(nombre, "PARTIDA CANCELADA: " + m.Texto);
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                // Cualquier fallo de red (conexión cortada, JSON inválido...)
                Log(nombre, "Conexión terminada: " + ex.Message);
            }
            finally
            {
                // Pase lo que pase (return, break o excepción), se cierra el socket
                conexion.Cerrar();
            }
        }

        // Una sola acción por cada estado que llega en su turno.
        //   forzarRobar: true cuando el servidor rechazó su jugada; en ese caso roba sin intentar otra cosa
        static void Actuar(ConexionJson conexion, EstadoJuego e, Random azar, bool forzarRobar)
        {
            // Si la partida terminó o no es su turno, no hace nada
            if (e.Terminada || e.Turno != e.TuIndice) return;
            Thread.Sleep(PausaMs);   // pausa "humana" antes de actuar

            // Comodín robado: solo falta elegir el color (el servidor lo tiene pendiente)
            if (e.CartaPendiente != null)
            {
                conexion.Enviar(new Mensaje { Tipo = Tipos.ElegirColor, Color = ColorMasComun(e.TuMano, azar) });
                return;
            }

            // Cartas de su mano que se pueden jugar ahora
            List<CartaDto> jugables = e.TuMano.Where(c => EsJugable(c, e)).ToList();
            if (forzarRobar || jugables.Count == 0)
            {
                conexion.Enviar(new Mensaje { Tipo = Tipos.Robar });
                return;
            }

            // 3 de cada 4 veces dice UNO; la otra "se le olvida" para probar la penalización
            if (e.PuedesDecirUno && azar.Next(4) != 0)
            {
                conexion.Enviar(new Mensaje { Tipo = Tipos.DecirUno });
                return;   // el servidor manda un estado nuevo y ahí juega
            }

            // Juega una carta jugable al azar; si es comodín, también manda el color elegido
            CartaDto elegida = jugables[azar.Next(jugables.Count)];
            bool comodin = elegida.Tipo == "Comodin" || elegida.Tipo == "Mas4";
            conexion.Enviar(new Mensaje
            {
                Tipo = Tipos.Jugar,
                Carta = elegida,
                Color = comodin ? ColorMasComun(e.TuMano, azar) : "Ninguno"
            });
        }

        // Copia de la regla para que el bot sepa qué intentar. Quien decide siempre es el servidor:
        // si esta copia y el servidor no coinciden, el servidor responde "error" y se nota en la consola.
        static bool EsJugable(CartaDto carta, EstadoJuego e)
        {
            // Los comodines se pueden jugar siempre
            if (carta.Tipo == "Comodin" || carta.Tipo == "Mas4") return true;
            // Mismo color que el color activo de la mesa
            if (carta.Color == e.ColorActivo) return true;
            CartaDto tope = e.CartaSuperior;
            // Mismo número que la carta de arriba (solo entre cartas numéricas)
            if (carta.Tipo == "Numero" && tope.Tipo == "Numero" && carta.Numero == tope.Numero) return true;
            // Mismo símbolo (Salta sobre Salta, +2 sobre +2, Reversa sobre Reversa)
            if (carta.Tipo != "Numero" && carta.Tipo == tope.Tipo) return true;
            return false;
        }

        // Elige el color del que el bot tiene más cartas en la mano.
        // Si no tiene ninguna de color (solo comodines), se queda con uno al azar.
        static string ColorMasComun(List<CartaDto> mano, Random azar)
        {
            string[] colores = { "Rojo", "Azul", "Verde", "Amarillo" };
            string mejor = colores[azar.Next(colores.Length)];   // valor inicial al azar
            int maximo = 0;
            foreach (string color in colores)
            {
                int cantidad = mano.Count(c => c.Color == color);
                if (cantidad > maximo)
                {
                    maximo = cantidad;
                    mejor = color;
                }
            }
            return mejor;
        }

        // Imprime una línea en la consola con el nombre del bot delante, para saber quién habla
        static void Log(string nombre, string texto)
        {
            Console.WriteLine("[" + nombre + "] " + texto);
        }
    }
}