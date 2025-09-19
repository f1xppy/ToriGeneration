using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Abstract.Services;
using ToriGeneration.Core.Abstract.Strategies;
using ToriGeneration.Core.Extensions.Geometry;
using ToriGeneration.Core.Generators;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;
using ToriGeneration.Core.Models.Dto.Responses;
using ToriGeneration.Core.Models.Enums;
using ToriGeneration.Core.Abstract.Generators;

namespace ToriGeneration.Services
{
    public class TorusGenerationService : ITorusGenerationService
    {
        private readonly Dictionary<GenerationMethod, ITorusGenerationStrategy> _strategies;

        private readonly ITorusListGenerator _torusListGenerator;
        public TorusGenerationService(ITorusListGenerator torusListGenerator)
        {
            _torusListGenerator = torusListGenerator;
        }

        public async Task<TorusListResponse> GenerateAsync(TorusGenerationParameters parameters)
        {
            var rootNode = new Cube();
            rootNode.InitializeNodeParameters(parameters);
            return await _torusListGenerator.GenerateTorusList(parameters, rootNode);
        }
    }
}
