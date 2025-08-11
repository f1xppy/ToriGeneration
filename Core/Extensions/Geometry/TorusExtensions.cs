using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Helpers;
using ToriGeneration.Core.Models.Dto.Geometry;

namespace ToriGeneration.Core.Extensions.Geometry
{
    public static class TorusExtensions
    {
        public static void GeneratePointsOnMajorCircle(this Torus torus)
        {
            var pointsCount = Convert.ToInt32(2 * Math.PI * torus.MajorRadius / torus.MinorRadius * 1.5);

            for (int i = 0; i < pointsCount; i++)
            {
                double angle = 2 * Math.PI / pointsCount * i;
                var p = new Point
                {
                    X = torus.Center.X + (torus.MajorRadius * Math.Cos(angle)),
                    Y = torus.Center.Y + (torus.MajorRadius * Math.Sin(angle)),
                    Z = torus.Center.Z
                };

                var helper = new PointRotationHelper();

                var point = helper.RotatePoint(p, torus.Center, torus.Rotation);

                var sphere = new Sphere
                {
                    Center = point,
                    Radius = torus.MinorRadius
                };

                torus.Spheres.Add(sphere);
            }
        }
    }
}
