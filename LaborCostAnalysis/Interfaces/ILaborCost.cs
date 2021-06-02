using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Interfaces
{
    interface ILaborCost
    {
        List<LaborCostModel> GetLaborCosts();

        string InsertLaborCosts(List<LaborCostModel> labor_costs);
    }
}
