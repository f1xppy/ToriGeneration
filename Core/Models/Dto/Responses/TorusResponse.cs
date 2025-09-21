using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Models.Dto.Geometry;

namespace ToriGeneration.Core.Models.Dto.Responses
{
    public class TorusResponse
    {
        public required Point Center { get; set; }

        public required double MajorRadius { get; set; }

        public required double MinorRadius { get; set; }

        public required Point Rotation { get; set; }
    }
}
