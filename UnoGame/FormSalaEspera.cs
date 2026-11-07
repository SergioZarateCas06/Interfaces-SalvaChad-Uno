using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UnoGame.Red;

// Sala de espera: muestra quién está conectado mientras faltan jugadores.
// Cuando llega el primer estado de la partida, abre la mesa (Form1).
namespace UnoGame
{
    public partial class FormSalaEspera : Form
    {
        const int Jugadores = 4;

        readonly ClienteRed cliente;
        readonly string nombre;       // nuestro nombre, para marcarnos con "(tú)"
        readonly Label[] casillas;    // las 4 filas de la lista
        readonly Color[] coloresAsiento =
        {
            Color.FromArgb(200, 40, 40),    // rojo
            Color.FromArgb(40, 100, 210),   // azul
            Color.FromArgb(40, 150, 70),    // verde
            Color.FromArgb(215, 165, 20)    // amarillo
        };
        bool saliendo;                // ya estamos cerrando la sala (evita avisos dobles)
        bool abriendoMesa;            // ya llegó el primer estado

        public FormSalaEspera(ClienteRed cliente, string nombre)
        {
            InitializeComponent();
            BackgroundImage = Properties.Resources.fondomenu;   // Mismo fondo que el menú

            this.cliente = cliente;
            this.nombre = nombre;
            casillas = new Label[] { lblJugador1, lblJugador2, lblJugador3, lblJugador4 };
            MostrarSala(null);

            // Orden importante: primero suscribirse y después CambiarInterfaz,
            // así lo que haya llegado mientras se cambiaba de ventana se entrega en orden.
            cliente.MensajeRecibido += AlRecibirMensaje;
            cliente.Desconectado += AlDesconectar;
            cliente.CambiarInterfaz(this);
        }

        // ---------- Mensajes del servidor ----------

        void AlRecibirMensaje(Mensaje mensaje)
        {
            if (saliendo || abriendoMesa) return;

            if (mensaje.Tipo == Tipos.Sala)
            {
                MostrarSala(mensaje.Nombres);
            }
            else if (mensaje.Tipo == Tipos.Estado)
            {
                AbrirMesa(mensaje.Estado);
            }
            else if (mensaje.Tipo == Tipos.Abortada)
            {
                Volver(string.IsNullOrWhiteSpace(mensaje.Texto) ? "La partida fue cancelada." : mensaje.Texto);
            }
        }

        void AlDesconectar(string motivo)
        {
            if (saliendo) return;
            Volver(motivo);
        }

        // Actualiza la lista y el contador ("2/4")
        void MostrarSala(List<string> nombres)
        {
            int cantidad = nombres == null ? 0 : nombres.Count;
            for (int i = 0; i < Jugadores; i++)
            {
                if (i < cantidad)
                {
                    bool soyYo = string.Equals(nombres[i], nombre, StringComparison.OrdinalIgnoreCase);
                    casillas[i].Text = nombres[i] + (soyYo ? "  (tú)" : "");
                    casillas[i].BackColor = coloresAsiento[i];
                    casillas[i].ForeColor = Color.White;
                }
                else
                {
                    casillas[i].Text = "Esperando jugador...";
                    casillas[i].BackColor = Color.FromArgb(90, 255, 255, 255);
                    casillas[i].ForeColor = Color.FromArgb(220, 220, 220);
                }
            }
            lblContador.Text = cantidad + "/" + Jugadores;
        }

        // ---------- Pasar a la mesa ----------

        void AbrirMesa(EstadoJuego estado)
        {
            abriendoMesa = true;

            // Traspaso: nos desuscribimos antes de crear Form1, que se suscribe en su constructor
            // y llama a cliente.CambiarInterfaz(this). Lo que llegue en medio queda en la cola.
            cliente.MensajeRecibido -= AlRecibirMensaje;
            cliente.Desconectado -= AlDesconectar;

            Opacity = 0;   // escondemos la sala sin cerrar el diálogo modal
            Form1 mesa = new Form1(cliente, estado);
            mesa.ShowDialog();
            mesa.Dispose();

            // La partida terminó (o se salió): se cierra la conexión y se vuelve al menú
            saliendo = true;
            cliente.Cerrar();
            DialogResult = DialogResult.Cancel;
        }

        // ---------- Salir ----------

        // Cierra la conexión, avisa (si hay motivo) y vuelve al menú
        void Volver(string aviso)
        {
            saliendo = true;
            cliente.MensajeRecibido -= AlRecibirMensaje;
            cliente.Desconectado -= AlDesconectar;
            cliente.Cerrar();
            if (!string.IsNullOrEmpty(aviso))
                MessageBox.Show(this, aviso, "Sala de espera", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.Cancel;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (saliendo || abriendoMesa) return;
            Volver(null);
        }

        // Escape = Salir
        private void FormSalaEspera_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                btnSalir_Click(sender, EventArgs.Empty);
            }
        }

        // Si cierran con la X, también soltamos la conexión para que el servidor lo note
        private void FormSalaEspera_FormClosed(object sender, FormClosedEventArgs e)
        {
            cliente.MensajeRecibido -= AlRecibirMensaje;
            cliente.Desconectado -= AlDesconectar;
            cliente.Cerrar();
        }
    }
}