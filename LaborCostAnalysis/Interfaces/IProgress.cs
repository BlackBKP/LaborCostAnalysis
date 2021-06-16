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

        List<ProgressModel> GetProgressViewModelsByUser(string user_name);

        string InsertProgress(List<ProgressModel> progress);

        string UpdateDuplicateProgress(List<ProgressModel> progress);

        string UpdateProgress(ProgressModel progress);
    }
}
