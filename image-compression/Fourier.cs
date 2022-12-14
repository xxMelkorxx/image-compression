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
		/// Дискретно-косинусное преобразование для матрицы размером 8x8.
		/// </summary>
		/// <param name="idct">Матрица 8х8</param>
		/// <returns></returns>
		public static ComplexMatrix DCT(ComplexMatrix idct)
        {
			const double pi = Math.PI;
			static double C(int x) => x == 0 ? 1 / Math.Sqrt(2) : 1;

			var dct = new ComplexMatrix(8, 8);
			for (var i = 0; i < 8; i++)
                for (var j = 0; j < 8; j++)
                {
                    for (var x = 0; x < 8; x++)
						for (var y = 0; y < 8; y++)
							dct.Matrix[i][j] += idct.Matrix[x][y] * Math.Cos((2 * x + 1) * (i * pi) / 16d) * Math.Cos((2 * y + 1) * (j * pi) / 16d);

                    dct.Matrix[i][j] *= 0.25 * C(i) * C(j);
				}

            return dct;
        }

		/// <summary>
		/// Дискретно-косинусное преобразование для матрицы размером 8x8.
		/// </summary>
		/// <param name="dct">Матрица 8х8</param>
		/// <returns></returns>
		public static ComplexMatrix IDCT(ComplexMatrix dct)
		{
			const double pi = Math.PI;
			static double C(int x) => x == 0 ? 1 / Math.Sqrt(2) : 1;

			var idct = new ComplexMatrix(8, 8);
			for (var x = 0; x < 8; x++)
				for (var y = 0; y < 8; y++)
				{
					for (var i = 0; i < 8; i++)
						for (var j = 0; j < 8; j++)
							idct.Matrix[x][y] += C(i) * C(j) * dct.Matrix[i][j] * Math.Cos((2 * x + 1) * (i * pi) / 16d) * Math.Cos((2 * y + 1) * (j * pi) / 16d);

					idct.Matrix[x][y] *= 0.25;
				}

			return idct;
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