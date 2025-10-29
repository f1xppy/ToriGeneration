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

        public double HalfEdge { get; set; } 

        public required Point Center { get; set; }

        public required List<Sphere> Spheres { get; set; }

        public required List<Cube> Children { get; set; }

        public bool IsLeaf { get; set; }

        public int NodeDepth { get; set; }

        public int MaxSpheresCount { get; set; }

        public double MinX { get; private set; }
        public double MaxX { get; private set; }
        public double MinY { get; private set; }
        public double MaxY { get; private set; }
        public double MinZ { get; private set; }
        public double MaxZ { get; private set; }

        public void ComputeBounds()
        {
            HalfEdge = Edge / 2;
            MinX = Center.X - HalfEdge;
            MaxX = Center.X + HalfEdge;
            MinY = Center.Y - HalfEdge;
            MaxY = Center.Y + HalfEdge;
            MinZ = Center.Z - HalfEdge;
            MaxZ = Center.Z + HalfEdge;
        }
    }
}
