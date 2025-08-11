using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Models.Dto.Geometry;

namespace ToriGeneration.Core.Extensions.Geometry
{
    public static class PointExtensions
    {
        public static double DistanceSquaredTo(this Point a, Point b)
        {
            return Math.Pow(a.X - b.X, 2) +
                   Math.Pow(a.Y - b.Y, 2) +
                   Math.Pow(a.Z - b.Z, 2);
        }
    }
}
