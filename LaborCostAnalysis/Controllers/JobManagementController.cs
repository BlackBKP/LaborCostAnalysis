using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Models;
using LaborCostAnalysis.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Controllers
{
    public class JobManagementController : Controller
    {
        IJob JobInterface;

        public JobManagementController()
        {
            this.JobInterface = new JobService();
        }

        public IActionResult Index()
        {
            return HttpContext.Session.GetString("LoginStatus") == "LoggedIn" ? View() : (IActionResult)RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public JsonResult GetJobs()
        {
            List<JobModel> jobs = JobInterface.GetJobs();
            return Json(jobs);
        }

        [HttpPost]
        public JsonResult EditJobName(string job_number, string job_name, string job_type, string job_budget)
        {
            JobInterface.UpdateJobName(job_number, job_name);
            JobInterface.UpdateJobType(job_number, job_type);
            JobInterface.UpdateJobBudget(job_number, Convert.ToInt32(job_budget.Replace(",", String.Empty)));
            return Json("Done");
        }

        [HttpPost]
        public JsonResult AddJob(string number, string name, int year, string type)
        {
            JobInterface.AddJob(number, name, year, type);
            return Json("Done");
        }
    }
}
