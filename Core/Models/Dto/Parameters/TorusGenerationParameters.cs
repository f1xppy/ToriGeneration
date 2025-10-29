using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Models.Enums;

namespace ToriGeneration.Core.Models.Dto.Parameters
{
    public class TorusGenerationParameters
    {
        [DefaultValue(100.0)]
        public double CubeEdge { get; set; }

        [DefaultValue(2.5)]
        public double MaxTorusRadius { get; set; }

        [DefaultValue(0.1)]
        public double MinTorusRadius { get; set; }

        [DefaultValue(0.25)]
        public double TorusThicknessCoefficient { get; set; }

        [DefaultValue(100000)]
        public int TargetTorusCount { get; set; }

        public double? Concentration { get; set; }

        [DefaultValue(1)]
        public GenerationMethod GenerationType { get; set; } 
    }
}
