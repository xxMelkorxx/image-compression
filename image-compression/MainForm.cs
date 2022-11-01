using System;
using System.Drawing;
using System.Windows.Forms;

namespace image_compression
{
    public partial class MainForm : Form
    {
		private Bitmap _initImage;

        public MainForm()
        {
            InitializeComponent();
        }

        private void OnClickButtonLoad(object sender, EventArgs e)
        {
			var dialog = new OpenFileDialog
			{
				Filter = "Image Files(*.PNG;)|*.PNG;|All files (*.*)|*.*"
			};
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					_initImage = new Bitmap(dialog.FileName);
					_initImage = ImageCompression.ConvertToHalftone(_initImage);

					CallImageForm("Исходное изображение", _initImage);
				}
				catch (Exception exception)
				{
					MessageBox.Show(exception.Message, "Ошибка!");
				}
			}
		}

		/// <summary>
		/// Функция вызовы формы с изображением.
		/// </summary>
		/// <param name="header">Название формы</param>
		/// <param name="bitmap">Изображение в формате Bitmap</param>
		private static void CallImageForm(string header, Bitmap bitmap)
		{
			var imageForm = new ImageForm(bitmap)
			{
				Text = header,
			};
			imageForm.Show();
		}
	}
}