using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Interfaces
{
    interface IOvertime
    {
        List<OvertimeModel> GetOvertimes();

        List<OvertimeModel> GetOvertimes(string job_id);

        string InsertOvertimes(List<OvertimeModel> ots);
    }
}
