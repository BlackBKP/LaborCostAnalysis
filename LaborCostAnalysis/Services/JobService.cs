using LaborCostAnalysis.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Services
{
    public class JobService :IJob
    {
        IConnectDB DB;

        public JobService()
        {
            this.DB = new ConnectDB();
        }
    }
}
