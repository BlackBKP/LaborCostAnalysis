using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Models;
using LaborCostAnalysis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Controllers
{
    public class JobSummaryController : Controller
    {
        IJobSummary JobSummary;
        IUser UserInterface;

        public JobSummaryController()
        {
            this.JobSummary = new JobSummaryService();
            this.UserInterface = new UserService();
        }

        public IActionResult Index()
        {
            return HttpContext.Session.GetString("LoginStatus") == "LoggedIn" ? View() : (IActionResult)RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public JsonResult GetJobsSummary()
        {
            string user_name = HttpContext.Session.GetString("UserID");
            UserAuthenticationModel ua = UserInterface.GetUserAuthentication(user_name);
            List<JobSummaryModel> jobs = new List<JobSummaryModel>();
            if (ua.permission == "Admin" || ua.permission == "Human Resource" || ua.permission == "Accounting")
            {
                jobs = JobSummary.GetJobsSummary();
            }
            else
            {
                jobs = JobSummary.GetJobsSummary(user_name);
            }
            return Json(jobs);
        }
    }
}
