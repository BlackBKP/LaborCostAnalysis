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

        List<JobModel> GetJobsWithAuthentication(string user_id);

        string UpdateJobName(string job_number, string job_name);

        string UpdateJobBudget(string job_number, int job_budget);

        string UpdateJobType(string job_number, string job_type);

        string AddJob(string job_number, string job_name, int job_year, string job_type);

        string InsertJobs(List<JobModel> import_jobs);
    }
}
