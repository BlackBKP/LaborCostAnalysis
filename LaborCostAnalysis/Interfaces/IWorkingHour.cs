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

        List<WorkingHoursModel> GetWorkingHours(string job_id, string year, int month, int week);

        string InsertWorkingHours(List<WorkingHoursModel> whs);

        string DeleteWorkingHours(string job_id, string year, int month, int week);
    }
}
