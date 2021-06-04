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
    public class SpentController : Controller
    {
        ISpentPerWeek SPW;

        public SpentController()
        {
            this.SPW = new SpentPerWeekService();
        }

        public IActionResult Index()
        {
            return HttpContext.Session.GetString("LoginStatus") == "LoggedIn" ? View() : (IActionResult)RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public JsonResult GetSpentCostPerWeeks()
        {
            List<List<SpentPerWeekModel>> projects = new List<List<SpentPerWeekModel>>();
            try
            {
                List<SpentPerWeekModel> spws = SPW.GetSpentCostPerWeeks();
                string[] job_id = spws.OrderByDescending(o => o.job_id).Select(s => s.job_id).Distinct().ToArray();
                for (int i = 0; i < job_id.Count(); i++)
                {
                    projects.Add(spws.Where(w => w.job_id == job_id[i]).Select(s => s).OrderBy(o => o.year).ThenBy(t => t.month).ThenBy(tt => tt.week).ToList());
                }
                return Json(projects);
            }
            catch(Exception ex)
            {
                return Json(ex);
            }
        }
    }
}
