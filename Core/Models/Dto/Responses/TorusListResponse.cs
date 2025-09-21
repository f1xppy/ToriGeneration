using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToriGeneration.Core.Models.Dto.Responses
{
    public class TorusListResponse
    {
        public required List<TorusResponse> TorusList { get; set; }

        public required int TotalCount { get; set; }

        public required double Concentration { get; set; }

        public required TimeSpan ElapsedTime { get; set; } 
    }
}
