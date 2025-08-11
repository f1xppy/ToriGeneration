using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToriGeneration.Core.Models.Dto.Geometry
{
    public class Cube
    {
        public double Edge {  get; set; }

        public Point Center { get; set; }

        public List<Sphere> Spheres { get; set; }
    }
}
