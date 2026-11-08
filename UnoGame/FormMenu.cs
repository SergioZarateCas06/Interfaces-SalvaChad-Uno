using NAudio.CoreAudioApi;
using System;  // Funciones básicas de C#
using System.Collections.Generic;  // Listas y colecciones
using System.ComponentModel; // Soporte para componentes de Windows Forms
using System.Data;    // Manejo de datos
using System.Drawing;   // Dibujo: imágenes, colores y tamaños
using System.Linq; // Consultas sobre listas
using System.Text; // Manejo de texto
using System.Threading.Tasks;  // Tareas asíncronas
using System.Windows.Forms; // Ventanas y controles de Windows Forms
                            // Es la ventana del menú principal
namespace UnoGame
{
    public partial class FormMenu : Form
    {

        public FormMenu()
        {
            InitializeComponent();
        }
        // Esconde el menú, abre la ventana de conexión al servidor.
        // Si al terminar la partida se elige "Jugar de nuevo", se vuelve a FormConexion
        // con el nombre y la IP ya puestos, sin reiniciar el programa.
        private void pbNuevaPartida_Click(object sender, EventArgs e)
        {
            this.Hide();
            string nombre = "", ip = null;
            int puerto = 0;
            bool otraVez;
            do
            {
                otraVez = false;
                FormConexion conexion = new FormConexion(nombre, ip, puerto);
                conexion.ShowDialog();
                if (conexion.DialogResult == DialogResult.OK)
                {
                    // Recordamos los datos por si piden jugar de nuevo
                    nombre = conexion.Nombre;
                    ip = conexion.Ip;
                    puerto = conexion.Puerto;

                    // Conectados: sala de espera, que abre la mesa cuando empieza la partida
                    FormSalaEspera sala = new FormSalaEspera(conexion.Cliente, conexion.Nombre);
                    sala.ShowDialog();
                    otraVez = sala.DialogResult == DialogResult.Retry;
                    sala.Dispose();
                }
                conexion.Dispose();
            } while (otraVez);
            this.Show();
        }
        // Se ejecuta al hacer clic en "Historial"
        private void pbHistorial_Click(object sender, EventArgs e)
        {
            string ip = FormConexion.LeerUltimaIp();   // con el nombre de la clase
            if (!string.IsNullOrWhiteSpace(ip)) ApiCliente.Host = ip;
            this.Hide();
            FormHistorial historial = new FormHistorial();
            historial.ShowDialog();
            this.Show();
        }
        // Se ejecuta al hacer clic en "Salir"
        private void pbSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        // Se ejecuta cuando el menú se abre
        private void FormMenu_Load(object sender, EventArgs e)
        {

        }
    }
}