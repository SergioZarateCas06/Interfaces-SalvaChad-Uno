using System;
using System.Collections.Generic;
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
    //
    // Los mensajes pasan por una cola: si en un momento nadie está suscrito (por ejemplo, mientras
    // se pasa de una ventana a otra), esperan en la cola y se entregan en orden cuando alguien
    // se suscribe y llama a CambiarInterfaz. Así no se pierde ningún mensaje.
    public class ClienteRed
    {
        ConexionJson conexion;
        volatile Control interfaz;
        volatile bool cerrando;
        readonly object candado = new object();
        // Mensajes (Mensaje) o avisos de desconexión (string) pendientes de entregar, en orden de llegada
        readonly Queue<object> bandeja = new Queue<object>();

        public event Action<Mensaje> MensajeRecibido;   // llegó algo del servidor
        public event Action<string> Desconectado;       // se perdió la conexión (con el motivo)
        public bool Conectado { get; private set; }

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

        // Pasa la entrega de mensajes a otra ventana. Orden correcto al cambiar de ventana:
        //   1) la ventana nueva se suscribe a MensajeRecibido / Desconectado
        //   2) la ventana nueva llama a CambiarInterfaz(this)
        // Lo que haya quedado en la cola se le entrega en cuanto su ventana esté lista.
        public void CambiarInterfaz(Control nueva)
        {
            interfaz = nueva;
            if (nueva == null) return;

            if (nueva.IsHandleCreated)
            {
                Avisar();
            }
            else
            {
                // La ventana aún no se ha mostrado (p. ej. se llamó desde su constructor):
                // esperamos a que tenga handle para poder entregarle los mensajes.
                EventHandler alCrear = null;
                alCrear = (s, e) =>
                {
                    nueva.HandleCreated -= alCrear;
                    Avisar();
                };
                nueva.HandleCreated += alCrear;
            }
        }

        void Escuchar()
        {
            try
            {
                while (true)
                {
                    Mensaje mensaje = conexion.Recibir();   // espera hasta que llegue una línea
                    if (mensaje == null) break;
                    Encolar(mensaje);
                }
            }
            catch (Exception)
            {
                // la conexión se cortó: se avisa más abajo
            }

            Conectado = false;
            if (!cerrando) Encolar("Se perdió la conexión con el servidor.");
        }

        void Encolar(object elemento)
        {
            lock (candado)
            {
                bandeja.Enqueue(elemento);
            }
            Avisar();
        }

        // Pide a la ventana actual que vacíe la cola (en el hilo de la interfaz)
        void Avisar()
        {
            Control c = interfaz;
            if (c == null || c.IsDisposed || !c.IsHandleCreated) return;   // se queda en cola
            try { c.BeginInvoke(new Action(Vaciar)); }
            catch (InvalidOperationException) { }   // la ventana se cerró justo ahora
        }

        // Se ejecuta en el hilo de la interfaz: entrega los mensajes en orden mientras haya
        // alguien suscrito. Si no hay nadie, los deja en la cola.
        void Vaciar()
        {
            while (true)
            {
                Mensaje mensaje = null;
                string motivo = null;
                Action<Mensaje> alMensaje = null;
                Action<string> alDesconectar = null;

                lock (candado)
                {
                    if (bandeja.Count == 0) return;
                    object siguiente = bandeja.Peek();
                    mensaje = siguiente as Mensaje;
                    if (mensaje != null)
                    {
                        alMensaje = MensajeRecibido;
                        if (alMensaje == null) return;   // nadie escucha todavía
                    }
                    else
                    {
                        motivo = (string)siguiente;
                        alDesconectar = Desconectado;
                        if (alDesconectar == null) return;
                    }
                    bandeja.Dequeue();
                }

                // Se invoca fuera del candado: el manejador puede cambiar de ventana,
                // suscribirse o incluso abrir otra ventana modal.
                if (alMensaje != null) alMensaje(mensaje);
                else alDesconectar(motivo);
            }
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
            lock (candado) { bandeja.Clear(); }
            if (conexion != null) conexion.Cerrar();
        }
    }
}