using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Models
{
    public class SpentPerWeekModel
    {
        public string job_id { get; set; }
        public string job_name { get; set; }

        public int week { get; set; }

        public int month { get; set; }

        public int year { get; set; }

        public int budget100 { get; set; }

        public int budget80 { get; set; }

        public int budget70 { get; set; }

        public int budget50 { get; set; }

        public int progress { get; set; }

        public int spent_cost { get; set; }

        public int acc_cost { get; set; }
    }
}
