using System;
using System.Numerics;

namespace image_compression
{
    /// <summary>
    /// Быстрое преобразование Фурье.
    /// </summary>
    public static class FFT
    {
        public const double DoublePi = 2 * Math.PI;

        /// <summary>
        /// Децимация по частоте.
        /// </summary>
        /// <param name="frame">Массив комлексных чисел.</param>
        /// <param name="direct">Прямой ход?</param>
        /// <returns></returns>
        public static Complex[] DecimationInFrequency(Complex[] frame, bool direct)
        {
            if (frame.Length == 1) return frame;
            var halfSampleSize = frame.Length >> 1;
            var fullSampleSize = frame.Length;

            var arg = direct ? -DoublePi / fullSampleSize : DoublePi / fullSampleSize;
            var omegaPowBase = new Complex(Math.Cos(arg), Math.Sin(arg));
            var omega = Complex.One;
            var result = new Complex[fullSampleSize];

            for (var j = 0; j < halfSampleSize; j++)
            {
                result[j] = frame[j] + frame[j + halfSampleSize];
                result[j + halfSampleSize] = omega * (frame[j] - frame[j + halfSampleSize]);
                omega *= omegaPowBase;
            }

            var yTop = new Complex[halfSampleSize];
            var yBottom = new Complex[halfSampleSize];
            for (var i = 0; i < halfSampleSize; i++)
            {
                yTop[i] = result[i];
                yBottom[i] = result[i + halfSampleSize];
            }

            yTop = DecimationInFrequency(yTop, direct);
            yBottom = DecimationInFrequency(yBottom, direct);
            for (var i = 0; i < halfSampleSize; i++)
            {
                var j = i << 1; // i = 2*j;
                result[j] = yTop[i];
                result[j + 1] = yBottom[i];
            }

            return result;
        }

        public static ComplexMatrix FFT_2D(ComplexMatrix matrix, bool direct)
        {
            var width = matrix.Width;
            var height = matrix.Height;
            var result = new ComplexMatrix(width, height);
            
            if (!direct) matrix = AngularTransform(matrix);
            for (var i = 0; i < width; i++)
                result.Matrix[i] = DecimationInFrequency(matrix.Matrix[i], direct);
            result = Transform(result);
            for (var i = 0; i < height; i++)
                result.Matrix[i] = DecimationInFrequency(result.Matrix[i], direct);
            result = Transform(result);
            if (direct) result = AngularTransform(result);

            if (!direct)
                for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    result.Matrix[i][j] /= width * height;

            return result;
        }

        /// <summary>
        /// Транспонирование матрицы.
        /// </summary>
        /// <param name="init">Исходная матрица</param>
        /// <returns>Транспонированная матрица</returns>
        public static ComplexMatrix Transform(ComplexMatrix init)
        {
            var width = init.Width;
            var height = init.Height;
            var result = new ComplexMatrix(height, width);
            
            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
                result.Matrix[i][j] = init.Matrix[j][i];
            
            return result;
        }

        /// <summary>
        /// Трансформация спектра, чтобы основная энергия была сконцентрирована в центре.
        /// </summary>
        public static ComplexMatrix AngularTransform(ComplexMatrix spectrum)
        {
            var width = spectrum.Width;
            var height = spectrum.Height;
            var halfWidth = width >> 1;
            var halfHeight = height >> 1;
            var result = new ComplexMatrix(width, height);

            for (var i = 0; i < halfWidth; i++)
            for (var j = 0; j < halfHeight; j++)
            {
                result.Matrix[i][j] = spectrum.Matrix[i + halfWidth][j + halfHeight];
                result.Matrix[i + halfWidth][j] = spectrum.Matrix[i][j + halfHeight];
                result.Matrix[i][j + halfHeight] = spectrum.Matrix[i + halfWidth][j];
                result.Matrix[i + halfWidth][j + halfHeight] = spectrum.Matrix[i][j];
            }

            return result;
        }
    }
}