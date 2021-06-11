using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Models
{
    public class ProgressModel
    {
        public string job_id { get; set; }

        public string job_number { get; set; }

        public string job_name { get; set; }

        public int year { get; set; }

        public int month { get; set; }

        public int estimated_budget { get; set; }

        public int job_progress { get; set; }

        public int spent_cost { get; set; }

        public int spent_cost_acc { get; set; }

        public int remainning_cost { get; set; }
    }
}
