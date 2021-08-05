using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Models
{
    public class LaborCostModel
    {
        public string job_id { get; set; }
        public string job_name { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int week { get; set; }
        public string week_time { get; set; }
        public int labor_cost { get; set; }
        public int ot_cost { get; set; }
        public int accomodate { get; set; }
        public int compensate { get; set; }
        public int social_security { get; set; }
        public int number_of_labor { get; set; }
    }
}
