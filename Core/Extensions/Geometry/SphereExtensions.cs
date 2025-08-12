using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ToriGeneration.Core.Models.Dto.Geometry;

namespace ToriGeneration.Core.Extensions.Geometry
{
    public static class SphereExtensions
    {
        public static bool IntersectsWith(this Sphere a, Sphere b)
        {
            return a.Center.DistanceSquaredTo(b.Center) <= Math.Pow(a.Radius + b.Radius, 2);
        }
    }
}
