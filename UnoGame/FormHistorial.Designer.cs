namespace UnoGame
{
    partial class FormHistorial
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
            this.Estadisticas = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvEstadisticas = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.labelEstadoHistorial = new System.Windows.Forms.Label();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.comboPartidas = new System.Windows.Forms.ComboBox();
            this.dgvMovimientos = new System.Windows.Forms.DataGridView();
            this.Estadisticas.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEstadisticas)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).BeginInit();
            this.SuspendLayout();
            // 
            // Estadisticas
            // 
            this.Estadisticas.Controls.Add(this.tabPage1);
            this.Estadisticas.Controls.Add(this.tabPage2);
            this.Estadisticas.Controls.Add(this.tabPage3);
            this.Estadisticas.Location = new System.Drawing.Point(12, 12);
            this.Estadisticas.Name = "Estadisticas";
            this.Estadisticas.SelectedIndex = 0;
            this.Estadisticas.Size = new System.Drawing.Size(733, 436);
            this.Estadisticas.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.dgvEstadisticas);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(725, 407);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Estadisticas";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(321, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 16);
            this.label1.TabIndex = 1;
            // 
            // dgvEstadisticas
            // 
            this.dgvEstadisticas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEstadisticas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEstadisticas.Location = new System.Drawing.Point(3, 3);
            this.dgvEstadisticas.Name = "dgvEstadisticas";
            this.dgvEstadisticas.ReadOnly = true;
            this.dgvEstadisticas.RowHeadersWidth = 51;
            this.dgvEstadisticas.RowTemplate.Height = 24;
            this.dgvEstadisticas.Size = new System.Drawing.Size(719, 401);
            this.dgvEstadisticas.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.labelEstadoHistorial);
            this.tabPage2.Controls.Add(this.dgvHistorial);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(725, 407);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "HIstorial de Partidas";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // labelEstadoHistorial
            // 
            this.labelEstadoHistorial.AutoSize = true;
            this.labelEstadoHistorial.Location = new System.Drawing.Point(320, 12);
            this.labelEstadoHistorial.Name = "labelEstadoHistorial";
            this.labelEstadoHistorial.Size = new System.Drawing.Size(44, 16);
            this.labelEstadoHistorial.TabIndex = 1;
            this.labelEstadoHistorial.Text = "label2";
            this.labelEstadoHistorial.Click += new System.EventHandler(this.label2_Click);
            // 
            // dgvHistorial
            // 
            this.dgvHistorial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistorial.Location = new System.Drawing.Point(3, 3);
            this.dgvHistorial.Name = "dgvHistorial";
            this.dgvHistorial.ReadOnly = true;
            this.dgvHistorial.RowHeadersWidth = 51;
            this.dgvHistorial.RowTemplate.Height = 24;
            this.dgvHistorial.Size = new System.Drawing.Size(719, 401);
            this.dgvHistorial.TabIndex = 0;
            this.dgvHistorial.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dgvMovimientos);
            this.tabPage3.Controls.Add(this.comboPartidas);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(725, 407);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Movimientos";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // comboPartidas
            // 
            this.comboPartidas.FormattingEnabled = true;
            this.comboPartidas.Location = new System.Drawing.Point(6, 6);
            this.comboPartidas.Name = "comboPartidas";
            this.comboPartidas.Size = new System.Drawing.Size(713, 24);
            this.comboPartidas.TabIndex = 0;
            this.comboPartidas.SelectedIndexChanged += new System.EventHandler(this.comboPartidas_SelectedIndexChanged);
            // 
            // dgvMovimientos
            // 
            this.dgvMovimientos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMovimientos.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvMovimientos.Location = new System.Drawing.Point(3, 74);
            this.dgvMovimientos.Name = "dgvMovimientos";
            this.dgvMovimientos.RowHeadersWidth = 51;
            this.dgvMovimientos.RowTemplate.Height = 24;
            this.dgvMovimientos.Size = new System.Drawing.Size(719, 330);
            this.dgvMovimientos.TabIndex = 1;
            // 
            // FormHistorial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UnoGame.Properties.Resources.fondomenu;
            this.ClientSize = new System.Drawing.Size(757, 460);
            this.Controls.Add(this.Estadisticas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormHistorial";
            this.Text = "FormHistorial";
            this.Load += new System.EventHandler(this.FormHistorial_Load);
            this.Estadisticas.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEstadisticas)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl Estadisticas;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dgvEstadisticas;
        private System.Windows.Forms.DataGridView dgvHistorial;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelEstadoHistorial;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dgvMovimientos;
        private System.Windows.Forms.ComboBox comboPartidas;
    }
}