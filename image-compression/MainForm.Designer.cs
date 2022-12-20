namespace image_compression
{
    partial class MainForm
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
			System.Windows.Forms.Label label_sko;
			this.button_compressImage = new System.Windows.Forms.Button();
			this.button_loadComressedImage = new System.Windows.Forms.Button();
			this.tB_sko = new System.Windows.Forms.TextBox();
			label_sko = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button_compressImage
			// 
			this.button_compressImage.Location = new System.Drawing.Point(12, 12);
			this.button_compressImage.Name = "button_compressImage";
			this.button_compressImage.Size = new System.Drawing.Size(200, 70);
			this.button_compressImage.TabIndex = 2;
			this.button_compressImage.Text = "Сжать исходное изображение";
			this.button_compressImage.UseVisualStyleBackColor = true;
			this.button_compressImage.Click += new System.EventHandler(this.OnClickButtonCompressImage);
			// 
			// button_loadComressedImage
			// 
			this.button_loadComressedImage.Location = new System.Drawing.Point(218, 12);
			this.button_loadComressedImage.Name = "button_loadComressedImage";
			this.button_loadComressedImage.Size = new System.Drawing.Size(200, 70);
			this.button_loadComressedImage.TabIndex = 4;
			this.button_loadComressedImage.Text = "Восстановить сжатое изображение из файла";
			this.button_loadComressedImage.UseVisualStyleBackColor = true;
			this.button_loadComressedImage.Click += new System.EventHandler(this.OnClickButtonLoadComressedImage);
			// 
			// tB_sko
			// 
			this.tB_sko.Location = new System.Drawing.Point(218, 88);
			this.tB_sko.Name = "tB_sko";
			this.tB_sko.ReadOnly = true;
			this.tB_sko.Size = new System.Drawing.Size(199, 25);
			this.tB_sko.TabIndex = 5;
			this.tB_sko.Text = "0";
			this.tB_sko.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label_sko
			// 
			label_sko.Location = new System.Drawing.Point(12, 88);
			label_sko.Name = "label_sko";
			label_sko.Size = new System.Drawing.Size(200, 25);
			label_sko.TabIndex = 6;
			label_sko.Text = "СКО:";
			label_sko.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(429, 122);
			this.Controls.Add(label_sko);
			this.Controls.Add(this.tB_sko);
			this.Controls.Add(this.button_loadComressedImage);
			this.Controls.Add(this.button_compressImage);
			this.Font = new System.Drawing.Font("JetBrains Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "ИСИТ ННГУ | Сжатие изображения";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
		private System.Windows.Forms.Button button_compressImage;
		private System.Windows.Forms.Button button_loadComressedImage;
		private System.Windows.Forms.TextBox tB_sko;
	}
}