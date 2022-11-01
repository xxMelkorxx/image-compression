using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace image_compression
{
    public class ImageCompression
    {
        private const int SizeSubmatrix = 8;

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
                _submatrices[n, m] = FFT.FFT_2D(_submatrices[n, m], true);
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