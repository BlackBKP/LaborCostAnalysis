using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Models
{
    public class JobSummaryModel
    {
        public string job_id { get; set; }

        public int estimated_budget { get; set; }

        public int labor_cost { get; set; }

        public int ot_labor_cost { get; set; }

        public int accomodation_cost { get; set; }

        public int compensation_cost { get; set; }

        public int social_security { get; set; }

        public int cost_to_date { get; set; }

        public int cost_usage { get; set; }

        public int remainning_cost { get; set; }

        public int work_completion { get; set; }

        public double hours { get; set; }

        public double ot_1_5 { get; set; }

        public double ot_3 { get; set; }

        public double total_man_hour { get; set; }

        public int no_of_labor { get; set; }

        public double avg_labor_cost_per_hour { get; set; }

    }
}
