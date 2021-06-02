using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Interfaces
{
    public interface IJob
    {
        List<JobModel> GetJobs();

        string AddJob(string job_number, string job_name, int job_year);

        string InsertJobs(List<JobModel> import_jobs);
    }
}
