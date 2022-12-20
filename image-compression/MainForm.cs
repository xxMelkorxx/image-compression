using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace image_compression
{
    public partial class MainForm : Form
    {
        private Bitmap _initImage;
        private ImageCompression _imageCompression;
        private ComplexMatrix _initMatrix;
        private ComplexMatrix _restoredMatrix;

        public MainForm()
        {
            InitializeComponent();
        }

        private void OnClickButtonCompressImage(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files(*.PNG)|*.PNG;|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _initImage = new Bitmap(dialog.FileName);
                    _imageCompression = new ImageCompression(_initImage);
                    _initMatrix = _imageCompression.InitMatrix;
                    CallImageForm("Исходное изображение", _imageCompression.InitMatrix.GetBitmap());
                    CallImageForm("Результат квантования изображения", _imageCompression.FctMatrix.GetBitmap());
                    tB_sko.Text = "0";
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Ошибка!");
                }
            }
        }

        private void OnClickButtonLoadComressedImage(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Binaries(*.binary)|*.binary;|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using var reader = new BinaryReader(File.OpenRead(dialog.FileName), Encoding.Default);
                    _imageCompression = new ImageCompression(reader);
                    _restoredMatrix = _imageCompression.InitMatrix;
                    CallImageForm("Разархивированное изображение", _imageCompression.InitMatrix.GetBitmap());
                    tB_sko.Text = ImageCompression.GetStandardDeviation(_initMatrix, _restoredMatrix).ToString("F5");
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