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
			this.button_loadInitImage = new System.Windows.Forms.Button();
			this.button_compressAndSaveImage = new System.Windows.Forms.Button();
			this.button_loadComressedImage = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button_loadInitImage
			// 
			this.button_loadInitImage.Location = new System.Drawing.Point(12, 12);
			this.button_loadInitImage.Name = "button_loadInitImage";
			this.button_loadInitImage.Size = new System.Drawing.Size(200, 70);
			this.button_loadInitImage.TabIndex = 2;
			this.button_loadInitImage.Text = "Загрузить исходное изображение";
			this.button_loadInitImage.UseVisualStyleBackColor = true;
			this.button_loadInitImage.Click += new System.EventHandler(this.OnClickButtonLoad);
			// 
			// button_compressAndSaveImage
			// 
			this.button_compressAndSaveImage.Location = new System.Drawing.Point(218, 12);
			this.button_compressAndSaveImage.Name = "button_compressAndSaveImage";
			this.button_compressAndSaveImage.Size = new System.Drawing.Size(200, 70);
			this.button_compressAndSaveImage.TabIndex = 3;
			this.button_compressAndSaveImage.Text = "Сжать и сохранить изображение";
			this.button_compressAndSaveImage.UseVisualStyleBackColor = true;
			// 
			// button_loadComressedImage
			// 
			this.button_loadComressedImage.Location = new System.Drawing.Point(424, 12);
			this.button_loadComressedImage.Name = "button_loadComressedImage";
			this.button_loadComressedImage.Size = new System.Drawing.Size(200, 70);
			this.button_loadComressedImage.TabIndex = 4;
			this.button_loadComressedImage.Text = "Загрузить сжатое изображение";
			this.button_loadComressedImage.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(634, 89);
			this.Controls.Add(this.button_loadComressedImage);
			this.Controls.Add(this.button_compressAndSaveImage);
			this.Controls.Add(this.button_loadInitImage);
			this.Font = new System.Drawing.Font("JetBrains Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "ИСИТ ННГУ | Сжатие изображения";
			this.ResumeLayout(false);

        }

        #endregion
		private System.Windows.Forms.Button button_loadInitImage;
		private System.Windows.Forms.Button button_compressAndSaveImage;
		private System.Windows.Forms.Button button_loadComressedImage;
	}
}