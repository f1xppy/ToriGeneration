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
        public required Point Center { get; set; }

        public required double Radius { get; set; }
    }
}
