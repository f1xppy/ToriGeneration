using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Extensions;
using ToriGeneration.Core.Extensions.Geometry;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;

namespace ToriGeneration.Core.Generators
{
    public class TorusGenerator
    {
        private readonly Random random = new();

        public Torus GenerateTorus(Cube cube, TorusGenerationParameters parameters)
        {
            var center = new Point
            {
                X = random.RandomDouble(-cube.Edge / 2, cube.Edge / 2),
                Y = random.RandomDouble(-cube.Edge / 2, cube.Edge / 2),
                Z = random.RandomDouble(-cube.Edge / 2, cube.Edge / 2)
            };

            var innerRadius = random.RandomDouble(parameters.MinTorusRadius, parameters.MaxTorusRadius);
            double minorRadius = innerRadius * parameters.TorusThicknessCoefficient;
            double majorRadius = innerRadius - minorRadius;

            var rotation = new Point
            {
                X = random.RandomDouble(0, 2 * Math.PI),
                Y = random.RandomDouble(0, 2 * Math.PI),
                Z = random.RandomDouble(0, 2 * Math.PI)
            };

            var torus = new Torus { 
                Center = center, 
                MajorRadius = majorRadius, 
                MinorRadius = minorRadius, 
                Rotation = rotation,
                Spheres = new List<Sphere>()
            };

            torus.GeneratePointsOnMajorCircle();

            return torus;
        }
    }
}
