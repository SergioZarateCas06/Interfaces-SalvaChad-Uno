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
    public partial class FormConfiguracion : Form
    {
        String jugador1, jugador2, jugador3, jugador4;
        public FormConfiguracion()
        {
            InitializeComponent();
            BackgroundImage = Properties.Resources.fondomenu;
            pictureBox1.Image = Properties.Resources.camporojo;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = Properties.Resources.campoazul;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.Image = Properties.Resources.campoverde;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.Image = Properties.Resources.campoamarillo;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }


        bool comprobar()
        {
            string j1 = richTextBox2.Text.Trim();
            string j2 = richTextBox3.Text.Trim();
            string j3 = richTextBox5.Text.Trim();
            string j4 = richTextBox7.Text.Trim();

            if (string.IsNullOrWhiteSpace(j1) || string.IsNullOrWhiteSpace(j2) ||string.IsNullOrWhiteSpace(j3) || string.IsNullOrWhiteSpace(j4))
            {
              MessageBox.Show("No puede haber nombres vacíos");
              return false;
            }
            if (j1==j2||j1==j3||j1==j4||j2==j3||j2==j4||j3==j4)
            {
                MessageBox.Show("Los nombres no pueden ser iguales");
                return false;
            }

            return true;
        }
        private void botonJugar_Click(object sender, EventArgs e)
        {
            
            if (comprobar())
            {
                jugador1 = richTextBox2.Text;
                jugador2 = richTextBox3.Text;
                jugador3 = richTextBox5.Text;
                jugador4 = richTextBox7.Text;

                this.Hide();
                Form juego = new Form1(jugador1, jugador2, jugador3, jugador4);
                juego.ShowDialog();
                if (juego.DialogResult == DialogResult.Abort)
                {
                    this.Close();
                }
                else
                {
                    this.Show();
                }
            }
        }
    }
}
