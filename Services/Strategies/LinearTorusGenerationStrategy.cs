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

namespace ToriGeneration.Services.Strategies
{
    public class LinearTorusGenerationStrategy : ITorusGenerationStrategy
    {
        public async Task<List<Torus>> Generate(TorusGenerationParameters parameters)
        {
            var rootNode = new Cube();
            rootNode.InitializeNodeParameters(parameters);
            return TorusListGenerator.GenerateTorusList(parameters, rootNode);
        }
    }
}
