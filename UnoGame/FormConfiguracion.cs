using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Es la ventana donde se escriben los nombres de los jugadores
namespace UnoGame
{
    public partial class FormConfiguracion : Form
    {
        String jugador1, jugador2, jugador3, jugador4;
        public FormConfiguracion()
        {
            InitializeComponent();   // Crea los botones y cuadros de texto del diseñador
            BackgroundImage = Properties.Resources.fondomenu;  // Imagen de fondo de la ventana
            pictureBox1.Image = Properties.Resources.camporojo; // Campo del jugador 1
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = Properties.Resources.campoazul;// Campo del jugador 2
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.Image = Properties.Resources.campoverde;// Campo del jugador 3
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.Image = Properties.Resources.campoamarillo; // Campo del jugador 4
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        // Revisa que los nombres escritos sean correcto
        bool comprobar()
        {
            // Leemos los nombres de los cuadros de texto
            string j1 = richTextBox2.Text.Trim();
            string j2 = richTextBox3.Text.Trim();
            string j3 = richTextBox5.Text.Trim();
            string j4 = richTextBox7.Text.Trim();
            // Si algún nombre está vacío (o solo tiene espacios), avisamos
            if (string.IsNullOrWhiteSpace(j1) || string.IsNullOrWhiteSpace(j2) ||string.IsNullOrWhiteSpace(j3) || string.IsNullOrWhiteSpace(j4))
            {
              MessageBox.Show("No puede haber nombres vacíos");
              return false;
            }
            // Si dos nombres son iguales (se comparan todas las parejas), avisamos
            if (j1==j2||j1==j3||j1==j4||j2==j3||j2==j4||j3==j4)
            {
                MessageBox.Show("Los nombres no pueden ser iguales");
                return false;
            }

            return true;   // Todo correcto
        }
        // Si los nombres son válidos, esconde esta ventana y abre el juego
        private void botonJugar_Click(object sender, EventArgs e)
        {
            
            if (comprobar())
            {
                // Guardamos los nombres escritos
                jugador1 = richTextBox2.Text;
                jugador2 = richTextBox3.Text;
                jugador3 = richTextBox5.Text;
                jugador4 = richTextBox7.Text;

                this.Hide(); // Esconde la ventana de configuración
                  // Abre la ventana del juego con los 4 nombre
                Form juego = new Form1(jugador1, jugador2, jugador3, jugador4);
                juego.ShowDialog();
                if (juego.DialogResult == DialogResult.Abort)
                {
                    this.Close(); // El jugador salió: se cierra también esta ventana
                }
                else
                {
                    this.Show(); // Se vuelve a mostrar la configuración
                }
            }
        }
    }
}
