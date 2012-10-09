namespace RecognitionV3
{
    partial class FormMain
    {
        /// <summary>
        /// Требуется переменная конструктора.
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
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ImageViewer = new Emgu.CV.UI.ImageBox();
            this.labelTimeSpend = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.trainingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelmessege = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImageViewer)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImageViewer
            // 
            this.ImageViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageViewer.Location = new System.Drawing.Point(-1, 24);
            this.ImageViewer.Name = "ImageViewer";
            this.ImageViewer.Size = new System.Drawing.Size(746, 367);
            this.ImageViewer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ImageViewer.TabIndex = 2;
            this.ImageViewer.TabStop = false;
            // 
            // labelTimeSpend
            // 
            this.labelTimeSpend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTimeSpend.AutoSize = true;
            this.labelTimeSpend.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTimeSpend.Location = new System.Drawing.Point(12, 394);
            this.labelTimeSpend.Name = "labelTimeSpend";
            this.labelTimeSpend.Size = new System.Drawing.Size(0, 24);
            this.labelTimeSpend.TabIndex = 3;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trainingToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(746, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // trainingToolStripMenuItem
            // 
            this.trainingToolStripMenuItem.Name = "trainingToolStripMenuItem";
            this.trainingToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.trainingToolStripMenuItem.Text = "Training";
            this.trainingToolStripMenuItem.Click += new System.EventHandler(this.trainingToolStripMenuItem_Click);
            // 
            // labelmessege
            // 
            this.labelmessege.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelmessege.AutoSize = true;
            this.labelmessege.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelmessege.Location = new System.Drawing.Point(248, 394);
            this.labelmessege.Name = "labelmessege";
            this.labelmessege.Size = new System.Drawing.Size(0, 24);
            this.labelmessege.TabIndex = 5;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 427);
            this.Controls.Add(this.labelmessege);
            this.Controls.Add(this.labelTimeSpend);
            this.Controls.Add(this.ImageViewer);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "Main";
            ((System.ComponentModel.ISupportInitialize)(this.ImageViewer)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox ImageViewer;
        private System.Windows.Forms.Label labelTimeSpend;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem trainingToolStripMenuItem;
        private System.Windows.Forms.Label labelmessege;
    }
}

