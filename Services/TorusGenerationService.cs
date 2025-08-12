using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Abstract.Services;
using ToriGeneration.Core.Abstract.Strategies;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;
using ToriGeneration.Core.Models.Enums;
using ToriGeneration.Services.Strategies;

namespace ToriGeneration.Services
{
    internal class TorusGenerationService : ITorusGenerationService
    {
        private readonly Dictionary<GenerationMethod, ITorusGenerationStrategy> _strategies;

        public TorusGenerationService(
        LinearTorusGenerationStrategy linear,
        GammaTorusGenerationStrategy gamma,
        GaussTorusGenerationStrategy gauss)
        {
            _strategies = new()
            {
                [GenerationMethod.Linear] = linear,
                [GenerationMethod.Gamma] = gamma,
                [GenerationMethod.Gauss] = gauss
            };
        }

        public async Task<List<Torus>> GenerateAsync(TorusGenerationParameters parameters)
        {
            if (!_strategies.TryGetValue(parameters.GenerationType, out var strategy))
                throw new ArgumentException($"Unsupported generation type: {parameters.GenerationType}");

            return await Task.Run(() => strategy.Generate(parameters));
        }
    }
}
