namespace KeyboardLayoutClient
{
    partial class ClientForm
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
            this.layoutIdTextBox = new System.Windows.Forms.TextBox();
            this.sendRequestButton = new System.Windows.Forms.Button();
            this.refreshLayoutButton = new System.Windows.Forms.Button();
            this.currentLayoutLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.presetLayoutsComboBox = new System.Windows.Forms.ComboBox();
            this.controlGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CheckConnectionButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logListBox
            // 
            this.logListBox.FormattingEnabled = true;
            this.logListBox.Location = new System.Drawing.Point(26, 35);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(341, 95);
            this.logListBox.TabIndex = 0;
            // 
            // layoutIdTextBox
            // 
            this.layoutIdTextBox.Location = new System.Drawing.Point(26, 137);
            this.layoutIdTextBox.Name = "layoutIdTextBox";
            this.layoutIdTextBox.Size = new System.Drawing.Size(251, 20);
            this.layoutIdTextBox.TabIndex = 1;
            this.layoutIdTextBox.TextChanged += new System.EventHandler(this.layoutIdTextBox_TextChanged);
            // 
            // sendRequestButton
            // 
            this.sendRequestButton.Location = new System.Drawing.Point(26, 182);
            this.sendRequestButton.Name = "sendRequestButton";
            this.sendRequestButton.Size = new System.Drawing.Size(85, 23);
            this.sendRequestButton.TabIndex = 2;
            this.sendRequestButton.Text = "sendRequest";
            this.sendRequestButton.UseVisualStyleBackColor = true;
            this.sendRequestButton.Click += new System.EventHandler(this.sendRequestButton_Click);
            // 
            // refreshLayoutButton
            // 
            this.refreshLayoutButton.Location = new System.Drawing.Point(117, 182);
            this.refreshLayoutButton.Name = "refreshLayoutButton";
            this.refreshLayoutButton.Size = new System.Drawing.Size(91, 23);
            this.refreshLayoutButton.TabIndex = 3;
            this.refreshLayoutButton.Text = "refreshLayout";
            this.refreshLayoutButton.UseVisualStyleBackColor = true;
            this.refreshLayoutButton.Click += new System.EventHandler(this.refreshLayoutButton_Click);
            // 
            // currentLayoutLabel
            // 
            this.currentLayoutLabel.AutoSize = true;
            this.currentLayoutLabel.Location = new System.Drawing.Point(373, 35);
            this.currentLayoutLabel.Name = "currentLayoutLabel";
            this.currentLayoutLabel.Size = new System.Drawing.Size(72, 13);
            this.currentLayoutLabel.TabIndex = 4;
            this.currentLayoutLabel.Text = "currentLayout";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(374, 61);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(35, 13);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.Text = "status";
            // 
            // presetLayoutsComboBox
            // 
            this.presetLayoutsComboBox.FormattingEnabled = true;
            this.presetLayoutsComboBox.Location = new System.Drawing.Point(26, 226);
            this.presetLayoutsComboBox.Name = "presetLayoutsComboBox";
            this.presetLayoutsComboBox.Size = new System.Drawing.Size(194, 21);
            this.presetLayoutsComboBox.TabIndex = 6;
            this.presetLayoutsComboBox.SelectedIndexChanged += new System.EventHandler(this.presetLayoutsComboBox_SelectedIndexChanged);
            // 
            // controlGroupBox
            // 
            this.controlGroupBox.Location = new System.Drawing.Point(26, 273);
            this.controlGroupBox.Name = "controlGroupBox";
            this.controlGroupBox.Size = new System.Drawing.Size(200, 100);
            this.controlGroupBox.TabIndex = 7;
            this.controlGroupBox.TabStop = false;
            this.controlGroupBox.Text = "groupBox1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(373, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "label1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(376, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "label2";
            // 
            // CheckConnectionButton
            // 
            this.CheckConnectionButton.Location = new System.Drawing.Point(214, 182);
            this.CheckConnectionButton.Name = "CheckConnectionButton";
            this.CheckConnectionButton.Size = new System.Drawing.Size(109, 23);
            this.CheckConnectionButton.TabIndex = 10;
            this.CheckConnectionButton.Text = "CheckConnection";
            this.CheckConnectionButton.UseVisualStyleBackColor = true;
            this.CheckConnectionButton.Click += new System.EventHandler(this.CheckConnectionButton_Click);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.CheckConnectionButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.controlGroupBox);
            this.Controls.Add(this.presetLayoutsComboBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.currentLayoutLabel);
            this.Controls.Add(this.refreshLayoutButton);
            this.Controls.Add(this.sendRequestButton);
            this.Controls.Add(this.layoutIdTextBox);
            this.Controls.Add(this.logListBox);
            this.Name = "ClientForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.TextBox layoutIdTextBox;
        private System.Windows.Forms.Button sendRequestButton;
        private System.Windows.Forms.Button refreshLayoutButton;
        private System.Windows.Forms.Label currentLayoutLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ComboBox presetLayoutsComboBox;
        private System.Windows.Forms.GroupBox controlGroupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button CheckConnectionButton;
    }
}

