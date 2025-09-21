using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ToriGeneration.Core.Abstract.Generators;
using ToriGeneration.Core.Abstract.Strategies;
using ToriGeneration.Core.Extensions.Geometry;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;
using ToriGeneration.Core.Models.Dto.Responses;
using ToriGeneration.Core.Models.Enums;

namespace ToriGeneration.Core.Generators
{
    public class TorusListGenerator : ITorusListGenerator
    {
        private readonly Dictionary<GenerationMethod, ITorusGenerationStrategy> _generationStrategies;

        public TorusListGenerator(
        LinearTorusGenerator linear,
        GammaTorusGenerator gamma,
        GaussTorusGenerator gauss)
        {
            _generationStrategies = new()
            {
                [GenerationMethod.Linear] = linear,
                [GenerationMethod.Gamma] = gamma,
                [GenerationMethod.Gauss] = gauss
            };
        }

        public async Task<TorusListResponse> GenerateTorusList (TorusGenerationParameters parameters, Cube rootNode)
        {
            var stopwatch = Stopwatch.StartNew();

            if (!_generationStrategies.TryGetValue(parameters.GenerationType, out var generator))
                throw new ArgumentException($"Unsupported generation type: {parameters.GenerationType}");

            var cubeVolume = Math.Pow(rootNode.Edge, 3);
            var currentTorusVolume = 0.0;
            var currentConcentration = 0.0;
            var maxAttempts = 250;
            var currentAttempts = 0;

            var torusList = new List<TorusResponse>();
            var torus = new Torus();

            //var testList = new List<Torus>();

            do
            {
                var hasIntersections = false;
                do
                {
                    torus = await Task.Run(() => generator.GenerateTorus(rootNode, parameters));
                    currentAttempts++;

                    hasIntersections = false;

                    // проверка в octree
                    //foreach (Sphere sphere in torus.Spheres)
                    //{
                    //    if (!rootNode.Intersects(sphere))
                    //    {
                    //        if (rootNode.SpheresIntersectsWith(sphere))
                    //        {
                    //            hasIntersections = true;
                    //            break;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        hasIntersections = true;
                    //        break;
                    //    }
                    //}
                    Parallel.ForEach(torus.Spheres, (sphere, state) =>
                    {
                        if (!rootNode.Intersects(sphere))
                        {
                            if (rootNode.SpheresIntersectsWith(sphere))
                            {
                                hasIntersections = true;
                                state.Stop();
                            }
                        }
                        else
                        {
                            hasIntersections = true;
                            state.Stop();
                        }
                    });

                    if (!hasIntersections)
                    {
                        torusList.Add(new TorusResponse
                        {
                            Center = torus.Center,
                            MajorRadius = torus.MajorRadius,
                            MinorRadius = torus.MinorRadius,
                            Rotation = torus.Rotation,
                        });
                        //testList.Add(torus);
                        foreach (var sphere in torus.Spheres)
                        {
                            rootNode.Insert(sphere);
                        };
                        currentTorusVolume += torus.MajorRadius * torus.MinorRadius * torus.MinorRadius * 2 * Math.PI * Math.PI;
                        currentConcentration = currentTorusVolume / cubeVolume;
                        currentAttempts = 0;
                    }

                }
                while (hasIntersections && currentAttempts < maxAttempts);

            }
            while (currentAttempts < maxAttempts && torusList.Count < parameters.TargetTorusCount);

            //int intersectionCount = 0;

            //for (int i = 0; i < testList.Count; i++)
            //{
            //    for (int j = i + 1; j < testList.Count; j++)
            //    {
            //        foreach (var sphere1 in testList[i].Spheres)
            //        {
            //            foreach (var sphere2 in testList[j].Spheres)
            //            {
            //                if (sphere1.IntersectsWith(sphere2))
            //                {
            //                    intersectionCount++;
            //                }
            //            }
            //        }
            //    }
            //}

            stopwatch.Stop();

            return new TorusListResponse
            {
                TorusList = torusList,
                TotalCount = torusList.Count,
                Concentration = currentConcentration,
                ElapsedTime = stopwatch.Elapsed
            };
        }
    }
}
