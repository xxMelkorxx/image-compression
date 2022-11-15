using System;
using System.Drawing;
using System.Windows.Forms;

namespace image_compression
{
    public partial class MainForm : Form
    {
        private Bitmap _initImage;
        private ImageCompression _imageCompression;

        public MainForm()
        {
            InitializeComponent();
        }

        private void OnClickButtonLoadImage(object sender, EventArgs e)
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
                    CallImageForm("Исходное изображение", _imageCompression.InitImage);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Ошибка!");
                }
            }
        }
        
        private void OnClickButtonCompressAndSaveImage(object sender, EventArgs e)
        {
            _imageCompression = new ImageCompression(_initImage);
            
            var dialog = new SaveFileDialog
            {
                Filter = "Binaries(*.binary)|*.binary;|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    
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