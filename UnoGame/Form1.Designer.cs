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
            this.Avanzar_turno = new System.Windows.Forms.Button();
            this.Invertir_sentido = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(46, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(488, 266);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(46, 307);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.botonRepartir);
            // 
            // Avanzar_turno
            // 
            this.Avanzar_turno.Location = new System.Drawing.Point(617, 21);
            this.Avanzar_turno.Name = "Avanzar_turno";
            this.Avanzar_turno.Size = new System.Drawing.Size(130, 40);
            this.Avanzar_turno.TabIndex = 2;
            this.Avanzar_turno.Text = "Avanzar_turno";
            this.Avanzar_turno.UseVisualStyleBackColor = true;
            this.Avanzar_turno.Click += new System.EventHandler(this.Avanzar_turno_Click);
            // 
            // Invertir_sentido
            // 
            this.Invertir_sentido.Location = new System.Drawing.Point(620, 95);
            this.Invertir_sentido.Name = "Invertir_sentido";
            this.Invertir_sentido.Size = new System.Drawing.Size(127, 45);
            this.Invertir_sentido.TabIndex = 3;
            this.Invertir_sentido.Text = "Invertir_sentido";
            this.Invertir_sentido.UseVisualStyleBackColor = true;
            this.Invertir_sentido.Click += new System.EventHandler(this.Invertir_turno_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Invertir_sentido);
            this.Controls.Add(this.Avanzar_turno);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Avanzar_turno;
        private System.Windows.Forms.Button Invertir_sentido;
    }
}

