namespace KeyboardLayoutServer
{
    partial class ServerForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.logListBox = new System.Windows.Forms.ListBox();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.serverGroupBox = new System.Windows.Forms.GroupBox();
            this.requestsCountLabel = new System.Windows.Forms.Label();
            this.startServerButton = new System.Windows.Forms.Button();
            this.stopServerButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // logListBox
            // 
            this.logListBox.FormattingEnabled = true;
            this.logListBox.Location = new System.Drawing.Point(0, 64);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(251, 95);
            this.logListBox.TabIndex = 0;
            // 
            // clearLogButton
            // 
            this.clearLogButton.Location = new System.Drawing.Point(265, 78);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(75, 23);
            this.clearLogButton.TabIndex = 1;
            this.clearLogButton.Text = "clearLog";
            this.clearLogButton.UseVisualStyleBackColor = true;
            this.clearLogButton.Click += new System.EventHandler(this.clearLogButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(48, 48);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(35, 13);
            this.statusLabel.TabIndex = 2;
            this.statusLabel.Text = "status";
            // 
            // serverGroupBox
            // 
            this.serverGroupBox.Location = new System.Drawing.Point(51, 198);
            this.serverGroupBox.Name = "serverGroupBox";
            this.serverGroupBox.Size = new System.Drawing.Size(200, 100);
            this.serverGroupBox.TabIndex = 3;
            this.serverGroupBox.TabStop = false;
            this.serverGroupBox.Text = "serverGroupBox";
            // 
            // requestsCountLabel
            // 
            this.requestsCountLabel.AutoSize = true;
            this.requestsCountLabel.Location = new System.Drawing.Point(265, 123);
            this.requestsCountLabel.Name = "requestsCountLabel";
            this.requestsCountLabel.Size = new System.Drawing.Size(75, 13);
            this.requestsCountLabel.TabIndex = 4;
            this.requestsCountLabel.Text = "requestsCount";
            // 
            // startServerButton
            // 
            this.startServerButton.Location = new System.Drawing.Point(362, 77);
            this.startServerButton.Name = "startServerButton";
            this.startServerButton.Size = new System.Drawing.Size(74, 23);
            this.startServerButton.TabIndex = 5;
            this.startServerButton.Text = "startServer";
            this.startServerButton.UseVisualStyleBackColor = true;
            this.startServerButton.Click += new System.EventHandler(this.startServerButton_Click);
            // 
            // stopServerButton
            // 
            this.stopServerButton.Location = new System.Drawing.Point(362, 107);
            this.stopServerButton.Name = "stopServerButton";
            this.stopServerButton.Size = new System.Drawing.Size(75, 23);
            this.stopServerButton.TabIndex = 6;
            this.stopServerButton.Text = "stopServer";
            this.stopServerButton.UseVisualStyleBackColor = true;
            this.stopServerButton.Click += new System.EventHandler(this.stopServerButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(345, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "label1";
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.stopServerButton);
            this.Controls.Add(this.startServerButton);
            this.Controls.Add(this.requestsCountLabel);
            this.Controls.Add(this.serverGroupBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.clearLogButton);
            this.Controls.Add(this.logListBox);
            this.Name = "ServerForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.Button clearLogButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.GroupBox serverGroupBox;
        private System.Windows.Forms.Label requestsCountLabel;
        private System.Windows.Forms.Button startServerButton;
        private System.Windows.Forms.Button stopServerButton;
        private System.Windows.Forms.Label label1;
    }
}

