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
            this.button3 = new System.Windows.Forms.Button();
            this.panelBurra.SuspendLayout();
            this.panelDescarte.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_rival_izquierda
            // 
            this.panel_rival_izquierda.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_rival_izquierda.Location = new System.Drawing.Point(20, 250);
            this.panel_rival_izquierda.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_rival_izquierda.Name = "panel_rival_izquierda";
            this.panel_rival_izquierda.Size = new System.Drawing.Size(150, 200);
            this.panel_rival_izquierda.TabIndex = 6;
            // 
            // panel_rival_arriba
            // 
            this.panel_rival_arriba.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_rival_arriba.Location = new System.Drawing.Point(420, 20);
            this.panel_rival_arriba.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_rival_arriba.Name = "panel_rival_arriba";
            this.panel_rival_arriba.Size = new System.Drawing.Size(399, 100);
            this.panel_rival_arriba.TabIndex = 7;
            // 
            // panel_rival_derecha
            // 
            this.panel_rival_derecha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_rival_derecha.Location = new System.Drawing.Point(1060, 250);
            this.panel_rival_derecha.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_rival_derecha.Name = "panel_rival_derecha";
            this.panel_rival_derecha.Size = new System.Drawing.Size(150, 200);
            this.panel_rival_derecha.TabIndex = 8;
            // 
            // panelBurra
            // 
            this.panelBurra.Controls.Add(this.labelBurra);
            this.panelBurra.Location = new System.Drawing.Point(520, 260);
            this.panelBurra.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelBurra.Name = "panelBurra";
            this.panelBurra.Size = new System.Drawing.Size(100, 150);
            this.panelBurra.TabIndex = 9;
            // 
            // labelBurra
            // 
            this.labelBurra.BackColor = System.Drawing.Color.Transparent;
            this.labelBurra.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelBurra.Location = new System.Drawing.Point(0, 0);
            this.labelBurra.Name = "labelBurra";
            this.labelBurra.Size = new System.Drawing.Size(100, 16);
            this.labelBurra.TabIndex = 11;
            this.labelBurra.Text = "Burra";
            this.labelBurra.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelBurra.Click += new System.EventHandler(this.labelBurra_Click);
            // 
            // panelDescarte
            // 
            this.panelDescarte.Controls.Add(this.labelDescarte);
            this.panelDescarte.Location = new System.Drawing.Point(651, 260);
            this.panelDescarte.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelDescarte.Name = "panelDescarte";
            this.panelDescarte.Size = new System.Drawing.Size(100, 150);
            this.panelDescarte.TabIndex = 10;
            // 
            // labelDescarte
            // 
            this.labelDescarte.BackColor = System.Drawing.Color.Transparent;
            this.labelDescarte.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelDescarte.Location = new System.Drawing.Point(0, 0);
            this.labelDescarte.Name = "labelDescarte";
            this.labelDescarte.Size = new System.Drawing.Size(100, 16);
            this.labelDescarte.TabIndex = 0;
            this.labelDescarte.Text = "Descarte";
            this.labelDescarte.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTurno
            // 
            this.labelTurno.AutoSize = true;
            this.labelTurno.BackColor = System.Drawing.Color.White;
            this.labelTurno.Location = new System.Drawing.Point(21, 32);
            this.labelTurno.Name = "labelTurno";
            this.labelTurno.Size = new System.Drawing.Size(120, 16);
            this.labelTurno.TabIndex = 0;
            this.labelTurno.Text = "Turno de jugador 1";
            // 
            // label_Mano_jugador
            // 
            this.label_Mano_jugador.AutoSize = true;
            this.label_Mano_jugador.BackColor = System.Drawing.Color.White;
            this.label_Mano_jugador.Location = new System.Drawing.Point(325, 462);
            this.label_Mano_jugador.Name = "label_Mano_jugador";
            this.label_Mano_jugador.Size = new System.Drawing.Size(60, 16);
            this.label_Mano_jugador.TabIndex = 11;
            this.label_Mano_jugador.Text = "Tu mano";
            // 
            // panel_mano_jugador1
            // 
            this.panel_mano_jugador1.Location = new System.Drawing.Point(308, 481);
            this.panel_mano_jugador1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_mano_jugador1.Name = "panel_mano_jugador1";
            this.panel_mano_jugador1.Size = new System.Drawing.Size(699, 210);
            this.panel_mano_jugador1.TabIndex = 12;
            // 
            // panelColor
            // 
            this.panelColor.Location = new System.Drawing.Point(756, 260);
            this.panelColor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelColor.Name = "panelColor";
            this.panelColor.Size = new System.Drawing.Size(40, 39);
            this.panelColor.TabIndex = 13;
            // 
            // labelColor
            // 
            this.labelColor.AutoSize = true;
            this.labelColor.Location = new System.Drawing.Point(756, 238);
            this.labelColor.Name = "labelColor";
            this.labelColor.Size = new System.Drawing.Size(37, 16);
            this.labelColor.TabIndex = 14;
            this.labelColor.Text = "color";
            // 
            // labelSentido
            // 
            this.labelSentido.AutoSize = true;
            this.labelSentido.Location = new System.Drawing.Point(21, 62);
            this.labelSentido.Name = "labelSentido";
            this.labelSentido.Size = new System.Drawing.Size(57, 16);
            this.labelSentido.TabIndex = 15;
            this.labelSentido.Text = "sentido: ";
            // 
            // label_jugador1
            // 
            this.label_jugador1.AutoSize = true;
            this.label_jugador1.Location = new System.Drawing.Point(573, 450);
            this.label_jugador1.Name = "label_jugador1";
            this.label_jugador1.Size = new System.Drawing.Size(113, 16);
            this.label_jugador1.TabIndex = 17;
            this.label_jugador1.Text = "jugador1: 7 cartas";
            // 
            // label_jugador2
            // 
            this.label_jugador2.AutoSize = true;
            this.label_jugador2.BackColor = System.Drawing.Color.White;
            this.label_jugador2.Location = new System.Drawing.Point(176, 334);
            this.label_jugador2.Name = "label_jugador2";
            this.label_jugador2.Size = new System.Drawing.Size(116, 16);
            this.label_jugador2.TabIndex = 18;
            this.label_jugador2.Text = "jugador 2: 7 cartas";
            this.label_jugador2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_jugador3
            // 
            this.label_jugador3.AutoSize = true;
            this.label_jugador3.BackColor = System.Drawing.Color.White;
            this.label_jugador3.Location = new System.Drawing.Point(571, 123);
            this.label_jugador3.Name = "label_jugador3";
            this.label_jugador3.Size = new System.Drawing.Size(116, 16);
            this.label_jugador3.TabIndex = 19;
            this.label_jugador3.Text = "jugador 3: 7 cartas";
            this.label_jugador3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_jugador4
            // 
            this.label_jugador4.AutoSize = true;
            this.label_jugador4.BackColor = System.Drawing.Color.White;
            this.label_jugador4.Location = new System.Drawing.Point(939, 346);
            this.label_jugador4.Name = "label_jugador4";
            this.label_jugador4.Size = new System.Drawing.Size(116, 16);
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
            this.SeleccionColor_Rojo.Location = new System.Drawing.Point(431, 172);
            this.SeleccionColor_Rojo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SeleccionColor_Rojo.Name = "SeleccionColor_Rojo";
            this.SeleccionColor_Rojo.Size = new System.Drawing.Size(191, 127);
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
            this.SeleccionColor_Azul.Location = new System.Drawing.Point(429, 306);
            this.SeleccionColor_Azul.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SeleccionColor_Azul.Name = "SeleccionColor_Azul";
            this.SeleccionColor_Azul.Size = new System.Drawing.Size(191, 127);
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
            this.SeleccionColor_Amarillo.Location = new System.Drawing.Point(629, 171);
            this.SeleccionColor_Amarillo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SeleccionColor_Amarillo.Name = "SeleccionColor_Amarillo";
            this.SeleccionColor_Amarillo.Size = new System.Drawing.Size(191, 127);
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
            this.SeleccionColor_Verde.Location = new System.Drawing.Point(629, 305);
            this.SeleccionColor_Verde.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SeleccionColor_Verde.Name = "SeleccionColor_Verde";
            this.SeleccionColor_Verde.Size = new System.Drawing.Size(191, 127);
            this.SeleccionColor_Verde.TabIndex = 24;
            this.SeleccionColor_Verde.Text = " ";
            this.SeleccionColor_Verde.UseVisualStyleBackColor = false;
            this.SeleccionColor_Verde.Visible = false;
            this.SeleccionColor_Verde.Click += new System.EventHandler(this.SeleccionColor_Verde_Click);
            this.SeleccionColor_Verde.MouseEnter += new System.EventHandler(this.SeleccionColor_Verde_MouseEnter);
            this.SeleccionColor_Verde.MouseLeave += new System.EventHandler(this.SeleccionColor_Verde_MouseLeave);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(281, 392);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(121, 58);
            this.button3.TabIndex = 25;
            this.button3.Text = "UNO";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = global::UnoGame.Properties.Resources.fondo_juego_1;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1232, 703);
            this.Controls.Add(this.button3);
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
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(999, 648);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Click += new System.EventHandler(this.Form1_Click);
            this.panelBurra.ResumeLayout(false);
            this.panelDescarte.ResumeLayout(false);
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
        private System.Windows.Forms.Button button3;
    }
}

