using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Extensions.Geometry;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;

namespace ToriGeneration.Core.Generators
{
    public static class TorusListGenerator
    {
        public static List<Torus> GenerateTorusList (TorusGenerationParameters parameters, Cube rootNode)
        {
            var generator = new TorusGenerator();

            var cubeVolume = Math.Pow(rootNode.Edge, 3);
            var currentTorusVolume = 0.0;
            var currentConcentration = 0.0;
            var stopGeneration = false;
            var maxAttempts = 10000;
            var currentAttempts = 0;

            var torusList = new List<Torus>();
            var torus = new Torus();

            do
            {
                var hasIntersections = false;
                do
                {
                    torus = generator.GenerateTorus(rootNode, parameters);
                    currentAttempts++;

                    hasIntersections = false;

                    // проверка в octree
                    foreach (Sphere sphere in torus.Spheres)
                    {

                        if (!rootNode.Intersects(sphere))
                        {
                            if (rootNode.SheresIntersectsWith(sphere))
                            {
                                hasIntersections = true;
                                break;
                            }
                        }
                    }


                    if (!hasIntersections)
                    {
                        torusList.Add(torus);
                        foreach (Sphere sphere in torus.Spheres)
                        {
                            rootNode.Insert(sphere);
                        }
                        currentTorusVolume += torus.MajorRadius * torus.MinorRadius * torus.MinorRadius * 2 * Math.PI * Math.PI;
                        currentConcentration = currentTorusVolume / cubeVolume;
                        currentAttempts = 0;
                    }

                }
                while (hasIntersections && currentAttempts < maxAttempts);

            }
            while (currentAttempts < maxAttempts);

            return torusList;
        }
    }
}
