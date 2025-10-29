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
            var halfEdge = node.Edge / 2;

            return Math.Abs(sphere.Center.X - node.Center.X) + sphere.Radius > halfEdge ||
                   Math.Abs(sphere.Center.Y - node.Center.Y) + sphere.Radius > halfEdge ||
                   Math.Abs(sphere.Center.Z - node.Center.Z) + sphere.Radius > halfEdge;
        }

        public static bool Contains (this Cube node, Sphere sphere)
        {
            return Math.Abs(sphere.Center.X - node.Center.X) + sphere.Radius < node.Edge / 2 &&
                   Math.Abs(sphere.Center.Y - node.Center.Y) + sphere.Radius < node.Edge / 2 &&
                   Math.Abs(sphere.Center.Z - node.Center.Z) + sphere.Radius < node.Edge / 2;
        }

        public static bool NeedCheck(this Cube node, Sphere sphere)
        {
            var sphereMinX = sphere.Center.X - sphere.Radius;
            var sphereMaxX = sphere.Center.X + sphere.Radius;
            var sphereMinY = sphere.Center.Y - sphere.Radius;
            var sphereMaxY = sphere.Center.Y + sphere.Radius;
            var sphereMinZ = sphere.Center.Z - sphere.Radius;
            var sphereMaxZ = sphere.Center.Z + sphere.Radius;

            return !(sphereMaxX < node.MinX || sphereMinX > node.MaxX ||
                     sphereMaxY < node.MinY || sphereMinY > node.MaxY ||
                     sphereMaxZ < node.MinZ || sphereMinZ > node.MaxZ);
        }

        public static bool SpheresIntersectsWith(this Cube node, Sphere sphere)
        {
            var stack = new Stack<Cube>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();

                if (currentNode.Spheres != null && currentNode.Spheres.Count > 0)
                {
                    foreach (var other in currentNode.Spheres)
                    {
                        if (sphere.IntersectsWith(other))
                            return true;
                    }
                }

                if (!currentNode.IsLeaf && currentNode.Children != null)
                {
                    for (int i = currentNode.Children.Count - 1; i >= 0; i--)
                    {
                        var child = currentNode.Children[i];
                        if (child.NeedCheck(sphere))
                        {
                            stack.Push(child);
                        }
                    }
                }
            }

            return false;
        }

        public static void Subdivide(this Cube node, List<Sphere> newSpheres)
        {
            if (node.NodeDepth == 1) return;

            node.IsLeaf = false;
            var centerOffset = node.Edge / 4;

            node.Children = new List<Cube>(8);
            for (int i = 0; i < 8; i++)
            {
                var child = new Cube
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
                    Children = new List<Cube>(),
                    Spheres = new List<Sphere>()
                };

                child.ComputeBounds();

                node.Children.Add(child);
            }

            var allSpheres = new List<Sphere>();
            if (node.Spheres != null && node.Spheres.Count > 0)
                allSpheres.AddRange(node.Spheres);
            if (newSpheres != null && newSpheres.Count > 0)
                allSpheres.AddRange(newSpheres);

            node.Spheres?.Clear();

            DistributeSpheres(node, allSpheres);
        }

        public static void InitializeNodeParameters (this Cube node, TorusGenerationParameters parameters)
        {
            node.MaxSpheresCount = 10;

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

        public static void InsertBatch(this Cube node, List<Sphere> spheres)
        {
            if (spheres == null || spheres.Count == 0)
                return;

            var queue = new Queue<(Cube node, List<Sphere> spheres)>();
            queue.Enqueue((node, spheres));

            while (queue.Count > 0)
            {
                var (currentNode, currentSpheres) = queue.Dequeue();

                if (currentNode.IsLeaf &&
                    currentNode.Spheres.Count + currentSpheres.Count <= currentNode.MaxSpheresCount)
                {
                    currentNode.Spheres.AddRange(currentSpheres);
                    continue;
                }

                if (currentNode.IsLeaf)
                {
                    currentNode.Subdivide(currentSpheres);
                }
                else
                {
                    DistributeSpheresToChildren(currentNode, currentSpheres, queue);
                }
            }
        }

        private static void DistributeSpheres(Cube node, List<Sphere> spheres)
        {
            var childSpheres = new List<Sphere>[8];
            for (int i = 0; i < 8; i++)
                childSpheres[i] = new List<Sphere>();

            var remainingSpheres = new List<Sphere>();

            foreach (var sphere in spheres)
            {
                bool placedInChild = false;

                for (int i = 0; i < 8; i++)
                {
                    if (node.Children[i].Contains(sphere))
                    {
                        childSpheres[i].Add(sphere);
                        placedInChild = true;
                        break;
                    }
                }

                if (!placedInChild)
                    remainingSpheres.Add(sphere);
            }

            for (int i = 0; i < 8; i++)
            {
                if (childSpheres[i].Count > 0)
                {
                    node.Children[i].Spheres.AddRange(childSpheres[i]);
                }
            }

            if (remainingSpheres.Count > 0)
            {
                node.Spheres.AddRange(remainingSpheres);
            }
        }

        private static void DistributeSpheresToChildren(Cube node, List<Sphere> spheres,
    Queue<(Cube node, List<Sphere> spheres)> queue)
        {
            var childSpheres = new List<Sphere>[8];
            for (int i = 0; i < 8; i++)
                childSpheres[i] = new List<Sphere>();

            var remainingSpheres = new List<Sphere>();

            foreach (var sphere in spheres)
            {
                bool placedInChild = false;

                for (int i = 0; i < 8; i++)
                {
                    if (node.Children[i].Contains(sphere))
                    {
                        childSpheres[i].Add(sphere);
                        placedInChild = true;
                        break;
                    }
                }

                if (!placedInChild)
                    remainingSpheres.Add(sphere);
            }

            for (int i = 0; i < 8; i++)
            {
                if (childSpheres[i].Count > 0)
                {
                    queue.Enqueue((node.Children[i], childSpheres[i]));
                }
            }

            if (remainingSpheres.Count > 0)
            {
                node.Spheres.AddRange(remainingSpheres);
            }
        }

        public static bool CheckIntersectionsParallel(this Cube rootNode, List<Sphere> spheres, int batchSize)
        {
            var hasIntersections = false;
            var totalSpheres = spheres.Count;

            Parallel.For(0, (int)Math.Ceiling((double)totalSpheres / batchSize), (batchIndex, state) =>
            {
                var start = batchIndex * batchSize;
                var end = Math.Min(start + batchSize, totalSpheres);

                for (int i = start; i < end; i++)
                {
                    if (hasIntersections)
                    {
                        state.Stop();
                        return;
                    }

                    var sphere = spheres[i];

                    if (rootNode.Intersects(sphere))
                    {
                        hasIntersections = true;
                        state.Stop();
                        return;
                    }

                    if (rootNode.SpheresIntersectsWith(sphere))
                    {
                        hasIntersections = true;
                        state.Stop();
                        return;
                    }
                }
            });

            return hasIntersections;
        }

        public static List<Torus> CheckBatchIntersections(this Cube rootNode, List<Torus> torusBatch)
        {
            var validTorusList = new List<Torus>();
            var spheresCache = new List<Sphere>[torusBatch.Count];

            Parallel.For(0, torusBatch.Count, i =>
            {
                spheresCache[i] = torusBatch[i].Spheres.ToList();
            });

            Parallel.For(0, torusBatch.Count, i =>
            {
                var torus = torusBatch[i];
                var spheres = spheresCache[i];

                if (!rootNode.CheckIntersectionsParallel(spheres, 50))
                {
                    lock (validTorusList)
                    {
                        validTorusList.Add(torus);
                    }
                }
            });

            return validTorusList;
        }
    }
}
