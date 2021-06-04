using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Interfaces
{
    interface IProgress
    {
        List<JobSummaryModel> GetJobProgress(string job_id);

        List<ProgressModel> GetProgressViewModels();

        string InsertProgress(List<ProgressModel> progress);

        string UpdateProgress(ProgressModel progress);
    }
}
