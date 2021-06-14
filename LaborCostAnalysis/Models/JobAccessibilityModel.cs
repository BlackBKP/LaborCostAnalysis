using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Models
{
    public class JobAccessibilityModel
    {
        public string job_id { get; set; }

        public string job_number { get; set; }

        public string job_name { get; set; }

        public int job_available { get; set; }
    }
}
