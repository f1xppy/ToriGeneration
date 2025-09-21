using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ToriGeneration.Core.Models.Dto;
using ToriGeneration.Core.Models.Dto.Geometry;
using ToriGeneration.Core.Models.Dto.Parameters;

namespace ToriGeneration.Core.Extensions.Geometry
{
    public static class CubeExtensions
    {
        public static bool Intersects(this Cube node, Sphere sphere)
        {
            return Math.Abs(sphere.Center.X - node.Center.X) + sphere.Radius >= node.Edge / 2 ||
                   Math.Abs(sphere.Center.Y - node.Center.Y) + sphere.Radius >= node.Edge / 2 ||
                   Math.Abs(sphere.Center.Z - node.Center.Z) + sphere.Radius >= node.Edge / 2;
        }

        public static bool Contains (this Cube node, Sphere sphere)
        {
            return Math.Abs(sphere.Center.X - node.Center.X) + sphere.Radius < node.Edge / 2 &&
                   Math.Abs(sphere.Center.Y - node.Center.Y) + sphere.Radius < node.Edge / 2 &&
                   Math.Abs(sphere.Center.Z - node.Center.Z) + sphere.Radius < node.Edge / 2;
        }

        public static bool ContainsCenter(this Cube node, Sphere sphere)
        {
            return Math.Abs(sphere.Center.X - node.Center.X) < node.Edge / 2 &&
                   Math.Abs(sphere.Center.Y - node.Center.Y) < node.Edge / 2 &&
                   Math.Abs(sphere.Center.Z - node.Center.Z) < node.Edge / 2;
        }

        public static bool NeedCheck(this Cube node, Sphere sphere)
        {
            return Math.Abs(sphere.Center.X - node.Center.X) < node.Edge / 2 + sphere.Radius ||
                   Math.Abs(sphere.Center.Y - node.Center.Y) < node.Edge / 2 + sphere.Radius ||
                   Math.Abs(sphere.Center.Z - node.Center.Z) < node.Edge / 2 + sphere.Radius;
        }

        public static bool SpheresIntersectsWith(this Cube node, Sphere sphere)
        {
            foreach (Sphere other in node.Spheres)
            {
                if (sphere.IntersectsWith(other))
                    return true;
            }

            if (!node.IsLeaf)
            {
                foreach (var child in node.Children)
                {
                    if (child.NeedCheck(sphere))
                        if (child.SpheresIntersectsWith(sphere))
                            return true;
                }
            }

            return false;
        }

        public static void Subdivide(this Cube node)
        {
            if (node.NodeDepth == 1) return;

            node.IsLeaf = false;
            var centerOffset = node.Edge / 4;

            for (int i = 0; i < 8; i++)
            {
                node.Children.Add(new Cube
                {
                    Edge = node.Edge / 2,
                    Center = new Point
                    {
                        X = node.Center.X + ((i & 1) != 0 ? centerOffset : -centerOffset),
                        Y = node.Center.Y + ((i & 2) != 0 ? centerOffset : -centerOffset),
                        Z = node.Center.Z + ((i & 4) != 0 ? centerOffset : -centerOffset)
                    },
                    NodeDepth = node.NodeDepth - 1,
                    MaxSpheresCount = node.MaxSpheresCount,
                    IsLeaf = true,
                    Children = [],
                    Spheres = []
                });
            }

            // Собираем сферы для перемещения
            var spheresToMove = new List<Sphere>();
            foreach (var sphere in node.Spheres)
            {
                foreach (var child in node.Children)
                {
                    if (child.Contains(sphere))
                    {
                        spheresToMove.Add(sphere);
                        break;
                    }
                }
            }

            // Перемещаем сферы в children
            foreach (var sphere in spheresToMove)
            {
                node.Spheres.Remove(sphere);
                foreach (var child in node.Children)
                {
                    if (child.Contains(sphere))
                    {
                        child.Spheres.Add(sphere);
                    }
                }
            }
        }

        public static void InitializeNodeParameters (this Cube node, TorusGenerationParameters parameters)
        {
            node.MaxSpheresCount = 30;

            var maxTorusMajorRadius = parameters.MaxTorusRadius - parameters.TorusThicknessCoefficient * parameters.MaxTorusRadius;
            var maxTorusMinorRadius = parameters.TorusThicknessCoefficient * parameters.MaxTorusRadius;

            var maxSpheresPerTorus = (int)Math.Ceiling(2 * Math.PI * maxTorusMajorRadius / maxTorusMinorRadius * 1.5);

            var maxSpheresCount = maxSpheresPerTorus * parameters.TargetTorusCount;

            var depth = 1;
            while (true)
            {
                int maxPerNode = (int)Math.Ceiling((double)maxSpheresCount / Math.Pow(8, depth));
                if (maxPerNode <= node.MaxSpheresCount)
                    break;

                depth++;
            }

            node.NodeDepth = depth + 1;

            node.Center = new Point
            {
                X = 0,
                Y = 0,
                Z = 0
            };
            node.Edge = parameters.CubeEdge;
            node.Spheres = [];
            node.Children = [];
            node.IsLeaf = true;
        }

        public static void Insert(this Cube node, Sphere sphere)
        {
            if (node.IsLeaf && (node.Spheres.Count < node.MaxSpheresCount))
            {
                node.Spheres.Add(sphere);
                return;
            }

            if (node.IsLeaf)
            {
                node.Subdivide();
            }

            foreach (var child in node.Children)
            {
                if (child.Contains(sphere))
                {
                    child.Insert(sphere);
                    return;
                }
            }

            // Если сфера перекрывает несколько подузлов, она остаётся в текущем узле
            node.Spheres.Add(sphere);
        }
    }
}
