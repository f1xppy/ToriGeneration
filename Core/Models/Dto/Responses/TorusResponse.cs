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
        public Point Center { get; set; }

        public double MajorRadius { get; set; }

        public double MinorRadius { get; set; }

        public Point Rotation { get; set; }
    }
}
