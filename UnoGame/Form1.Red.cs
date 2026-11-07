using System;
using System.Windows.Forms;
using UnoGame.Red;

// Parte "en red" de Form1: constructor que recibe la conexión y el primer estado.
namespace UnoGame
{
    public partial class Form1
    {
        ClienteRed cliente;          // conexión con el servidor (null en el modo local de antes)
        bool modoRed;                // true: la mesa la maneja el servidor, no el reparto local
        EstadoJuego estadoActual;    // último estado recibido

        // Abre la mesa para una partida en red. Lo llama la sala de espera con el primer estado.
        // Reutiliza el constructor anterior con los nombres que vienen en el estado.
        public Form1(ClienteRed cliente, EstadoJuego estadoInicial)
            : this(Nombre(estadoInicial, 0), Nombre(estadoInicial, 1), Nombre(estadoInicial, 2), Nombre(estadoInicial, 3))
        {
            this.cliente = cliente;
            modoRed = true;

            // Primero suscribirse y después CambiarInterfaz: lo que llegó mientras tanto se entrega en orden
            cliente.MensajeRecibido += AlRecibirMensajeRed;
            cliente.Desconectado += AlDesconectarRed;
            cliente.CambiarInterfaz(this);

            this.FormClosed += (s, e) =>
            {
                cliente.MensajeRecibido -= AlRecibirMensajeRed;
                cliente.Desconectado -= AlDesconectarRed;
                cliente.Cerrar();
            };

            AplicarEstado(estadoInicial);
        }

        static string Nombre(EstadoJuego estado, int i)
        {
            if (estado == null || estado.Nombres == null || i >= estado.Nombres.Count) return "Jugador " + (i + 1);
            return estado.Nombres[i];
        }

        void AlRecibirMensajeRed(Mensaje mensaje)
        {
            if (mensaje.Tipo == Tipos.Estado)
            {
                AplicarEstado(mensaje.Estado);
            }
            else if (mensaje.Tipo == Tipos.Abortada)
            {
                CerrarPorRed(string.IsNullOrWhiteSpace(mensaje.Texto) ? "La partida fue cancelada." : mensaje.Texto);
            }
            // TODO (issue de la mesa en red): Evento, Error y Fin
        }

        void AlDesconectarRed(string motivo)
        {
            CerrarPorRed(motivo);
        }

        void CerrarPorRed(string aviso)
        {
            if (cierreForzado) return;
            cierreForzado = true;   // que FormClosing no pregunte "¿quieres salir?"
            MessageBox.Show(this, aviso, "Partida", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.Cancel;
        }

        // Aquí se dibujará la mesa a partir del estado del servidor (mano, descarte, turnos...).
        void AplicarEstado(EstadoJuego estado)
        {
            estadoActual = estado;
            // TODO (issue de la mesa en red): pintar estadoActual
        }
    }
}
