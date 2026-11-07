using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using UnoGame.Red;

// Ventana donde el jugador escribe su nombre y la IP del servidor para unirse a la partida
namespace UnoGame
{
    public partial class FormConexion : Form
    {
        ClienteRed cliente;          // la conexión en curso
        bool conectando;             // true mientras esperamos al servidor
        bool resuelto;               // true cuando ya hubo bienvenida o rechazo (ignora avisos tardíos)
        string ipAGuardar;

        // Lo que la sala de espera (#10) necesita cuando esta ventana se cierra con DialogResult.OK
        public ClienteRed Cliente { get; private set; }   // conexión ya abierta y aceptada
        public int Asiento { get; private set; }          // asiento asignado por el servidor (0..3)
        public string Nombre { get; private set; }        // nombre con el que entramos

        public FormConexion()
        {
            InitializeComponent();   // Crea los controles del diseñador
            BackgroundImage = Properties.Resources.fondomenu;   // Mismo fondo que el menú
            txtPuerto.Text = Protocolo.PuertoPorDefecto.ToString();
            txtIp.Text = LeerUltimaIp();
        }

        // ---------- Validación ----------

        // Revisa los campos; si algo está mal avisa y devuelve false
        bool Validar(out string nombre, out string ip, out int puerto)
        {
            nombre = txtNombre.Text.Trim();
            ip = txtIp.Text.Trim();
            puerto = 0;

            if (nombre.Length == 0 || ip.Length == 0 || txtPuerto.Text.Trim().Length == 0)
            {
                Aviso("Completa tu nombre, la IP del servidor y el puerto.");
                return false;
            }

            if (ip.Equals("localhost", StringComparison.OrdinalIgnoreCase)) ip = "127.0.0.1";

            IPAddress direccion;
            if (!IPAddress.TryParse(ip, out direccion) || direccion.AddressFamily != AddressFamily.InterNetwork
                || ip.Split('.').Length != 4)
            {
                Aviso("La IP del servidor no es válida (ejemplo: 192.168.1.20).");
                txtIp.Focus();
                return false;
            }

            if (!int.TryParse(txtPuerto.Text.Trim(), out puerto) || puerto < 1 || puerto > 65535)
            {
                Aviso("El puerto debe ser un número entre 1 y 65535.");
                txtPuerto.Focus();
                return false;
            }

            return true;
        }

        void Aviso(string texto)
        {
            MessageBox.Show(this, texto, "Conexión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ---------- Conectar ----------

        private async void btnConectar_Click(object sender, EventArgs e)
        {
            if (conectando) return;

            string nombre, ip;
            int puerto;
            if (!Validar(out nombre, out ip, out puerto)) return;

            EstablecerConectando(true);
            resuelto = false;
            ClienteRed nuevo = new ClienteRed();
            nuevo.MensajeRecibido += AlRecibirMensaje;   // nos suscribimos ANTES de conectar para no perder la bienvenida
            nuevo.Desconectado += AlDesconectar;
            cliente = nuevo;
            ipAGuardar = ip;
            Nombre = nombre;

            try
            {
                await nuevo.ConectarAsync(ip, puerto, nombre, this);
            }
            catch (Exception ex)
            {
                DescartarCliente();
                if (!IsDisposed)
                {
                    EstablecerConectando(false);
                    MessageBox.Show(this,
                        "No se pudo conectar al servidor " + ip + ":" + puerto + ".\n\n" +
                        "Revisa la IP, el puerto y que el servidor esté encendido.\n(" + ex.Message + ")",
                        "No se pudo conectar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // La ventana se cerró mientras conectábamos: no dejamos la conexión abierta
            if (IsDisposed)
            {
                nuevo.Cerrar();
                return;
            }
            // Ahora esperamos la respuesta del servidor (bienvenida o rechazo) en AlRecibirMensaje
        }

        void AlRecibirMensaje(Mensaje mensaje)
        {
            if (resuelto) return;

            if (mensaje.Tipo == Tipos.Bienvenida)
            {
                resuelto = true;
                Asiento = mensaje.Indice.HasValue ? mensaje.Indice.Value : 0;   // guardamos nuestro asiento
                GuardarUltimaIp(ipAGuardar);

                // Entregamos la conexión a la siguiente ventana: dejamos de escuchar aquí
                Cliente = cliente;
                cliente.MensajeRecibido -= AlRecibirMensaje;
                cliente.Desconectado -= AlDesconectar;
                conectando = false;
                DialogResult = DialogResult.OK;   // cierra esta ventana -> sala de espera (#10)
            }
            else if (mensaje.Tipo == Tipos.Rechazo)
            {
                resuelto = true;
                string motivo = string.IsNullOrWhiteSpace(mensaje.Texto) ? "El servidor rechazó la conexión." : mensaje.Texto;
                DescartarCliente();   // cerramos la conexión
                EstablecerConectando(false);
                MessageBox.Show(this, motivo, "Conexión rechazada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        void AlDesconectar(string motivo)
        {
            if (resuelto) return;   // ya respondimos a un rechazo; este aviso es el cierre normal
            resuelto = true;
            DescartarCliente();
            EstablecerConectando(false);
            MessageBox.Show(this, motivo, "Conexión perdida", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Cierra y olvida la conexión en curso (si no se entregó a la sala de espera)
        void DescartarCliente()
        {
            if (cliente != null)
            {
                cliente.MensajeRecibido -= AlRecibirMensaje;
                cliente.Desconectado -= AlDesconectar;
                cliente.Cerrar();
                cliente = null;
            }
        }

        // Habilita o deshabilita el formulario mientras se conecta
        void EstablecerConectando(bool valor)
        {
            conectando = valor;
            btnConectar.Enabled = !valor;
            txtNombre.Enabled = !valor;
            txtIp.Enabled = !valor;
            txtPuerto.Enabled = !valor;
            lblEstado.Text = valor ? "Conectando..." : "";
        }

        // Volver: equivalente a cerrar la ventana sin conectar
        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (conectando) return;   // no permitir salir mientras se está conectando
            DialogResult = DialogResult.Cancel;
        }

        // Enter = Conectar, Escape = Volver (reemplaza AcceptButton/CancelButton del Button)
        private void FormConexion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.Handled = true; btnConectar_Click(sender, EventArgs.Empty); }
            if (e.KeyCode == Keys.Escape) { e.Handled = true; btnVolver_Click(sender, EventArgs.Empty); }
        }

        // Si se cierra la ventana sin éxito, no dejamos conexiones abiertas
        private void FormConexion_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult != DialogResult.OK) DescartarCliente();
        }

        // ---------- Recordar la última IP ----------

        static string RutaConfig()
        {
            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UnoGame");
            return Path.Combine(carpeta, "ultima_ip.txt");
        }

        static string LeerUltimaIp()
        {
            try
            {
                string ruta = RutaConfig();
                if (File.Exists(ruta)) return File.ReadAllText(ruta).Trim();
            }
            catch (Exception) { }   // si no se puede leer, simplemente empezamos vacío
            return "";
        }

        static void GuardarUltimaIp(string ip)
        {
            try
            {
                string ruta = RutaConfig();
                Directory.CreateDirectory(Path.GetDirectoryName(ruta));
                File.WriteAllText(ruta, ip);
            }
            catch (Exception) { }   // no es crítico
        }
    }
}