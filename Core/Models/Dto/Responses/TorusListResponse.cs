using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToriGeneration.Core.Models.Dto.Responses
{
    public class TorusListResponse
    {
        public List<TorusResponse> TorusList { get; set; }

        public int TotalCount { get; set; }

        public double Concentration { get; set; }

        public string ElapsedTime { get; set; } 
    }
}
