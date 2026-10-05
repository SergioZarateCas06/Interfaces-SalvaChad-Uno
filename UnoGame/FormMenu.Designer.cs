namespace UnoGame
{
    partial class FormMenu
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
            this.pbSalir = new System.Windows.Forms.PictureBox();
            this.pbHistorial = new System.Windows.Forms.PictureBox();
            this.pbNuevaPartida = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbSalir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHistorial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNuevaPartida)).BeginInit();
            this.SuspendLayout();
            // 
            // pbSalir
            // 
            this.pbSalir.BackColor = System.Drawing.Color.Transparent;
            this.pbSalir.Image = global::UnoGame.Properties.Resources.ImagenBotonSalir1;
            this.pbSalir.Location = new System.Drawing.Point(285, 267);
            this.pbSalir.Name = "pbSalir";
            this.pbSalir.Size = new System.Drawing.Size(204, 69);
            this.pbSalir.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSalir.TabIndex = 2;
            this.pbSalir.TabStop = false;
            this.pbSalir.Click += new System.EventHandler(this.pbSalir_Click);
            // 
            // pbHistorial
            // 
            this.pbHistorial.BackColor = System.Drawing.Color.Transparent;
            this.pbHistorial.Image = global::UnoGame.Properties.Resources.ImagenBotonHistorial1;
            this.pbHistorial.Location = new System.Drawing.Point(245, 145);
            this.pbHistorial.Name = "pbHistorial";
            this.pbHistorial.Size = new System.Drawing.Size(293, 85);
            this.pbHistorial.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbHistorial.TabIndex = 1;
            this.pbHistorial.TabStop = false;
            this.pbHistorial.Click += new System.EventHandler(this.pbHistorial_Click);
            // 
            // pbNuevaPartida
            // 
            this.pbNuevaPartida.BackColor = System.Drawing.Color.Transparent;
            this.pbNuevaPartida.Image = global::UnoGame.Properties.Resources.ImagenBotonNuevaPartida1;
            this.pbNuevaPartida.Location = new System.Drawing.Point(206, 22);
            this.pbNuevaPartida.Name = "pbNuevaPartida";
            this.pbNuevaPartida.Size = new System.Drawing.Size(388, 106);
            this.pbNuevaPartida.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbNuevaPartida.TabIndex = 0;
            this.pbNuevaPartida.TabStop = false;
            this.pbNuevaPartida.Click += new System.EventHandler(this.pbNuevaPartida_Click);
            // 
            // FormMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UnoGame.Properties.Resources.fondomenu;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(837, 466);
            this.Controls.Add(this.pbSalir);
            this.Controls.Add(this.pbHistorial);
            this.Controls.Add(this.pbNuevaPartida);
            this.Name = "FormMenu";
            this.Text = "Menú Principal";
            ((System.ComponentModel.ISupportInitialize)(this.pbSalir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHistorial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNuevaPartida)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbNuevaPartida;
        private System.Windows.Forms.PictureBox pbHistorial;
        private System.Windows.Forms.PictureBox pbSalir;
    }
}