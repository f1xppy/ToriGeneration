using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using ToriGeneration.Core.Abstract.Generators;
using ToriGeneration.Core.Abstract.Strategies;
using ToriGeneration.Core.Configuration;
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
        private readonly ToriGenerationConfig _config;
        private readonly IMemoryCache _cache;

        private const int _batchSize = 8;

        public TorusListGenerator(
        LinearTorusGenerator linear,
        GammaTorusGenerator gamma,
        GaussTorusGenerator gauss,
        IOptions<ToriGenerationConfig> config,
        IMemoryCache cache)
        {
            _config = config.Value;
            _cache = cache;
            _generationStrategies = new()
            {
                [GenerationMethod.Linear] = linear,
                [GenerationMethod.Gamma] = gamma,
                [GenerationMethod.Gauss] = gauss
            };
        }


        public async Task<TorusListResponse> GenerateTorusList (TorusGenerationParameters parameters, 
            Cube rootNode, string cacheKey = "")
        {
            var stopwatch = Stopwatch.StartNew();
            var maxRadius = 0.0;

            if (!_generationStrategies.TryGetValue(parameters.GenerationType, out var generator))
                throw new ArgumentException($"Unsupported generation type: {parameters.GenerationType}");

            var cubeVolume = Math.Pow(rootNode.Edge, 3);
            var currentTorusVolume = 0.0;
            var currentConcentration = 0.0;
            var currentAttempts = 0;

            var torusList = new List<TorusResponse>();

            do
            {
                var remainingCount = parameters.TargetTorusCount - torusList.Count;
                var currentBatchSize = Math.Min(_batchSize, remainingCount);

                if (currentBatchSize <= 0) break;

                var batchResults = await GenerateAndValidateTorusBatch(
                    rootNode, parameters, currentBatchSize, generator);

                if (batchResults.Count == 0) currentAttempts += _batchSize;
                else {
                    foreach (var torus in batchResults)
                    {
                        torusList.Add(new TorusResponse
                        {
                            Center = torus.Center,
                            MajorRadius = torus.MajorRadius,
                            MinorRadius = torus.MinorRadius,
                            Rotation = torus.Rotation,
                        });
                        maxRadius = Math.Max(maxRadius, torus.MajorRadius + torus.MinorRadius);

                        if (cacheKey != "")
                        {
                            _cache.Set(cacheKey, torusList.Count);
                        }

                        rootNode.InsertBatch(torus.Spheres);
                        currentTorusVolume += torus.MajorRadius * torus.MinorRadius * torus.MinorRadius * 2 * Math.PI * Math.PI;
                        currentConcentration = currentTorusVolume / cubeVolume;
                    }
                    currentAttempts = 0;
                }

            } while (currentAttempts < _config.MaxAttempts && torusList.Count < parameters.TargetTorusCount);

            stopwatch.Stop();
            _cache.Set(cacheKey, -1);

            return new TorusListResponse
            {
                TorusList = torusList,
                TotalCount = torusList.Count,
                Concentration = currentConcentration,
                ElapsedTime = stopwatch.Elapsed,
                MaxRetries = currentAttempts,
                MaxRadius = maxRadius
            };
        }

        private async Task<List<Torus>> GenerateAndValidateTorusBatch(
            Cube rootNode, TorusGenerationParameters parameters, int batchSize, ITorusGenerationStrategy generator)
        {
            var generatedToruses = new List<Torus>();
            var tasks = new List<Task<Torus>>();

            for (int i = 0; i < batchSize; i++)
            {
                tasks.Add(Task.Run(async () => await generator.GenerateTorus(rootNode, parameters)));
            }

            var torusBatch = await Task.WhenAll(tasks);
            generatedToruses.AddRange(torusBatch);

            var validTorus = rootNode.CheckBatchIntersections(generatedToruses);
            var finalValidTorus = CheckInternalBatchIntersections(validTorus);

            return finalValidTorus;
        }

        public static List<Torus> CheckInternalBatchIntersections(List<Torus> torusBatch)
        {
            if (torusBatch.Count <= 1)
                return torusBatch;

            var validTorusList = new List<Torus>();
            var spheresCache = new List<Sphere>[torusBatch.Count];

            Parallel.For(0, torusBatch.Count, i =>
            {
                spheresCache[i] = torusBatch[i].Spheres.ToList();
            });

            for (int i = 0; i < torusBatch.Count; i++)
            {
                var torus = torusBatch[i];
                var spheres = spheresCache[i];
                bool hasInternalIntersections = false;

                foreach (var validTorus in validTorusList)
                {
                    if (CheckTorusSpheresIntersection(torus, validTorus))
                    {
                        hasInternalIntersections = true;
                        break;
                    }
                }

                if (!hasInternalIntersections)
                {
                    validTorusList.Add(torus);
                }
            }

            return validTorusList;
        }

        private static bool CheckTorusSpheresIntersection(Torus torus1, Torus torus2)
        {
            foreach (var sphere1 in torus1.Spheres)
            {
                foreach (var sphere2 in torus2.Spheres)
                {
                    if (sphere1.IntersectsWith(sphere2))
                        return true;
                }
            }
            return false;
        }
    }
}
