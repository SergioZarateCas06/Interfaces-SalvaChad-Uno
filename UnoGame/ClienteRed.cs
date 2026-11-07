using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnoGame.Red;

namespace UnoGame
{
    // Lado cliente de la conexión: se conecta al servidor, escucha sus mensajes en un hilo
    // aparte y se los entrega al formulario ya en el hilo de la interfaz (BeginInvoke),
    // porque los controles de Windows Forms solo se pueden tocar desde ese hilo.
    public class ClienteRed
    {
        ConexionJson conexion;
        volatile Control interfaz;
        volatile bool cerrando;

        public event Action<Mensaje> MensajeRecibido;   // llegó algo del servidor
        public event Action<string> Desconectado;       // se perdió la conexión (con el motivo)
        public bool Conectado { get; private set; }
        public volatile Mensaje UltimaSala;             // último mensaje "sala" recibido (para quien tome el relevo)

        // Cambia el formulario que recibe los eventos (al pasar de una ventana a otra)
        public void CambiarInterfaz(Control nueva)
        {
            interfaz = nueva;
        }

        // Abre la conexión y manda el mensaje "unirse". Lanza excepción si no se pudo conectar.
        // "interfaz" es el formulario que recibirá los eventos.
        public async Task ConectarAsync(string ip, int puerto, string nombre, Control interfaz)
        {
            this.interfaz = interfaz;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            try
            {
                // Connect bloquea; lo mandamos a otro hilo para no congelar la ventana
                await Task.Run(() => socket.Connect(ip, puerto));
            }
            catch
            {
                socket.Close();
                throw;
            }

            conexion = new ConexionJson(socket);
            Conectado = true;
            cerrando = false;
            Enviar(new Mensaje { Tipo = Tipos.Unirse, Nombre = nombre });

            Thread hilo = new Thread(Escuchar);
            hilo.IsBackground = true;   // si se cierra el programa, este hilo no lo mantiene vivo
            hilo.Start();
        }

        void Escuchar()
        {
            try
            {
                while (true)
                {
                    Mensaje mensaje = conexion.Recibir();   // espera hasta que llegue una línea
                    if (mensaje == null) break;
                    if (mensaje.Tipo == Tipos.Sala) UltimaSala = mensaje;   // se guarda aunque la ventana esté cambiando
                    EnHiloDeInterfaz(() =>
                    {
                        Action<Mensaje> manejador = MensajeRecibido;
                        if (manejador != null) manejador(mensaje);
                    });
                }
            }
            catch (Exception)
            {
                // la conexión se cortó: se avisa más abajo
            }

            Conectado = false;
            if (!cerrando)
            {
                EnHiloDeInterfaz(() =>
                {
                    Action<string> manejador = Desconectado;
                    if (manejador != null) manejador("Se perdió la conexión con el servidor.");
                });
            }
        }

        void EnHiloDeInterfaz(Action accion)
        {
            Control c = interfaz;
            if (c == null || c.IsDisposed || !c.IsHandleCreated) return;
            try { c.BeginInvoke(accion); }
            catch (InvalidOperationException) { }   // la ventana se cerró justo ahora
        }

        public void Enviar(Mensaje mensaje)
        {
            try { conexion.Enviar(mensaje); }
            catch (Exception) { }   // si falló, el hilo que escucha avisará con Desconectado
        }

        // --- Atajos para cada acción del jugador ---
        public void Jugar(Card carta, CardColor color)
        {
            Enviar(new Mensaje { Tipo = Tipos.Jugar, Carta = CartaDto.De(carta), Color = color.ToString() });
        }

        public void Robar()
        {
            Enviar(new Mensaje { Tipo = Tipos.Robar });
        }

        public void ElegirColor(CardColor color)
        {
            Enviar(new Mensaje { Tipo = Tipos.ElegirColor, Color = color.ToString() });
        }

        public void DecirUno()
        {
            Enviar(new Mensaje { Tipo = Tipos.DecirUno });
        }

        public void Cerrar()
        {
            cerrando = true;
            Conectado = false;
            if (conexion != null) conexion.Cerrar();
        }
    }
}