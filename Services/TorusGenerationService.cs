using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToriGeneration.Core.Abstract.Generators;
using ToriGeneration.Core.Abstract.Services;
using ToriGeneration.Core.Abstract.Strategies;
using ToriGeneration.Core.Extensions.Geometry;
using ToriGeneration.Core.Generators;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;
using ToriGeneration.Core.Models.Dto.Responses;
using ToriGeneration.Core.Models.Enums;

namespace ToriGeneration.Services
{
    public class TorusGenerationService : ITorusGenerationService
    {
        private readonly ITorusListGenerator _torusListGenerator;
        public TorusGenerationService(ITorusListGenerator torusListGenerator)
        {
            _torusListGenerator = torusListGenerator;
        }

        public async Task<TorusListResponse> Generate(TorusGenerationParameters parameters)
        {
            var rootNode = new Cube { 
                Center = new Point { X = 0, Y = 0, Z = 0 }, 
                Children = [],
                Spheres = [],
            };
            rootNode.InitializeNodeParameters(parameters);
            rootNode.ComputeBounds();
            return await _torusListGenerator.GenerateTorusList(parameters, rootNode);
        }

        public async Task<List<string>> GenerateMultipleTorusLists(int taskCount, TorusGenerationParameters parameters)
        {
            var resultsDirectory = Path.Combine("jsonResults", 
                $"cube={parameters.CubeEdge}_count={parameters.TargetTorusCount}");
            Directory.CreateDirectory(resultsDirectory);

            var tasks = new List<Task<TorusListResponse>>();

            var result = RunGeneration(tasks, parameters, taskCount).Result;

            _ = Task.Run(() => AwaitGenerationComplete(tasks, resultsDirectory, result));

            return result;
        }

        private async Task<List<String>> RunGeneration(List<Task<TorusListResponse>> tasks, 
            TorusGenerationParameters parameters,
            int taskCount)
        {
            var cacheKeys = new List<string>();
            for (int i = 0; i < taskCount; i++)
            {
                var rootNode = new Cube
                {
                    Center = new Point { X = 0, Y = 0, Z = 0 },
                    Children = [],
                    Spheres = [],
                };
                rootNode.InitializeNodeParameters(parameters);
                rootNode.ComputeBounds();
                var cacheKey = Guid.NewGuid().ToString();
                cacheKeys.Add(cacheKey);
                tasks.Add(_torusListGenerator.GenerateTorusList(parameters, rootNode, cacheKey));
                
            }
            return cacheKeys;
        }

        private async Task AwaitGenerationComplete(List<Task<TorusListResponse>> tasks, 
            string resultsDirectory, List<string> cacheKeys)
        {
            var results = await Task.WhenAll(tasks);

            var saveTasks = new List<Task>();

            foreach (var result in results)
            {
                if (result != null)
                {
                    saveTasks.Add(SaveResultToFile(result, resultsDirectory));
                }
            }

            await Task.WhenAll(saveTasks);
        }

        private async Task SaveResultToFile(TorusListResponse result, string directory)
        {
            var fileName = $"{result.MaxRadius}.json";
            var filePath = Path.Combine(directory, fileName);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(result, options);
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}
