using System;    // Funciones básicas de C#
using System.Collections.Generic;  // Listas y colecciones
using System.ComponentModel;  // Soporte para componentes de Windows
using System.Data; // Manejo de datos   
using System.Drawing; // Dibujo: imágenes, colores y tamaños q
using System.Linq;  // Consultas sobre listas   
using System.Text;  // Manejo de texto
using System.Threading.Tasks;  // Tareas asíncronas
using System.Windows.Forms;   // Ventanas y controles de Windows Forms

namespace UnoGame
{
    // Es la ventana de fin de partida
    public partial class FormFinPartida : Form
    {
        // Se ejecuta al abrir la ventana. Crea los controles del diseñador
        public FormFinPartida()
        {
            InitializeComponent();
        }
        // Se ejecuta al hacer clic en "Volver al menú"
        private void pbVolverMenu_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
        }
        // Se ejecuta al hacer clic en "Jugar de nuevo"
        private void pbJugarDeNuevo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
        }
        // MostrarGanado
        public void MostrarGanador(string ganador)
        {
            labelGanador.Text = $"¡El ganador es: {ganador}!";
        }
    }
}
