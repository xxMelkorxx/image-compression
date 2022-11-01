using System.Drawing;
using System.Windows.Forms;

namespace image_compression
{
	public partial class ImageForm : Form
	{
		public ImageForm(Bitmap bitmap)
		{
			InitializeComponent();
			pictureBox_image.Image = bitmap;
			Clipboard.SetImage(bitmap);
		}
	}
}
