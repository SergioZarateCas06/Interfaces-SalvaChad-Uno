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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
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
            this.panelBurra.SuspendLayout();
            this.panelDescarte.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(24, 479);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(152, 38);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(111, 523);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.botonRepartir);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(24, 523);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(81, 36);
            this.button2.TabIndex = 4;
            this.button2.Text = "jugar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel_rival_izquierda
            // 
            this.panel_rival_izquierda.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_rival_izquierda.Location = new System.Drawing.Point(20, 250);
            this.panel_rival_izquierda.Name = "panel_rival_izquierda";
            this.panel_rival_izquierda.Size = new System.Drawing.Size(150, 200);
            this.panel_rival_izquierda.TabIndex = 6;
            // 
            // panel_rival_arriba
            // 
            this.panel_rival_arriba.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_rival_arriba.Location = new System.Drawing.Point(420, 20);
            this.panel_rival_arriba.Name = "panel_rival_arriba";
            this.panel_rival_arriba.Size = new System.Drawing.Size(400, 100);
            this.panel_rival_arriba.TabIndex = 7;
            // 
            // panel_rival_derecha
            // 
            this.panel_rival_derecha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_rival_derecha.Location = new System.Drawing.Point(1060, 250);
            this.panel_rival_derecha.Name = "panel_rival_derecha";
            this.panel_rival_derecha.Size = new System.Drawing.Size(150, 200);
            this.panel_rival_derecha.TabIndex = 8;
            // 
            // panelBurra
            // 
            this.panelBurra.Controls.Add(this.labelBurra);
            this.panelBurra.Location = new System.Drawing.Point(520, 260);
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
            this.panelDescarte.Location = new System.Drawing.Point(650, 260);
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
            this.panel_mano_jugador1.Name = "panel_mano_jugador1";
            this.panel_mano_jugador1.Size = new System.Drawing.Size(699, 210);
            this.panel_mano_jugador1.TabIndex = 12;
            // 
            // panelColor
            // 
            this.panelColor.Location = new System.Drawing.Point(756, 260);
            this.panelColor.Name = "panelColor";
            this.panelColor.Size = new System.Drawing.Size(40, 40);
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
            this.labelSentido.Location = new System.Drawing.Point(21, 61);
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
            this.label_jugador3.Location = new System.Drawing.Point(570, 123);
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
            this.label_jugador4.Location = new System.Drawing.Point(938, 346);
            this.label_jugador4.Name = "label_jugador4";
            this.label_jugador4.Size = new System.Drawing.Size(116, 16);
            this.label_jugador4.TabIndex = 20;
            this.label_jugador4.Text = "jugador 4: 7 cartas";
            this.label_jugador4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = global::UnoGame.Properties.Resources.fondo_juego_1;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1232, 703);
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
            this.Controls.Add(this.panelDescarte);
            this.Controls.Add(this.panelBurra);
            this.Controls.Add(this.panel_rival_derecha);
            this.Controls.Add(this.panel_rival_arriba);
            this.Controls.Add(this.panel_rival_izquierda);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox1);
            this.MinimumSize = new System.Drawing.Size(1000, 650);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelBurra.ResumeLayout(false);
            this.panelDescarte.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
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
    }
}

