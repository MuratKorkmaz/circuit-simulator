using System.Windows.Forms;

namespace JavaToSharp
{
    partial class FormMain
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportLink = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.осциллографToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUnionCurves = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBreakCurves = new System.Windows.Forms.ToolStripMenuItem();
            this.схемыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.pbFooter = new System.Windows.Forms.PictureBox();
            this.pbCircuit = new System.Windows.Forms.PictureBox();
            this._ucSimulationParameters = new JavaToSharp.ucSimulationParameters();
            this.menuStrip1.SuspendLayout();
            this.tlpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFooter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCircuit)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.осциллографToolStripMenuItem,
            this.схемыToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(700, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "enuStrip";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiImport,
            this.tsmiExport,
            this.tsmiExportLink,
            this.toolStripSeparator1,
            this.tsmiExit});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // tsmiImport
            // 
            this.tsmiImport.Name = "tsmiImport";
            this.tsmiImport.Size = new System.Drawing.Size(165, 22);
            this.tsmiImport.Text = "Импорт";
            this.tsmiImport.Click += new System.EventHandler(this.tsmiImport_Click);
            // 
            // tsmiExport
            // 
            this.tsmiExport.Name = "tsmiExport";
            this.tsmiExport.Size = new System.Drawing.Size(165, 22);
            this.tsmiExport.Text = "Экспорт";
            this.tsmiExport.Click += new System.EventHandler(this.tsmiExport_Click);
            // 
            // tsmiExportLink
            // 
            this.tsmiExportLink.Name = "tsmiExportLink";
            this.tsmiExportLink.Size = new System.Drawing.Size(165, 22);
            this.tsmiExportLink.Text = "Экспорт. ссылку";
            this.tsmiExportLink.Click += new System.EventHandler(this.tsmiExportLink_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(162, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(165, 22);
            this.tsmiExit.Text = "Выход";
            // 
            // осциллографToolStripMenuItem
            // 
            this.осциллографToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiUnionCurves,
            this.tsmiBreakCurves});
            this.осциллографToolStripMenuItem.Name = "осциллографToolStripMenuItem";
            this.осциллографToolStripMenuItem.Size = new System.Drawing.Size(96, 20);
            this.осциллографToolStripMenuItem.Text = "Осциллограф";
            // 
            // tsmiUnionCurves
            // 
            this.tsmiUnionCurves.Name = "tsmiUnionCurves";
            this.tsmiUnionCurves.Size = new System.Drawing.Size(164, 22);
            this.tsmiUnionCurves.Text = "Объединить всё";
            this.tsmiUnionCurves.Click += new System.EventHandler(this.tsmiUnionCurves_Click);
            // 
            // tsmiBreakCurves
            // 
            this.tsmiBreakCurves.Name = "tsmiBreakCurves";
            this.tsmiBreakCurves.Size = new System.Drawing.Size(164, 22);
            this.tsmiBreakCurves.Text = "Разъединить всё";
            this.tsmiBreakCurves.Click += new System.EventHandler(this.tsmiBreakCurves_Click);
            // 
            // схемыToolStripMenuItem
            // 
            this.схемыToolStripMenuItem.Name = "схемыToolStripMenuItem";
            this.схемыToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.схемыToolStripMenuItem.Text = "Схемы";
            // 
            // tlpMain
            // 
            this.tlpMain.BackColor = System.Drawing.Color.White;
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlpMain.Controls.Add(this.pbFooter, 0, 0);
            this.tlpMain.Controls.Add(this.pbCircuit, 0, 1);
            this.tlpMain.Controls.Add(this._ucSimulationParameters, 1, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 24);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpMain.Size = new System.Drawing.Size(700, 481);
            this.tlpMain.TabIndex = 1;
            // 
            // pbFooter
            // 
            this.pbFooter.BackColor = System.Drawing.Color.White;
            this.tlpMain.SetColumnSpan(this.pbFooter, 2);
            this.pbFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbFooter.Image = global::JavaToSharp.Properties.Resources.footer;
            this.pbFooter.Location = new System.Drawing.Point(3, 3);
            this.pbFooter.Name = "pbFooter";
            this.pbFooter.Size = new System.Drawing.Size(694, 94);
            this.pbFooter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbFooter.TabIndex = 0;
            this.pbFooter.TabStop = false;
            // 
            // pbCircuit
            // 
            this.pbCircuit.BackColor = System.Drawing.Color.Black;
            this.pbCircuit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbCircuit.Location = new System.Drawing.Point(3, 103);
            this.pbCircuit.Name = "pbCircuit";
            this.pbCircuit.Size = new System.Drawing.Size(544, 275);
            this.pbCircuit.TabIndex = 1;
            this.pbCircuit.TabStop = false;
            // 
            // _ucSimulationParameters
            // 
            this._ucSimulationParameters.CurrentSpeed = 50;
            this._ucSimulationParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ucSimulationParameters.IsStopped = false;
            this._ucSimulationParameters.Location = new System.Drawing.Point(553, 103);
            this._ucSimulationParameters.Name = "_ucSimulationParameters";
            this._ucSimulationParameters.PowerLight = 50;
            this._ucSimulationParameters.SimulationSpeed = 3;
            this._ucSimulationParameters.Size = new System.Drawing.Size(144, 275);
            this._ucSimulationParameters.TabIndex = 2;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 505);
            this.Controls.Add(this.tlpMain);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "Circuit Simulator v0.99";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormMain_KeyPress);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbFooter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCircuit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiImport;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportLink;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ToolStripMenuItem осциллографToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiUnionCurves;
        private System.Windows.Forms.ToolStripMenuItem tsmiBreakCurves;
        private System.Windows.Forms.ToolStripMenuItem схемыToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.PictureBox pbFooter;
        private System.Windows.Forms.PictureBox pbCircuit;
        private ucSimulationParameters _ucSimulationParameters;
    }
}

