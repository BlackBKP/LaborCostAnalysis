using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Models
{
    public class ManpowerModel
    {
        public string job_id { get; set; }

        public int year { get; set; }

        public int week { get; set; }

        public int month { get; set; }

        public double normal { get; set; }

        public double ot_1_5 { get; set; }

        public double ot_3 { get; set; }

        public double acc_hour { get; set; }

    }
}