using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Interfaces
{
    interface IJobSummary
    {
        List<JobSummaryModel> GetJobsSummary();
        List<JobSummaryModel> GetJobsSummary(string user_name);
    }
}
