using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;
using ToriGeneration.Core.Models.Dto.Responses;

namespace ToriGeneration.Core.Abstract.Generators
{
    public interface ITorusListGenerator
    {
        Task<TorusListResponse> GenerateTorusList(TorusGenerationParameters parameters, Cube rootNode);
    }
}
