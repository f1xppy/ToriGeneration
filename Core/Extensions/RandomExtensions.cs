using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToriGeneration.Core.Extensions
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Возвращает случайное число между min и max
        /// </summary>
        /// <param name="random">Экземпляр Random</param>
        /// <param name="min">Минимальное значение (включительно)</param>
        /// <param name="max">Максимальное значение (исключительно)</param>
        /// <returns>Случайное число в диапазоне [min, max)</returns>
        public static double RandomDouble(this Random random, double min, double max)
        {
            if (min > max)
                throw new ArgumentException("Min value cannot be greater than max value");

            return (random.NextDouble() * (max - min)) + min;
        }
    }
}
