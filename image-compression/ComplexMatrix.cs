using System.Numerics;
using System.Drawing;

namespace image_compression
{
    public struct ComplexMatrix
    {
        public int Width => Matrix.Length;
        public int Height => Matrix[0].Length;

        public Complex[][] Matrix;

        /// <summary>
        /// Конструктор. Инициализирует MatrixImage с указанным размером.
        /// </summary>
        /// <param name="width">Ширина</param>
        /// <param name="height">Спектр</param>
        public ComplexMatrix(int width, int height)
        {
            Matrix = new Complex[width][];
            for (var i = 0; i < width; i++)
                Matrix[i] = new Complex[height];
        }

        /// <summary>
        /// Конструктор. Создаёт из bitmap.
        /// </summary>
        /// <param name="bitmap"></param>
        public ComplexMatrix(Bitmap bitmap)
        {
            Matrix = new Complex[bitmap.Width][];
            for (var i = 0; i < Width; i++)
            {
                Matrix[i] = new Complex[bitmap.Height];
                for (var j = 0; j < Height; j++)
                    Matrix[i][j] = bitmap.GetPixel(i, j).R;
            }
        }
    }
}