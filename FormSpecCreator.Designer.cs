namespace SpecCreator
{
    partial class FormSpecCreator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSpecCreator));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnBatchD2S = new System.Windows.Forms.Button();
            this.btnS2D = new System.Windows.Forms.Button();
            this.btnD2S = new System.Windows.Forms.Button();
            this.btnBatchS2D = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 190);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(318, 23);
            this.progressBar.TabIndex = 40;
            // 
            // btnBatchD2S
            // 
            this.btnBatchD2S.Location = new System.Drawing.Point(0, 100);
            this.btnBatchD2S.Name = "btnBatchD2S";
            this.btnBatchD2S.Size = new System.Drawing.Size(156, 82);
            this.btnBatchD2S.TabIndex = 39;
            this.btnBatchD2S.Text = "批次 Word 轉 SQL";
            this.btnBatchD2S.UseVisualStyleBackColor = true;
            // 
            // btnS2D
            // 
            this.btnS2D.Location = new System.Drawing.Point(162, 12);
            this.btnS2D.Name = "btnS2D";
            this.btnS2D.Size = new System.Drawing.Size(156, 82);
            this.btnS2D.TabIndex = 38;
            this.btnS2D.Text = "SQL 轉 Word";
            this.btnS2D.UseVisualStyleBackColor = true;
            // 
            // btnD2S
            // 
            this.btnD2S.Location = new System.Drawing.Point(0, 12);
            this.btnD2S.Name = "btnD2S";
            this.btnD2S.Size = new System.Drawing.Size(156, 82);
            this.btnD2S.TabIndex = 37;
            this.btnD2S.Text = "Word 轉 SQL";
            this.btnD2S.UseVisualStyleBackColor = true;
            // 
            // btnBatchS2D
            // 
            this.btnBatchS2D.Location = new System.Drawing.Point(162, 100);
            this.btnBatchS2D.Name = "btnBatchS2D";
            this.btnBatchS2D.Size = new System.Drawing.Size(156, 82);
            this.btnBatchS2D.TabIndex = 41;
            this.btnBatchS2D.Text = "批次 SQL 轉 Word";
            this.btnBatchS2D.UseVisualStyleBackColor = true;
            // 
            // FormSpecCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(318, 213);
            this.Controls.Add(this.btnBatchS2D);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnBatchD2S);
            this.Controls.Add(this.btnS2D);
            this.Controls.Add(this.btnD2S);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormSpecCreator";
            this.Text = "規格產生器";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnBatchD2S;
        private System.Windows.Forms.Button btnS2D;
        private System.Windows.Forms.Button btnD2S;
        private System.Windows.Forms.Button btnBatchS2D;
    }
}