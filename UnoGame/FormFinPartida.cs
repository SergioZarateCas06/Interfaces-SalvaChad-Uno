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
    public partial class FormFinPartida : Form
    {
        public FormFinPartida()
        {
            InitializeComponent();
        }

        private void pbVolverMenu_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
        }

        private void pbJugarDeNuevo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
        }
        public void MostrarGanador(string ganador)
        {
            labelGanador.Text = $"¡El ganador es: {ganador}!";
        }
    }
}
