using System;
using System.Drawing;

namespace image_compression
{
    public class ImageCompression
    {
        private const int SizeSubmatrix = 8;

        private int[,] _qMatrix =
        {
            { 16, 11, 10, 16, 24, 40, 51, 61 },
            { 12, 12, 14, 19, 26, 58, 60, 55 },
            { 14, 13, 16, 24, 40, 57, 69, 56 },
            { 14, 17, 22, 29, 51, 87, 80, 62 },
            { 18, 22, 37, 56, 68, 109, 103, 77 },
            { 24, 35, 55, 64, 81, 104, 113, 92 },
            { 49, 64, 78, 87, 103, 121, 120, 101 },
            { 72, 92, 95, 98, 112, 100, 103, 99 }
        };

        private ComplexMatrix[,] _submatrices;
        private int N => _submatrices.GetLength(0);
        private int M => _submatrices.GetLength(1);

        public int Width => InitMatrix.Width;
        public int Height => InitMatrix.Height;

        public ComplexMatrix InitMatrix;

        public ImageCompression(Bitmap bitmap)
        {
            InitMatrix = new ComplexMatrix(bitmap);
            SplittingIntoSubmatrices();
            FourierTransformOfSubmatrices();
            FilteringSubmatrices();
        }

        /// <summary>
        /// Разбивает основную матрицу на подматрицы размером <see cref="SizeSubmatrix"/>.
        /// </summary>
        private void SplittingIntoSubmatrices()
        {
            _submatrices = new ComplexMatrix[Width / SizeSubmatrix, Height / SizeSubmatrix];
            for (var n = 0; n < N; n++)
            for (var m = 0; m < M; m++)
            {
                _submatrices[n, m] = new ComplexMatrix(SizeSubmatrix, SizeSubmatrix);
                var ni = n * SizeSubmatrix;
                var mj = m * SizeSubmatrix;
                for (var i = 0; i < SizeSubmatrix; i++)
                for (var j = 0; j < SizeSubmatrix; j++)
                    _submatrices[n, m].Matrix[i][j] = InitMatrix.Matrix[ni + i][mj + j];
            }
        }

        /// <summary>
        /// Преобразование фурье для каждой субматрицы.
        /// </summary>
        private void FourierTransformOfSubmatrices()
        {
            for (var n = 0; n < N; n++)
            for (var m = 0; m < M; m++)
                _submatrices[n, m] = Fourier.FFT_2D(_submatrices[n, m], true);
        }

        /// <summary>
        /// Применение фильтра высоких частот для каждой подматрицы.
        /// </summary>
        private void FilteringSubmatrices()
        {
            for (var n = 0; n < N; n++)
            for (var m = 0; m < M; m++)
            {
                for (var i = 0; i < SizeSubmatrix; i++)
                for (var j = 0; j < SizeSubmatrix; j++)
                {
                    _submatrices[n, m].Matrix[i][j] = Math.Round(_submatrices[n, m].Matrix[i][j].Real / _qMatrix[i, j]);
                }
            }
        }
        
        /// <summary>
        /// Конвертация изображения в полутоновое.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap ConvertToHalftone(Bitmap bitmap)
        {
            var width = bitmap.Width - bitmap.Width % SizeSubmatrix;
            var height = bitmap.Height - bitmap.Height % SizeSubmatrix;

            var newBitmap = new Bitmap(width, height);

            for (var i = 0; i < bitmap.Width; i++)
            for (var j = 0; j < bitmap.Height; j++)
            {
                var pixel = bitmap.GetPixel(i, j);
                var halftoneValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                newBitmap.SetPixel(i, j, Color.FromArgb(halftoneValue, halftoneValue, halftoneValue));
            }

            return newBitmap;
        }
    }
}