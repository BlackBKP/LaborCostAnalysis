using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Interfaces
{
    interface IWorkingHour
    {
        List<WorkingHoursModel> GetWorkingHours();

        List<WorkingHoursModel> GetWorkingHours(string job_id);

        string InsertWorkingHours(List<WorkingHoursModel> whs);
    }
}
