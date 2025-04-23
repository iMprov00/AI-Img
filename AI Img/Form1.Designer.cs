namespace AI_Img
{
    partial class Form1
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
            this.picSource = new System.Windows.Forms.PictureBox();
            this.picResult = new System.Windows.Forms.PictureBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.txtPrompt = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picResult)).BeginInit();
            this.SuspendLayout();
            // 
            // picSource
            // 
            this.picSource.Location = new System.Drawing.Point(26, 12);
            this.picSource.Name = "picSource";
            this.picSource.Size = new System.Drawing.Size(177, 188);
            this.picSource.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picSource.TabIndex = 0;
            this.picSource.TabStop = false;
            this.picSource.DoubleClick += new System.EventHandler(this.picSource_DoubleClick);
            // 
            // picResult
            // 
            this.picResult.Location = new System.Drawing.Point(254, 12);
            this.picResult.Name = "picResult";
            this.picResult.Size = new System.Drawing.Size(521, 426);
            this.picResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picResult.TabIndex = 1;
            this.picResult.TabStop = false;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.btnGenerate.Location = new System.Drawing.Point(26, 253);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "button1";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtPrompt
            // 
            this.txtPrompt.Location = new System.Drawing.Point(141, 253);
            this.txtPrompt.Name = "txtPrompt";
            this.txtPrompt.Size = new System.Drawing.Size(100, 20);
            this.txtPrompt.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(65, 302);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(35, 13);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtPrompt);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.picResult);
            this.Controls.Add(this.picSource);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picSource;
        private System.Windows.Forms.PictureBox picResult;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox txtPrompt;
        private System.Windows.Forms.Label lblStatus;
    }
}

