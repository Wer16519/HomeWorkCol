namespace UnhookClient
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
            this.btnCreateHook = new System.Windows.Forms.Button();
            this.btnUnhook = new System.Windows.Forms.Button();
            this.listBoxResults = new System.Windows.Forms.ListBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelCurrentHook = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCreateHook
            // 
            this.btnCreateHook.Location = new System.Drawing.Point(327, 97);
            this.btnCreateHook.Name = "btnCreateHook";
            this.btnCreateHook.Size = new System.Drawing.Size(75, 23);
            this.btnCreateHook.TabIndex = 0;
            this.btnCreateHook.Text = "Create Hook";
            this.btnCreateHook.UseVisualStyleBackColor = true;
            this.btnCreateHook.Click += new System.EventHandler(this.btnCreateHook_Click);
            // 
            // btnUnhook
            // 
            this.btnUnhook.Location = new System.Drawing.Point(327, 136);
            this.btnUnhook.Name = "btnUnhook";
            this.btnUnhook.Size = new System.Drawing.Size(75, 23);
            this.btnUnhook.TabIndex = 1;
            this.btnUnhook.Text = "Unhook";
            this.btnUnhook.UseVisualStyleBackColor = true;
            this.btnUnhook.Click += new System.EventHandler(this.btnUnhook_Click);
            // 
            // listBoxResults
            // 
            this.listBoxResults.FormattingEnabled = true;
            this.listBoxResults.Location = new System.Drawing.Point(201, 187);
            this.listBoxResults.Name = "listBoxResults";
            this.listBoxResults.Size = new System.Drawing.Size(379, 95);
            this.listBoxResults.TabIndex = 2;
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(12, 288);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(621, 20);
            this.txtResult.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(327, 316);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(327, 333);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "label2";
            // 
            // labelCurrentHook
            // 
            this.labelCurrentHook.AutoSize = true;
            this.labelCurrentHook.Location = new System.Drawing.Point(327, 350);
            this.labelCurrentHook.Name = "labelCurrentHook";
            this.labelCurrentHook.Size = new System.Drawing.Size(67, 13);
            this.labelCurrentHook.TabIndex = 6;
            this.labelCurrentHook.Text = "CurrentHook";
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.labelCurrentHook);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.listBoxResults);
            this.Controls.Add(this.btnUnhook);
            this.Controls.Add(this.btnCreateHook);
            this.Name = "ClientForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreateHook;
        private System.Windows.Forms.Button btnUnhook;
        private System.Windows.Forms.ListBox listBoxResults;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelCurrentHook;
    }
}

