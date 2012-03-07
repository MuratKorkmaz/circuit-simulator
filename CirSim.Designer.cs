namespace circuit_emulator
{
    partial class CirSim
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pbFooter = new System.Windows.Forms.PictureBox();
            this.cv = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.resetButton = new System.Windows.Forms.Button();
            this.stoppedCheck = new System.Windows.Forms.CheckBox();
            this.lbSimSpeed = new System.Windows.Forms.Label();
            this.speedBar = new System.Windows.Forms.HScrollBar();
            this.lbCurrentSpeed = new System.Windows.Forms.Label();
            this.currentBar = new System.Windows.Forms.HScrollBar();
            this.powerLabel = new System.Windows.Forms.Label();
            this.powerBar = new System.Windows.Forms.HScrollBar();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFooter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cv)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tableLayoutPanel1.Controls.Add(this.cv, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(489, 471);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pbFooter
            // 
            this.pbFooter.BackColor = System.Drawing.Color.White;
            this.pbFooter.BackgroundImage = global::circuit_emulator.Properties.Resources.footer;
            this.pbFooter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbFooter.Location = new System.Drawing.Point(3, 3);
            this.pbFooter.Name = "pbFooter";
            this.pbFooter.Size = new System.Drawing.Size(282, 91);
            this.pbFooter.TabIndex = 0;
            this.pbFooter.TabStop = false;
            // 
            // cv
            // 
            this.cv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cv.Location = new System.Drawing.Point(3, 3);
            this.cv.Name = "cv";
            this.cv.Size = new System.Drawing.Size(183, 465);
            this.cv.TabIndex = 1;
            this.cv.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.pbFooter);
            this.flowLayoutPanel1.Controls.Add(this.resetButton);
            this.flowLayoutPanel1.Controls.Add(this.stoppedCheck);
            this.flowLayoutPanel1.Controls.Add(this.lbSimSpeed);
            this.flowLayoutPanel1.Controls.Add(this.speedBar);
            this.flowLayoutPanel1.Controls.Add(this.lbCurrentSpeed);
            this.flowLayoutPanel1.Controls.Add(this.currentBar);
            this.flowLayoutPanel1.Controls.Add(this.powerLabel);
            this.flowLayoutPanel1.Controls.Add(this.powerBar);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(192, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(294, 465);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(3, 100);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 0;
            this.resetButton.Text = "Сбросить";
            this.resetButton.UseVisualStyleBackColor = true;
            // 
            // stoppedCheck
            // 
            this.stoppedCheck.Location = new System.Drawing.Point(3, 129);
            this.stoppedCheck.Name = "stoppedCheck";
            this.stoppedCheck.Size = new System.Drawing.Size(104, 24);
            this.stoppedCheck.TabIndex = 1;
            this.stoppedCheck.Text = "Остановлено";
            this.stoppedCheck.UseVisualStyleBackColor = true;
            // 
            // lbSimSpeed
            // 
            this.lbSimSpeed.AutoSize = true;
            this.lbSimSpeed.Location = new System.Drawing.Point(3, 156);
            this.lbSimSpeed.Name = "lbSimSpeed";
            this.lbSimSpeed.Size = new System.Drawing.Size(35, 13);
            this.lbSimSpeed.TabIndex = 2;
            this.lbSimSpeed.Text = "label1";
            // 
            // speedBar
            // 
            this.speedBar.Location = new System.Drawing.Point(0, 169);
            this.speedBar.Name = "speedBar";
            this.speedBar.Size = new System.Drawing.Size(288, 17);
            this.speedBar.TabIndex = 3;
            // 
            // lbCurrentSpeed
            // 
            this.lbCurrentSpeed.AutoSize = true;
            this.lbCurrentSpeed.Location = new System.Drawing.Point(3, 186);
            this.lbCurrentSpeed.Name = "lbCurrentSpeed";
            this.lbCurrentSpeed.Size = new System.Drawing.Size(35, 13);
            this.lbCurrentSpeed.TabIndex = 6;
            this.lbCurrentSpeed.Text = "label2";
            // 
            // currentBar
            // 
            this.currentBar.Location = new System.Drawing.Point(0, 199);
            this.currentBar.Name = "currentBar";
            this.currentBar.Size = new System.Drawing.Size(285, 17);
            this.currentBar.TabIndex = 4;
            // 
            // powerLabel
            // 
            this.powerLabel.AutoSize = true;
            this.powerLabel.Location = new System.Drawing.Point(3, 216);
            this.powerLabel.Name = "powerLabel";
            this.powerLabel.Size = new System.Drawing.Size(35, 13);
            this.powerLabel.TabIndex = 7;
            this.powerLabel.Text = "label3";
            // 
            // powerBar
            // 
            this.powerBar.Location = new System.Drawing.Point(0, 229);
            this.powerBar.Name = "powerBar";
            this.powerBar.Size = new System.Drawing.Size(285, 15);
            this.powerBar.TabIndex = 5;
            // 
            // CirSim
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 471);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CirSim";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CirSim";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbFooter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cv)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pbFooter;
        internal System.Windows.Forms.PictureBox cv;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button resetButton;
        internal System.Windows.Forms.CheckBox stoppedCheck;
        private System.Windows.Forms.Label lbSimSpeed;
        private System.Windows.Forms.HScrollBar speedBar;
        private System.Windows.Forms.HScrollBar currentBar;
        private System.Windows.Forms.HScrollBar powerBar;
        private System.Windows.Forms.Label lbCurrentSpeed;
        private System.Windows.Forms.Label powerLabel;
    }
}