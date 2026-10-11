namespace UnoGame
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel_rival_izquierda = new System.Windows.Forms.Panel();
            this.panel_rival_arriba = new System.Windows.Forms.Panel();
            this.panel_rival_derecha = new System.Windows.Forms.Panel();
            this.panelBurra = new System.Windows.Forms.Panel();
            this.labelBurra = new System.Windows.Forms.Label();
            this.panelDescarte = new System.Windows.Forms.Panel();
            this.labelDescarte = new System.Windows.Forms.Label();
            this.labelTurno = new System.Windows.Forms.Label();
            this.label_Mano_jugador = new System.Windows.Forms.Label();
            this.panel_mano_jugador1 = new System.Windows.Forms.Panel();
            this.panelColor = new System.Windows.Forms.Panel();
            this.labelColor = new System.Windows.Forms.Label();
            this.labelSentido = new System.Windows.Forms.Label();
            this.label_jugador1 = new System.Windows.Forms.Label();
            this.label_jugador2 = new System.Windows.Forms.Label();
            this.label_jugador3 = new System.Windows.Forms.Label();
            this.label_jugador4 = new System.Windows.Forms.Label();
            this.SeleccionColor_Rojo = new System.Windows.Forms.Button();
            this.SeleccionColor_Azul = new System.Windows.Forms.Button();
            this.SeleccionColor_Amarillo = new System.Windows.Forms.Button();
            this.SeleccionColor_Verde = new System.Windows.Forms.Button();
            this.buttonSilencio = new System.Windows.Forms.Button();
            this.botonUno = new System.Windows.Forms.PictureBox();
            this.panelBurra.SuspendLayout();
            this.panelDescarte.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.botonUno)).BeginInit();
            this.SuspendLayout();
            // 
            // panel_rival_izquierda
            // 
            this.panel_rival_izquierda.BackColor = System.Drawing.Color.Transparent;
            this.panel_rival_izquierda.Location = new System.Drawing.Point(9, 154);
            this.panel_rival_izquierda.Margin = new System.Windows.Forms.Padding(2);
            this.panel_rival_izquierda.Name = "panel_rival_izquierda";
            this.panel_rival_izquierda.Size = new System.Drawing.Size(112, 211);
            this.panel_rival_izquierda.TabIndex = 6;
            // 
            // panel_rival_arriba
            // 
            this.panel_rival_arriba.BackColor = System.Drawing.Color.Transparent;
            this.panel_rival_arriba.Location = new System.Drawing.Point(272, 16);
            this.panel_rival_arriba.Margin = new System.Windows.Forms.Padding(2);
            this.panel_rival_arriba.Name = "panel_rival_arriba";
            this.panel_rival_arriba.Size = new System.Drawing.Size(418, 81);
            this.panel_rival_arriba.TabIndex = 7;
            // 
            // panel_rival_derecha
            // 
            this.panel_rival_derecha.BackColor = System.Drawing.Color.Transparent;
            this.panel_rival_derecha.Location = new System.Drawing.Point(802, 154);
            this.panel_rival_derecha.Margin = new System.Windows.Forms.Padding(2);
            this.panel_rival_derecha.Name = "panel_rival_derecha";
            this.panel_rival_derecha.Size = new System.Drawing.Size(112, 211);
            this.panel_rival_derecha.TabIndex = 8;
            // 
            // panelBurra
            // 
            this.panelBurra.BackColor = System.Drawing.Color.Transparent;
            this.panelBurra.Controls.Add(this.labelBurra);
            this.panelBurra.Location = new System.Drawing.Point(390, 211);
            this.panelBurra.Margin = new System.Windows.Forms.Padding(2);
            this.panelBurra.Name = "panelBurra";
            this.panelBurra.Size = new System.Drawing.Size(75, 122);
            this.panelBurra.TabIndex = 9;
            // 
            // labelBurra
            // 
            this.labelBurra.BackColor = System.Drawing.Color.Transparent;
            this.labelBurra.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelBurra.Location = new System.Drawing.Point(0, 0);
            this.labelBurra.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelBurra.Name = "labelBurra";
            this.labelBurra.Size = new System.Drawing.Size(75, 13);
            this.labelBurra.TabIndex = 11;
            this.labelBurra.Text = "Burra";
            this.labelBurra.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelDescarte
            // 
            this.panelDescarte.BackColor = System.Drawing.Color.Transparent;
            this.panelDescarte.Controls.Add(this.labelDescarte);
            this.panelDescarte.Location = new System.Drawing.Point(488, 211);
            this.panelDescarte.Margin = new System.Windows.Forms.Padding(2);
            this.panelDescarte.Name = "panelDescarte";
            this.panelDescarte.Size = new System.Drawing.Size(75, 122);
            this.panelDescarte.TabIndex = 10;
            // 
            // labelDescarte
            // 
            this.labelDescarte.BackColor = System.Drawing.Color.Transparent;
            this.labelDescarte.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelDescarte.Location = new System.Drawing.Point(0, 0);
            this.labelDescarte.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelDescarte.Name = "labelDescarte";
            this.labelDescarte.Size = new System.Drawing.Size(75, 13);
            this.labelDescarte.TabIndex = 0;
            this.labelDescarte.Text = "Descarte";
            this.labelDescarte.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTurno
            // 
            this.labelTurno.AutoSize = true;
            this.labelTurno.BackColor = System.Drawing.Color.White;
            this.labelTurno.Location = new System.Drawing.Point(6, 4);
            this.labelTurno.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTurno.Name = "labelTurno";
            this.labelTurno.Size = new System.Drawing.Size(97, 13);
            this.labelTurno.TabIndex = 0;
            this.labelTurno.Text = "Turno de jugador 1";
            // 
            // label_Mano_jugador
            // 
            this.label_Mano_jugador.AutoSize = true;
            this.label_Mano_jugador.BackColor = System.Drawing.Color.White;
            this.label_Mano_jugador.Location = new System.Drawing.Point(195, 376);
            this.label_Mano_jugador.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Mano_jugador.Name = "label_Mano_jugador";
            this.label_Mano_jugador.Size = new System.Drawing.Size(49, 13);
            this.label_Mano_jugador.TabIndex = 11;
            this.label_Mano_jugador.Text = "Tu mano";
            // 
            // panel_mano_jugador1
            // 
            this.panel_mano_jugador1.BackColor = System.Drawing.Color.Transparent;
            this.panel_mano_jugador1.Location = new System.Drawing.Point(197, 391);
            this.panel_mano_jugador1.Margin = new System.Windows.Forms.Padding(2);
            this.panel_mano_jugador1.Name = "panel_mano_jugador1";
            this.panel_mano_jugador1.Size = new System.Drawing.Size(551, 171);
            this.panel_mano_jugador1.TabIndex = 12;
            // 
            // panelColor
            // 
            this.panelColor.Location = new System.Drawing.Point(567, 211);
            this.panelColor.Margin = new System.Windows.Forms.Padding(2);
            this.panelColor.Name = "panelColor";
            this.panelColor.Size = new System.Drawing.Size(30, 32);
            this.panelColor.TabIndex = 13;
            // 
            // labelColor
            // 
            this.labelColor.AutoSize = true;
            this.labelColor.Location = new System.Drawing.Point(567, 193);
            this.labelColor.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelColor.Name = "labelColor";
            this.labelColor.Size = new System.Drawing.Size(30, 13);
            this.labelColor.TabIndex = 14;
            this.labelColor.Text = "color";
            // 
            // labelSentido
            // 
            this.labelSentido.AutoSize = true;
            this.labelSentido.Location = new System.Drawing.Point(6, 28);
            this.labelSentido.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSentido.Name = "labelSentido";
            this.labelSentido.Size = new System.Drawing.Size(47, 13);
            this.labelSentido.TabIndex = 15;
            this.labelSentido.Text = "sentido: ";
            // 
            // label_jugador1
            // 
            this.label_jugador1.AutoSize = true;
            this.label_jugador1.Location = new System.Drawing.Point(430, 366);
            this.label_jugador1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_jugador1.Name = "label_jugador1";
            this.label_jugador1.Size = new System.Drawing.Size(92, 13);
            this.label_jugador1.TabIndex = 17;
            this.label_jugador1.Text = "jugador1: 7 cartas";
            // 
            // label_jugador2
            // 
            this.label_jugador2.AutoSize = true;
            this.label_jugador2.BackColor = System.Drawing.Color.White;
            this.label_jugador2.Location = new System.Drawing.Point(9, 131);
            this.label_jugador2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_jugador2.Name = "label_jugador2";
            this.label_jugador2.Size = new System.Drawing.Size(95, 13);
            this.label_jugador2.TabIndex = 18;
            this.label_jugador2.Text = "jugador 2: 7 cartas";
            this.label_jugador2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_jugador3
            // 
            this.label_jugador3.AutoSize = true;
            this.label_jugador3.BackColor = System.Drawing.Color.White;
            this.label_jugador3.Location = new System.Drawing.Point(428, 100);
            this.label_jugador3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_jugador3.Name = "label_jugador3";
            this.label_jugador3.Size = new System.Drawing.Size(95, 13);
            this.label_jugador3.TabIndex = 19;
            this.label_jugador3.Text = "jugador 3: 7 cartas";
            this.label_jugador3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_jugador4
            // 
            this.label_jugador4.AutoSize = true;
            this.label_jugador4.BackColor = System.Drawing.Color.White;
            this.label_jugador4.Location = new System.Drawing.Point(828, 131);
            this.label_jugador4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_jugador4.Name = "label_jugador4";
            this.label_jugador4.Size = new System.Drawing.Size(95, 13);
            this.label_jugador4.TabIndex = 20;
            this.label_jugador4.Text = "jugador 4: 7 cartas";
            this.label_jugador4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SeleccionColor_Rojo
            // 
            this.SeleccionColor_Rojo.BackColor = System.Drawing.Color.Red;
            this.SeleccionColor_Rojo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SeleccionColor_Rojo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SeleccionColor_Rojo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SeleccionColor_Rojo.Location = new System.Drawing.Point(323, 140);
            this.SeleccionColor_Rojo.Name = "SeleccionColor_Rojo";
            this.SeleccionColor_Rojo.Size = new System.Drawing.Size(143, 103);
            this.SeleccionColor_Rojo.TabIndex = 21;
            this.SeleccionColor_Rojo.Text = " ";
            this.SeleccionColor_Rojo.UseVisualStyleBackColor = false;
            this.SeleccionColor_Rojo.Visible = false;
            this.SeleccionColor_Rojo.Click += new System.EventHandler(this.SeleccionColor_Rojo_Click);
            this.SeleccionColor_Rojo.MouseEnter += new System.EventHandler(this.SeleccionColor_Rojo_MouseEnter);
            this.SeleccionColor_Rojo.MouseLeave += new System.EventHandler(this.SeleccionColor_Rojo_MouseLeave);
            // 
            // SeleccionColor_Azul
            // 
            this.SeleccionColor_Azul.BackColor = System.Drawing.Color.Blue;
            this.SeleccionColor_Azul.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.SeleccionColor_Azul.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SeleccionColor_Azul.Location = new System.Drawing.Point(322, 249);
            this.SeleccionColor_Azul.Name = "SeleccionColor_Azul";
            this.SeleccionColor_Azul.Size = new System.Drawing.Size(143, 103);
            this.SeleccionColor_Azul.TabIndex = 22;
            this.SeleccionColor_Azul.Text = " ";
            this.SeleccionColor_Azul.UseVisualStyleBackColor = false;
            this.SeleccionColor_Azul.Visible = false;
            this.SeleccionColor_Azul.Click += new System.EventHandler(this.SeleccionColor_Azul_Click);
            this.SeleccionColor_Azul.MouseEnter += new System.EventHandler(this.SeleccionColor_Azul_MouseEnter);
            this.SeleccionColor_Azul.MouseLeave += new System.EventHandler(this.SeleccionColor_Azul_MouseLeave);
            // 
            // SeleccionColor_Amarillo
            // 
            this.SeleccionColor_Amarillo.BackColor = System.Drawing.Color.Yellow;
            this.SeleccionColor_Amarillo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SeleccionColor_Amarillo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.SeleccionColor_Amarillo.FlatAppearance.BorderSize = 2;
            this.SeleccionColor_Amarillo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SeleccionColor_Amarillo.Location = new System.Drawing.Point(472, 139);
            this.SeleccionColor_Amarillo.Name = "SeleccionColor_Amarillo";
            this.SeleccionColor_Amarillo.Size = new System.Drawing.Size(143, 103);
            this.SeleccionColor_Amarillo.TabIndex = 23;
            this.SeleccionColor_Amarillo.Text = " ";
            this.SeleccionColor_Amarillo.UseVisualStyleBackColor = false;
            this.SeleccionColor_Amarillo.Visible = false;
            this.SeleccionColor_Amarillo.Click += new System.EventHandler(this.SeleccionColor_Amarillo_Click);
            this.SeleccionColor_Amarillo.MouseEnter += new System.EventHandler(this.SeleccionColor_Amarillo_MouseEnter);
            this.SeleccionColor_Amarillo.MouseLeave += new System.EventHandler(this.SeleccionColor_Amarillo_MouseLeave);
            // 
            // SeleccionColor_Verde
            // 
            this.SeleccionColor_Verde.BackColor = System.Drawing.Color.Green;
            this.SeleccionColor_Verde.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SeleccionColor_Verde.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.SeleccionColor_Verde.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SeleccionColor_Verde.Location = new System.Drawing.Point(472, 248);
            this.SeleccionColor_Verde.Name = "SeleccionColor_Verde";
            this.SeleccionColor_Verde.Size = new System.Drawing.Size(143, 103);
            this.SeleccionColor_Verde.TabIndex = 24;
            this.SeleccionColor_Verde.Text = " ";
            this.SeleccionColor_Verde.UseVisualStyleBackColor = false;
            this.SeleccionColor_Verde.Visible = false;
            this.SeleccionColor_Verde.Click += new System.EventHandler(this.SeleccionColor_Verde_Click);
            this.SeleccionColor_Verde.MouseEnter += new System.EventHandler(this.SeleccionColor_Verde_MouseEnter);
            this.SeleccionColor_Verde.MouseLeave += new System.EventHandler(this.SeleccionColor_Verde_MouseLeave);
            // 
            // buttonSilencio
            // 
            this.buttonSilencio.Location = new System.Drawing.Point(848, -1);
            this.buttonSilencio.Name = "buttonSilencio";
            this.buttonSilencio.Size = new System.Drawing.Size(75, 23);
            this.buttonSilencio.TabIndex = 26;
            this.buttonSilencio.Text = "🔊 Sonido: ON";
            this.buttonSilencio.UseVisualStyleBackColor = true;
            // 
            // botonUno
            // 
            this.botonUno.BackColor = System.Drawing.Color.Transparent;
            this.botonUno.Image = global::UnoGame.Properties.Resources.ImagenBotonUno1;
            this.botonUno.Location = new System.Drawing.Point(177, 310);
            this.botonUno.Margin = new System.Windows.Forms.Padding(2);
            this.botonUno.Name = "botonUno";
            this.botonUno.Size = new System.Drawing.Size(140, 55);
            this.botonUno.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.botonUno.TabIndex = 27;
            this.botonUno.TabStop = false;
            this.botonUno.Visible = false;
            this.botonUno.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = global::UnoGame.Properties.Resources.fondo_juego_1;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(924, 571);
            this.Controls.Add(this.botonUno);
            this.Controls.Add(this.buttonSilencio);
            this.Controls.Add(this.SeleccionColor_Verde);
            this.Controls.Add(this.SeleccionColor_Amarillo);
            this.Controls.Add(this.SeleccionColor_Azul);
            this.Controls.Add(this.SeleccionColor_Rojo);
            this.Controls.Add(this.label_jugador4);
            this.Controls.Add(this.label_jugador3);
            this.Controls.Add(this.label_jugador2);
            this.Controls.Add(this.label_jugador1);
            this.Controls.Add(this.labelSentido);
            this.Controls.Add(this.labelColor);
            this.Controls.Add(this.panelColor);
            this.Controls.Add(this.panel_mano_jugador1);
            this.Controls.Add(this.label_Mano_jugador);
            this.Controls.Add(this.labelTurno);
            this.Controls.Add(this.panel_rival_derecha);
            this.Controls.Add(this.panel_rival_arriba);
            this.Controls.Add(this.panel_rival_izquierda);
            this.Controls.Add(this.panelDescarte);
            this.Controls.Add(this.panelBurra);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(752, 532);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Juego UNO";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Click += new System.EventHandler(this.Form1_Click);
            this.panelBurra.ResumeLayout(false);
            this.panelDescarte.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.botonUno)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel_rival_izquierda;
        private System.Windows.Forms.Panel panel_rival_arriba;
        private System.Windows.Forms.Panel panel_rival_derecha;
        private System.Windows.Forms.Panel panelBurra;
        private System.Windows.Forms.Panel panelDescarte;
        private System.Windows.Forms.Label labelTurno;
        private System.Windows.Forms.Label labelBurra;
        private System.Windows.Forms.Label labelDescarte;
        private System.Windows.Forms.Label label_Mano_jugador;
        private System.Windows.Forms.Panel panel_mano_jugador1;
        private System.Windows.Forms.Panel panelColor;
        private System.Windows.Forms.Label labelColor;
        private System.Windows.Forms.Label labelSentido;
        private System.Windows.Forms.Label label_jugador1;
        private System.Windows.Forms.Label label_jugador2;
        private System.Windows.Forms.Label label_jugador3;
        private System.Windows.Forms.Label label_jugador4;
        private System.Windows.Forms.Button SeleccionColor_Rojo;
        private System.Windows.Forms.Button SeleccionColor_Azul;
        private System.Windows.Forms.Button SeleccionColor_Amarillo;
        private System.Windows.Forms.Button SeleccionColor_Verde;
        private System.Windows.Forms.Button buttonSilencio;
        private System.Windows.Forms.PictureBox botonUno;
    }
}

