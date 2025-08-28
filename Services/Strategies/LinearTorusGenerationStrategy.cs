using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Abstract.Strategies;
using ToriGeneration.Core.Extensions.Geometry;
using ToriGeneration.Core.Generators;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;
using ToriGeneration.Core.Models.Dto.Responses;

namespace ToriGeneration.Services.Strategies
{
    public class LinearTorusGenerationStrategy : ITorusGenerationStrategy
    {
        private readonly TorusListGenerator _torusListGenerator = new TorusListGenerator();
        public async Task<TorusListResponse> Generate(TorusGenerationParameters parameters)
        {
            var rootNode = new Cube();
            rootNode.InitializeNodeParameters(parameters);
            return await _torusListGenerator.GenerateTorusList(parameters, rootNode);
        }
    }
}
