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
        IUser UserInterface;

        public SpentController()
        {
            this.SPW = new SpentPerWeekService();
            this.UserInterface = new UserService();
        }

        public IActionResult Index()
        {
            return HttpContext.Session.GetString("LoginStatus") == "LoggedIn" ? View() : (IActionResult)RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public JsonResult GetSpentCostPerWeeks(string year)
        {
            List<List<SpentPerWeekModel>> projects = new List<List<SpentPerWeekModel>>();
            try
            {
                string user_name = HttpContext.Session.GetString("UserID");
                UserAuthenticationModel ua = UserInterface.GetUserAuthentication(user_name);
                List<SpentPerWeekModel> spws = new List<SpentPerWeekModel>();
                if (ua.permission == "Admin" || ua.permission == "Human Resource" || ua.permission == "Accounting")
                {
                    spws = (year == "ALL") ? SPW.GetSpentCostPerWeeks() : SPW.GetSpentCostPerWeeks(year);
                    string[] job_id = spws.OrderByDescending(o => o.job_id).Select(s => s.job_id).Distinct().ToArray();
                    for (int i = 0; i < job_id.Count(); i++)
                    {
                        projects.Add(spws.Where(w => w.job_id == job_id[i]).Select(s => s).OrderBy(o => o.year).ThenBy(t => t.month).ThenBy(tt => tt.week).ToList());
                    }
                }
                else
                {
                    spws = (year == "ALL") ? SPW.GetSpentPerWeeksByUser(user_name) : SPW.GetSpentPerWeeksByUser(user_name,year);
                    string[] job_id = spws.OrderByDescending(o => o.job_id).Select(s => s.job_id).Distinct().ToArray();
                    for (int i = 0; i < job_id.Count(); i++)
                    {
                        projects.Add(spws.Where(w => w.job_id == job_id[i]).Select(s => s).OrderBy(o => o.year).ThenBy(t => t.month).ThenBy(tt => tt.week).ToList());
                    }
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
