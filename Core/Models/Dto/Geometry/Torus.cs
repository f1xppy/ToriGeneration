using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToriGeneration.Core.Models.Dto.Geometry
{
    public class Torus
    {
        public Point Center { get; set; }

        public double MajorRadius { get; set; }

        public double MinorRadius { get; set; }

        public Point Rotation { get; set; }

        public List<Sphere> Spheres { get; set; }
    }
}
