using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Models;
using LaborCostAnalysis.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Controllers
{
    public class HomeController : Controller
    {
        IJob JobInterface;
        IProgress ProgressInterface;
        ISpentPerWeek SpentInterface;
        IManpower ManpowerInterface;
        INormalOvertime NormalOvertimeInterface;

        public HomeController()
        {
            this.JobInterface = new JobService();
            this.ProgressInterface = new ProgressService();
            this.SpentInterface = new SpentPerWeekService();
            this.ManpowerInterface = new ManpowerService();
            this.NormalOvertimeInterface = new NormalOvertimeService();
        }

        [HttpGet]
        public JsonResult GetJobs()
        {
            List<JobModel> jobs = JobInterface.GetJobs();
            return Json(jobs);
        }

        [HttpGet]
        public JsonResult GetJobsByYear(string year)
        {
            List<JobModel> jobs = JobInterface.GetJobs();
            jobs = jobs.Where(w => w.job_id.Substring(1, 2) == year.Substring(2, 2)).Select(s => s).ToList();
            return Json(jobs);
        }

        [HttpGet]
        public JsonResult GetJobProgress(string job_id)
        {
            List<JobSummaryModel> jobs = ProgressInterface.GetJobProgress(job_id);
            return Json(jobs);
        }

        [HttpGet]
        public JsonResult GetHalfMonthSpent(string job_id)
        {
            List<SpentPerWeekModel> spws = SpentInterface.GetSpentPerWeeksByJob(job_id);
            return Json(spws);
        }

        [HttpGet]
        public JsonResult GetManPower(string job_id)
        {
            List<ManpowerModel> mphs = ManpowerInterface.GetManpower(job_id);
            return Json(mphs);
        }

        [HttpGet]
        public JsonResult GetNormalOvertimeRatio(string job_id)
        {
            List<NormalOvertimeModel> nprs = NormalOvertimeInterface.NormalPerOvertime(job_id);
            return Json(nprs);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
