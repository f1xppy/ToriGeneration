using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;
using ToriGeneration.Core.Models.Dto.Responses;

namespace ToriGeneration.Core.Abstract.Strategies
{
    public interface ITorusGenerationStrategy
    {
        Task<TorusListResponse> Generate(TorusGenerationParameters parameters);
    }
}
