using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Models.Enums;

namespace ToriGeneration.Core.Models.Dto.Parameters
{
    public class TorusGenerationParameters
    {
        public double CubeEdge { get; set; }

        public double MaxTorusRadius { get; set; }

        public double MinTorusRadius { get; set; }

        public double TorusThicknessCoefficient { get; set; }

        public int TargetTorusCount { get; set; }

        public double? Concentration { get; set; }

        public GenerationMethod GenerationType { get; set; } 
    }
}
