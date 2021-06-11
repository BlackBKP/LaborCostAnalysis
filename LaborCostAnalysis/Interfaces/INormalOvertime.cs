using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Interfaces
{
    interface INormalOvertime
    {
        List<NormalOvertimeModel> NormalPerOvertime();

        List<NormalOvertimeModel> NormalPerOvertime(string job_id);

        List<NormalOvertimeModel> NormalPerOvertimeByYear(string year);
    }
}
