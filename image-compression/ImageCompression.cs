using System;
using System.Drawing;
using System.IO;
using System.Numerics;

namespace image_compression
{
    public class ImageCompression
    {
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

        public ComplexMatrix InitMatrix;
        public ComplexMatrix FctMatrix;
        public ComplexMatrix RestoredMatrix;

        private const int SizeSubmatrix = 8;
        private ComplexMatrix[,] _submatrices;

        private int N => _submatrices.GetLength(0);
        private int M => _submatrices.GetLength(1);
        public int Width { get; set; }
        public int Height { get; set; }

        public ImageCompression(Bitmap bitmap)
        {
            var initImage = ConvertToHalftone(bitmap);
            Width = initImage.Width;
            Height = initImage.Height;
            InitMatrix = new ComplexMatrix(initImage);
            FctMatrix = new ComplexMatrix(Width, Height, true);

            SplittingIntoSubmatrices(true);
            FourierTransformOfSubmatrices(true);
            FilteringSubmatrices(true);

            HuffmanArchiver.EncodeFile(FctMatrix);
        }

        public ImageCompression(BinaryReader reader)
        {
            HuffmanArchiver.DecodeFile(reader);
            FctMatrix = HuffmanArchiver.GetDecodeMatrix();
            Width = FctMatrix.Width;
            Height = FctMatrix.Height;
            InitMatrix = new ComplexMatrix(Width, Height);

            SplittingIntoSubmatrices(false);
            FilteringSubmatrices(false);
            FourierTransformOfSubmatrices(false);

            for (var n = 0; n < N; n++)
            for (var m = 0; m < M; m++)
            for (var i = 0; i < SizeSubmatrix; i++)
            for (var j = 0; j < SizeSubmatrix; j++)
                InitMatrix.Matrix[n * SizeSubmatrix + i][m * SizeSubmatrix + j] = _submatrices[n, m].Matrix[i][j];
        }

        /// <summary>
        /// Разбивает основную матрицу на подматрицы размером <see cref="SizeSubmatrix"/>.
        /// </summary>
        private void SplittingIntoSubmatrices(bool direct)
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
                    if (direct)
                        _submatrices[n, m].Matrix[i][j] = InitMatrix.Matrix[ni + i][mj + j];
                    else
                        _submatrices[n, m].Matrix[i][j] = FctMatrix.Matrix[ni + i][mj + j];
            }
        }

        /// <summary>
        /// Преобразование фурье для каждой субматрицы.
        /// </summary>
        private void FourierTransformOfSubmatrices(bool direct)
        {
            for (var n = 0; n < N; n++)
            for (var m = 0; m < M; m++)
                if (direct)
                    _submatrices[n, m] = Fourier.DCT(_submatrices[n, m]);
                else
                    _submatrices[n, m] = Fourier.IDCT(_submatrices[n, m]);
        }

        /// <summary>
        /// Применение фильтра высоких частот для каждой подматрицы.
        /// </summary>
        private void FilteringSubmatrices(bool direct)
        {
            for (var n = 0; n < N; n++)
            for (var m = 0; m < M; m++)
            {
                var ni = n * SizeSubmatrix;
                var mj = m * SizeSubmatrix;
                for (var i = 0; i < SizeSubmatrix; i++)
                for (var j = 0; j < SizeSubmatrix; j++)
                    if (direct)
                        FctMatrix.Matrix[ni + i][mj + j] = _submatrices[n, m].Matrix[i][j] = new Complex(Math.Round(_submatrices[n, m].Matrix[i][j].Real / _qMatrix[i, j]), 0);
                    else
                        _submatrices[n, m].Matrix[i][j] *= _qMatrix[i, j];
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

            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                var pixel = bitmap.GetPixel(i, j);
                var halftoneValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                newBitmap.SetPixel(i, j, Color.FromArgb(halftoneValue, halftoneValue, halftoneValue));
            }

            return newBitmap;
        }

        /// <summary>
        /// Подсчёт среднеквадратичного отклонения между двумя матрицами.
        /// </summary>
        /// <param name="m1">Первая матрица.</param>
        /// <param name="m2">Вторая матрица.</param>
        /// <returns></returns>
        public static double GetStandardDeviation(ComplexMatrix m1, ComplexMatrix m2)
        {
            if (m1.Width != m2.Width ||
                m1.Height != m2.Height)
                return 0;

            var w = m1.Width;
            var h = m1.Height;
            double sumUp = 0, sumDown = 0;
            for (var i = 0; i < w; i++)
            for (var j = 0; j < h; j++)
            {
                sumUp += (m1.Matrix[i][j].Magnitude - m2.Matrix[i][j].Magnitude) * (m1.Matrix[i][j].Magnitude - m2.Matrix[i][j].Magnitude);
                sumDown += m1.Matrix[i][j].Magnitude * m2.Matrix[i][j].Magnitude;
            }

            return sumUp / sumDown;
        }
    }
}