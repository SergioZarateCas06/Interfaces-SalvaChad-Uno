namespace UnoGame
{
    partial class FormFinPartida
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelGanador = new System.Windows.Forms.Label();
            this.pbVolverMenu = new System.Windows.Forms.PictureBox();
            this.pbJugarDeNuevo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbVolverMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbJugarDeNuevo)).BeginInit();
            this.SuspendLayout();
            // 
            // labelGanador
            // 
            this.labelGanador.BackColor = System.Drawing.Color.Transparent;
            this.labelGanador.Font = new System.Drawing.Font("Segoe UI", 28.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGanador.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(230)))), ((int)(((byte)(200)))));
            this.labelGanador.Location = new System.Drawing.Point(12, 22);
            this.labelGanador.Name = "labelGanador";
            this.labelGanador.Size = new System.Drawing.Size(776, 120);
            this.labelGanador.TabIndex = 0;
            this.labelGanador.Text = "label1";
            this.labelGanador.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbVolverMenu
            // 
            this.pbVolverMenu.BackColor = System.Drawing.Color.Transparent;
            this.pbVolverMenu.Image = global::UnoGame.Properties.Resources.ImagenVolverAlMenu;
            this.pbVolverMenu.Location = new System.Drawing.Point(470, 267);
            this.pbVolverMenu.Name = "pbVolverMenu";
            this.pbVolverMenu.Size = new System.Drawing.Size(318, 87);
            this.pbVolverMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbVolverMenu.TabIndex = 2;
            this.pbVolverMenu.TabStop = false;
            this.pbVolverMenu.Click += new System.EventHandler(this.pbVolverMenu_Click);
            // 
            // pbJugarDeNuevo
            // 
            this.pbJugarDeNuevo.BackColor = System.Drawing.Color.Transparent;
            this.pbJugarDeNuevo.Image = global::UnoGame.Properties.Resources.ImagenJugarDeNuevo;
            this.pbJugarDeNuevo.Location = new System.Drawing.Point(12, 267);
            this.pbJugarDeNuevo.Name = "pbJugarDeNuevo";
            this.pbJugarDeNuevo.Size = new System.Drawing.Size(318, 87);
            this.pbJugarDeNuevo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbJugarDeNuevo.TabIndex = 1;
            this.pbJugarDeNuevo.TabStop = false;
            this.pbJugarDeNuevo.Click += new System.EventHandler(this.pbJugarDeNuevo_Click);
            // 
            // FormFinPartida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UnoGame.Properties.Resources.fondomenu;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pbVolverMenu);
            this.Controls.Add(this.pbJugarDeNuevo);
            this.Controls.Add(this.labelGanador);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormFinPartida";
            this.Text = "Fin de la Partida";
            ((System.ComponentModel.ISupportInitialize)(this.pbVolverMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbJugarDeNuevo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelGanador;
        private System.Windows.Forms.PictureBox pbJugarDeNuevo;
        private System.Windows.Forms.PictureBox pbVolverMenu;
    }
}