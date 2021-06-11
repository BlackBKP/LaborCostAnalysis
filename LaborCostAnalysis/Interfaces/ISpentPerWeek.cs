using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Interfaces
{
    interface ISpentPerWeek
    {
        List<SpentPerWeekModel> GetSpentCostPerWeeks();
        List<SpentPerWeekModel> GetSpentCostPerWeeks(string year);
        List<SpentPerWeekModel> GetSpentPerWeeksByJob(string job_id);
    }
}
