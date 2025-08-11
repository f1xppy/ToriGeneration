using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;
using ToriGeneration.Core.Models.Enums;

namespace ToriGeneration.Core.Abstract.Services
{
    public interface ITorusGenerationService
    {
        Task<List<Torus>> GenerateAsync(ToriGenerationParameters parameters);
    }
}
