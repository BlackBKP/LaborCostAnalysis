using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Models
{
    public class NormalOvertimeModel
    {
        public string job_id { get; set; }
        public double normal { get; set; }
        public double overtime { get; set; }
    }
}
