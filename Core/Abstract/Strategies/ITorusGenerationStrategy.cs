using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;

namespace ToriGeneration.Core.Abstract.Strategies
{
    public interface ITorusGenerationStrategy
    {
        List<Torus> Generate(ToriGenerationParameters parameters);
    }
}
