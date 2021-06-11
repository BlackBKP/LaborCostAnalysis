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
    public class NormalOvertimeController : Controller
    {
        INormalOvertime NormalOvertime;

        public NormalOvertimeController()
        {
            this.NormalOvertime = new NormalOvertimeService();
        }

        public IActionResult Index()
        {
            return HttpContext.Session.GetString("LoginStatus") == "LoggedIn" ? View() : (IActionResult)RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public JsonResult GetData(string year)
        {
            List<NormalOvertimeModel> npero = (year == "ALL") ? NormalOvertime.NormalPerOvertime() : NormalOvertime.NormalPerOvertimeByYear(year);
            return Json(npero);
        }
    }
}
