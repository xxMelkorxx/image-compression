namespace image_compression
{
	partial class ImageForm
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
			this.pictureBox_image = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox_image
			// 
			this.pictureBox_image.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox_image.BackColor = System.Drawing.Color.Black;
			this.pictureBox_image.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox_image.Location = new System.Drawing.Point(13, 13);
			this.pictureBox_image.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.pictureBox_image.Name = "pictureBox_image";
			this.pictureBox_image.Size = new System.Drawing.Size(498, 501);
			this.pictureBox_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox_image.TabIndex = 0;
			this.pictureBox_image.TabStop = false;
			// 
			// ImageForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(524, 527);
			this.Controls.Add(this.pictureBox_image);
			this.Font = new System.Drawing.Font("JetBrains Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "ImageForm";
			this.Text = "ImageForm";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_image)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox_image;
	}
}