using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnoGame
{
    public partial class FormMenu : Form
    {
        public FormMenu()
        {
            InitializeComponent();
        }

        private void pbNuevaPartida_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormConfiguracion juego = new FormConfiguracion();
            juego.ShowDialog();
            this.Show();
        }

        private void pbHistorial_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormHistorial historial = new FormHistorial();
            historial.ShowDialog();
            this.Show();
        }

        private void pbSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FormMenu_Load(object sender, EventArgs e)
        {

        }
    }
}
