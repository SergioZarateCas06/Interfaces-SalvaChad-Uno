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
        // Esconde el menú, abre la ventana de conexión al servidor
        private void pbNuevaPartida_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormConexion conexion = new FormConexion();
            conexion.ShowDialog();
            if (conexion.DialogResult == DialogResult.OK)
            {
                // Conectados: pasamos a la sala de espera, que abre la mesa cuando empieza la partida
                FormSalaEspera sala = new FormSalaEspera(conexion.Cliente, conexion.Nombre);
                sala.ShowDialog();
                sala.Dispose();
            }
            conexion.Dispose();
            this.Show();
        }
        // Se ejecuta al hacer clic en "Historial"
        private void pbHistorial_Click(object sender, EventArgs e)
        {
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