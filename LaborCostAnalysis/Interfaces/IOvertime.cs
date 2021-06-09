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

        List<OvertimeModel> GetOvertimes(string job_id, string year, int month, int week);

        string InsertOvertimes(List<OvertimeModel> ots);

        string DeleteOvertimes(string job_id, string year, int month, int week);
    }
}
