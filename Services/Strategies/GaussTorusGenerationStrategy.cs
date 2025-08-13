using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Abstract.Strategies;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;

namespace ToriGeneration.Services.Strategies
{
    public class GaussTorusGenerationStrategy : ITorusGenerationStrategy
    {
        public async Task<List<Torus>> Generate(TorusGenerationParameters parameters)
        {
            return new List<Torus>();
        }
    }
}
