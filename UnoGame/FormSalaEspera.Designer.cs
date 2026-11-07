namespace UnoGame
{
    partial class FormSalaEspera
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelSala = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblContador = new System.Windows.Forms.Label();
            this.lblJugador1 = new System.Windows.Forms.Label();
            this.lblJugador2 = new System.Windows.Forms.Label();
            this.lblJugador3 = new System.Windows.Forms.Label();
            this.lblJugador4 = new System.Windows.Forms.Label();
            this.lblAviso = new System.Windows.Forms.Label();
            this.btnSalir = new System.Windows.Forms.PictureBox();
            this.panelSala.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSalir)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSala
            // 
            this.panelSala.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.panelSala.Controls.Add(this.lblTitulo);
            this.panelSala.Controls.Add(this.lblContador);
            this.panelSala.Controls.Add(this.lblJugador1);
            this.panelSala.Controls.Add(this.lblJugador2);
            this.panelSala.Controls.Add(this.lblJugador3);
            this.panelSala.Controls.Add(this.lblJugador4);
            this.panelSala.Controls.Add(this.lblAviso);
            this.panelSala.Controls.Add(this.btnSalir);
            this.panelSala.Location = new System.Drawing.Point(188, 24);
            this.panelSala.Name = "panelSala";
            this.panelSala.Size = new System.Drawing.Size(461, 380);
            this.panelSala.TabIndex = 0;
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.Gold;
            this.lblTitulo.Location = new System.Drawing.Point(0, 14);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(461, 40);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Sala de espera";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblContador
            // 
            this.lblContador.BackColor = System.Drawing.Color.Transparent;
            this.lblContador.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Bold);
            this.lblContador.ForeColor = System.Drawing.Color.White;
            this.lblContador.Location = new System.Drawing.Point(0, 56);
            this.lblContador.Name = "lblContador";
            this.lblContador.Size = new System.Drawing.Size(461, 52);
            this.lblContador.TabIndex = 1;
            this.lblContador.Text = "0/4";
            this.lblContador.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblJugador1
            // 
            this.lblJugador1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.lblJugador1.Location = new System.Drawing.Point(60, 118);
            this.lblJugador1.Name = "lblJugador1";
            this.lblJugador1.Size = new System.Drawing.Size(340, 36);
            this.lblJugador1.TabIndex = 2;
            this.lblJugador1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblJugador2
            // 
            this.lblJugador2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.lblJugador2.Location = new System.Drawing.Point(60, 162);
            this.lblJugador2.Name = "lblJugador2";
            this.lblJugador2.Size = new System.Drawing.Size(340, 36);
            this.lblJugador2.TabIndex = 3;
            this.lblJugador2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblJugador3
            // 
            this.lblJugador3.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.lblJugador3.Location = new System.Drawing.Point(60, 206);
            this.lblJugador3.Name = "lblJugador3";
            this.lblJugador3.Size = new System.Drawing.Size(340, 36);
            this.lblJugador3.TabIndex = 4;
            this.lblJugador3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblJugador4
            // 
            this.lblJugador4.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.lblJugador4.Location = new System.Drawing.Point(60, 250);
            this.lblJugador4.Name = "lblJugador4";
            this.lblJugador4.Size = new System.Drawing.Size(340, 36);
            this.lblJugador4.TabIndex = 5;
            this.lblJugador4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAviso
            // 
            this.lblAviso.BackColor = System.Drawing.Color.Transparent;
            this.lblAviso.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Italic);
            this.lblAviso.ForeColor = System.Drawing.Color.Gold;
            this.lblAviso.Location = new System.Drawing.Point(0, 286);
            this.lblAviso.Name = "lblAviso";
            this.lblAviso.Size = new System.Drawing.Size(461, 24);
            this.lblAviso.TabIndex = 6;
            this.lblAviso.Text = "La partida empieza cuando se unan los 4 jugadores";
            this.lblAviso.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.Transparent;
            this.btnSalir.BackgroundImage = global::UnoGame.Properties.Resources.ImagenBotonVolver;
            this.btnSalir.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.Location = new System.Drawing.Point(9, 332);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(98, 45);
            this.btnSalir.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnSalir.TabIndex = 7;
            this.btnSalir.TabStop = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // FormSalaEspera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(837, 428);
            this.Controls.Add(this.panelSala);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "FormSalaEspera";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sala de espera";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSalaEspera_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormSalaEspera_KeyDown);
            this.panelSala.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnSalir)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSala;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblContador;
        private System.Windows.Forms.Label lblJugador1;
        private System.Windows.Forms.Label lblJugador2;
        private System.Windows.Forms.Label lblJugador3;
        private System.Windows.Forms.Label lblJugador4;
        private System.Windows.Forms.Label lblAviso;
        private System.Windows.Forms.PictureBox btnSalir;
    }
}