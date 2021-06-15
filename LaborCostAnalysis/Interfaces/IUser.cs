using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Interfaces
{
    interface IUser
    {
        List<UserAuthenticationModel> GetUsers();

        string AddUser(string user_id, string user_name, string role);

        string EditUser(string user_id, string user_name, string role);

        List<JobAccessibilityModel> GetJobAccessibility(string user_id);

        string AddJobAccessibility(string user_id, string job_id);

        string RemoveJobAccessibility(string user_id, string job_id);
    }
}
