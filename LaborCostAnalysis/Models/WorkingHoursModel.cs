using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Models
{
    public class WorkingHoursModel
    {
        public string job_id { get; set; }
        public string employee_id { get; set; }
        public DateTime working_day { get; set; }
        public int week { get; set; }
        public int month { get; set; }
        public int hours { get; set; }
    }
}
