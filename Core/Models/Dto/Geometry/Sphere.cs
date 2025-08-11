using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToriGeneration.Core.Models.Dto.Geometry
{
    /// <summary>
    /// Сфера
    /// </summary>
    public class Sphere
    {
        public Point Center { get; set; }

        public double Radius { get; set; }
    }
}
