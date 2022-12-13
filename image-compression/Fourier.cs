using System;
using System.Numerics;

namespace image_compression
{
    /// <summary>
    /// Преобразование Фурье.
    /// </summary>
    public static class Fourier
    {
        /// <summary>
        /// Быстрое преобразование фурье.
        /// </summary>
        /// <param name="frame">Массив комлексных чисел.</param>
        /// <param name="direct">Прямой ход?</param>
        /// <returns></returns>
        public static Complex[] FFT(Complex[] frame, bool direct)
        {
            if (frame.Length == 1) return frame;
            var halfSize = frame.Length >> 1;
            var fullSize = frame.Length;

            var arg = direct ? -2 * Math.PI / fullSize : 2 * Math.PI / fullSize;
            //var omegaPowBase = new Complex(Math.Cos(arg), 0);
            var omegaPowBase = new Complex(Math.Cos(arg), Math.Sin(arg));
            var omega = Complex.One;
            var result = new Complex[fullSize];

            for (var j = 0; j < halfSize; j++)
            {
                result[j] = frame[j] + frame[j + halfSize];
                result[j + halfSize] = omega * (frame[j] - frame[j + halfSize]);
                omega *= omegaPowBase;
            }

            var yTop = new Complex[halfSize];
            var yBottom = new Complex[halfSize];
            for (var i = 0; i < halfSize; i++)
            {
                yTop[i] = result[i];
                yBottom[i] = result[i + halfSize];
            }

            yTop = FFT(yTop, direct);
            yBottom = FFT(yBottom, direct);
            for (var i = 0; i < halfSize; i++)
            {
                var j = i << 1;
                result[j] = yTop[i];
                result[j + 1] = yBottom[i];
            }

            return result;
        }

        /// <summary>
        /// Двумерное быстрое преобразованеи Фурье.
        /// </summary>
        /// <param name="matrix">Исходная матрица.</param>
        /// <param name="direct">Прямой ход?</param>
        /// <returns></returns>
        public static ComplexMatrix FFT_2D(ComplexMatrix matrix, bool direct)
        {
            var width = matrix.Width;
            var height = matrix.Height;
            var result = new ComplexMatrix(width, height);
            
            //if (!direct) matrix = AngularTransform(matrix);
            for (var i = 0; i < width; i++)
                result.Matrix[i] = FFT(matrix.Matrix[i], direct);
            result = Transform(result);
            for (var i = 0; i < height; i++)
                result.Matrix[i] = FFT(result.Matrix[i], direct);
            result = Transform(result);
            //if (direct) result = AngularTransform(result);

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