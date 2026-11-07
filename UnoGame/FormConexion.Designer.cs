namespace UnoGame
{
    partial class FormConexion
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
            this.panelFormulario = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblIp = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.lblPuerto = new System.Windows.Forms.Label();
            this.txtPuerto = new System.Windows.Forms.TextBox();
            this.btnConectar = new System.Windows.Forms.PictureBox();
            this.btnVolver = new System.Windows.Forms.PictureBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.panelFormulario.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnConectar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnVolver)).BeginInit();
            this.SuspendLayout();
            // 
            // panelFormulario
            // 
            this.panelFormulario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.panelFormulario.Controls.Add(this.lblTitulo);
            this.panelFormulario.Controls.Add(this.lblNombre);
            this.panelFormulario.Controls.Add(this.txtNombre);
            this.panelFormulario.Controls.Add(this.lblIp);
            this.panelFormulario.Controls.Add(this.txtIp);
            this.panelFormulario.Controls.Add(this.lblPuerto);
            this.panelFormulario.Controls.Add(this.txtPuerto);
            this.panelFormulario.Controls.Add(this.btnConectar);
            this.panelFormulario.Controls.Add(this.btnVolver);
            this.panelFormulario.Controls.Add(this.lblEstado);
            this.panelFormulario.Location = new System.Drawing.Point(188, 24);
            this.panelFormulario.Name = "panelFormulario";
            this.panelFormulario.Size = new System.Drawing.Size(461, 380);
            this.panelFormulario.TabIndex = 0;
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
            this.lblTitulo.Text = "Conectar al servidor";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.BackColor = System.Drawing.Color.Transparent;
            this.lblNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblNombre.ForeColor = System.Drawing.Color.White;
            this.lblNombre.Location = new System.Drawing.Point(60, 70);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(98, 20);
            this.lblNombre.TabIndex = 1;
            this.lblNombre.Text = "Tu nombre";
            // 
            // txtNombre
            // 
            this.txtNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtNombre.Location = new System.Drawing.Point(60, 92);
            this.txtNombre.MaxLength = 30;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(340, 30);
            this.txtNombre.TabIndex = 1;
            // 
            // lblIp
            // 
            this.lblIp.AutoSize = true;
            this.lblIp.BackColor = System.Drawing.Color.Transparent;
            this.lblIp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblIp.ForeColor = System.Drawing.Color.White;
            this.lblIp.Location = new System.Drawing.Point(60, 134);
            this.lblIp.Name = "lblIp";
            this.lblIp.Size = new System.Drawing.Size(131, 20);
            this.lblIp.TabIndex = 3;
            this.lblIp.Text = "IP del servidor";
            // 
            // txtIp
            // 
            this.txtIp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtIp.Location = new System.Drawing.Point(60, 156);
            this.txtIp.MaxLength = 15;
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(340, 30);
            this.txtIp.TabIndex = 2;
            // 
            // lblPuerto
            // 
            this.lblPuerto.AutoSize = true;
            this.lblPuerto.BackColor = System.Drawing.Color.Transparent;
            this.lblPuerto.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblPuerto.ForeColor = System.Drawing.Color.White;
            this.lblPuerto.Location = new System.Drawing.Point(60, 198);
            this.lblPuerto.Name = "lblPuerto";
            this.lblPuerto.Size = new System.Drawing.Size(64, 20);
            this.lblPuerto.TabIndex = 5;
            this.lblPuerto.Text = "Puerto";
            // 
            // txtPuerto
            // 
            this.txtPuerto.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtPuerto.Location = new System.Drawing.Point(60, 220);
            this.txtPuerto.MaxLength = 5;
            this.txtPuerto.Name = "txtPuerto";
            this.txtPuerto.Size = new System.Drawing.Size(120, 30);
            this.txtPuerto.TabIndex = 3;
            // 
            // btnConectar
            // 
            this.btnConectar.BackColor = System.Drawing.Color.Transparent;
            this.btnConectar.BackgroundImage = global::UnoGame.Properties.Resources.ImagenBotonConectar;
            this.btnConectar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnConectar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConectar.Location = new System.Drawing.Point(143, 256);
            this.btnConectar.Name = "btnConectar";
            this.btnConectar.Size = new System.Drawing.Size(174, 57);
            this.btnConectar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnConectar.TabIndex = 4;
            this.btnConectar.TabStop = false;
            this.btnConectar.Click += new System.EventHandler(this.btnConectar_Click);
            // 
            // btnVolver
            // 
            this.btnVolver.BackColor = System.Drawing.Color.Transparent;
            this.btnVolver.BackgroundImage = global::UnoGame.Properties.Resources.ImagenBotonVolver;
            this.btnVolver.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.Location = new System.Drawing.Point(6, 319);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(107, 49);
            this.btnVolver.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnVolver.TabIndex = 5;
            this.btnVolver.TabStop = false;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // lblEstado
            // 
            this.lblEstado.BackColor = System.Drawing.Color.Transparent;
            this.lblEstado.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Italic);
            this.lblEstado.ForeColor = System.Drawing.Color.Gold;
            this.lblEstado.Location = new System.Drawing.Point(100, 336);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(260, 24);
            this.lblEstado.TabIndex = 9;
            this.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormConexion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(837, 428);
            this.Controls.Add(this.panelFormulario);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "FormConexion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Conectar al servidor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormConexion_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormConexion_KeyDown);
            this.panelFormulario.ResumeLayout(false);
            this.panelFormulario.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnConectar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnVolver)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelFormulario;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblIp;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label lblPuerto;
        private System.Windows.Forms.TextBox txtPuerto;
        private System.Windows.Forms.PictureBox btnConectar;
        private System.Windows.Forms.PictureBox btnVolver;
        private System.Windows.Forms.Label lblEstado;
    }
}