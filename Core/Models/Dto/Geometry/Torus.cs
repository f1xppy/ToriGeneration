using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToriGeneration.Core.Models.Dto.Geometry
{
    public class Torus
    {
        public required Point Center { get; set; }

        public required double MajorRadius { get; set; }

        public required double MinorRadius { get; set; }

        public required Point Rotation { get; set; }

        public required List<Sphere> Spheres { get; set; }
    }
}
