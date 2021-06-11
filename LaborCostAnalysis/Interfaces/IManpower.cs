using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Interfaces
{
    interface IManpower
    {
        List<List<ManpowerModel>> GetMPHModels();
        List<List<ManpowerModel>> GetMPHModels(string job_id);
        List<List<ManpowerModel>> GetMPHModelsByYear(string year);
        List<ManpowerModel> GetManpower(string job_id);
    }
}
