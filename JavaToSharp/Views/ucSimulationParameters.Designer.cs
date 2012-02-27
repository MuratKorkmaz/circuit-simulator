namespace JavaToSharp
{
    partial class ucSimulationParameters
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btReset = new System.Windows.Forms.Button();
            this.chbStop = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.sbSimulationSpeed = new System.Windows.Forms.HScrollBar();
            this.label2 = new System.Windows.Forms.Label();
            this.sbCurrentSpeed = new System.Windows.Forms.HScrollBar();
            this.label3 = new System.Windows.Forms.Label();
            this.sbPowerLight = new System.Windows.Forms.HScrollBar();
            this.lbCopyright = new System.Windows.Forms.Label();
            this.lbCurrentScheme = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btReset);
            this.flowLayoutPanel1.Controls.Add(this.chbStop);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.sbSimulationSpeed);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.sbCurrentSpeed);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.sbPowerLight);
            this.flowLayoutPanel1.Controls.Add(this.lbCopyright);
            this.flowLayoutPanel1.Controls.Add(this.lbCurrentScheme);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(142, 272);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btReset
            // 
            this.btReset.Location = new System.Drawing.Point(3, 3);
            this.btReset.Name = "btReset";
            this.btReset.Size = new System.Drawing.Size(135, 23);
            this.btReset.TabIndex = 0;
            this.btReset.Text = "Сброс";
            this.btReset.UseVisualStyleBackColor = true;
            this.btReset.Click += new System.EventHandler(this.btReset_Click);
            // 
            // chbStop
            // 
            this.chbStop.AutoSize = true;
            this.chbStop.Location = new System.Drawing.Point(3, 32);
            this.chbStop.Name = "chbStop";
            this.chbStop.Size = new System.Drawing.Size(93, 17);
            this.chbStop.TabIndex = 1;
            this.chbStop.Text = "Остановлено";
            this.chbStop.UseVisualStyleBackColor = true;
            this.chbStop.CheckedChanged += new System.EventHandler(this.chbStop_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Скорость симуляции";
            // 
            // sbSimulationSpeed
            // 
            this.sbSimulationSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sbSimulationSpeed.Location = new System.Drawing.Point(0, 65);
            this.sbSimulationSpeed.Maximum = 260;
            this.sbSimulationSpeed.Name = "sbSimulationSpeed";
            this.sbSimulationSpeed.Size = new System.Drawing.Size(141, 17);
            this.sbSimulationSpeed.TabIndex = 3;
            this.sbSimulationSpeed.Value = 3;
            this.sbSimulationSpeed.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sbSimulationSpeed_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Скорость тока";
            // 
            // sbCurrentSpeed
            // 
            this.sbCurrentSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sbCurrentSpeed.Location = new System.Drawing.Point(0, 95);
            this.sbCurrentSpeed.Minimum = 1;
            this.sbCurrentSpeed.Name = "sbCurrentSpeed";
            this.sbCurrentSpeed.Size = new System.Drawing.Size(141, 17);
            this.sbCurrentSpeed.TabIndex = 5;
            this.sbCurrentSpeed.Value = 50;
            this.sbCurrentSpeed.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sbCurrentSpeed_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Яркость мощности";
            // 
            // sbPowerLight
            // 
            this.sbPowerLight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sbPowerLight.Enabled = false;
            this.sbPowerLight.Location = new System.Drawing.Point(0, 125);
            this.sbPowerLight.Minimum = 1;
            this.sbPowerLight.Name = "sbPowerLight";
            this.sbPowerLight.Size = new System.Drawing.Size(141, 17);
            this.sbPowerLight.TabIndex = 7;
            this.sbPowerLight.Value = 50;
            this.sbPowerLight.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sbPowerLight_Scroll);
            // 
            // lbCopyright
            // 
            this.lbCopyright.AutoSize = true;
            this.lbCopyright.Location = new System.Drawing.Point(3, 142);
            this.lbCopyright.Name = "lbCopyright";
            this.lbCopyright.Size = new System.Drawing.Size(55, 13);
            this.lbCopyright.TabIndex = 8;
            this.lbCopyright.Text = "Копирайт";
            // 
            // lbCurrentScheme
            // 
            this.lbCurrentScheme.AutoSize = true;
            this.lbCurrentScheme.Location = new System.Drawing.Point(3, 155);
            this.lbCurrentScheme.Name = "lbCurrentScheme";
            this.lbCurrentScheme.Size = new System.Drawing.Size(86, 13);
            this.lbCurrentScheme.TabIndex = 9;
            this.lbCurrentScheme.Text = "Текущая схема";
            // 
            // ucSimulationParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "ucSimulationParameters";
            this.Size = new System.Drawing.Size(145, 275);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btReset;
        private System.Windows.Forms.CheckBox chbStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.HScrollBar sbSimulationSpeed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.HScrollBar sbCurrentSpeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.HScrollBar sbPowerLight;
        private System.Windows.Forms.Label lbCopyright;
        private System.Windows.Forms.Label lbCurrentScheme;

    }
}
