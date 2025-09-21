using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToriGeneration.Core.Models.Dto.Geometry
{
    public class Cube
    {
        public double Edge {  get; set; }

        public required Point Center { get; set; }

        public required List<Sphere> Spheres { get; set; }

        public required List<Cube> Children { get; set; }

        public bool IsLeaf { get; set; }

        public int NodeDepth { get; set; }

        public int MaxSpheresCount { get; set; }
    }
}
