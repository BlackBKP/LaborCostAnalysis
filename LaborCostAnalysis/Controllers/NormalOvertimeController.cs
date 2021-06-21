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
        IUser UserInterface;

        public NormalOvertimeController()
        {
            this.NormalOvertime = new NormalOvertimeService();
            this.UserInterface = new UserService();
        }

        public IActionResult Index()
        {
            return HttpContext.Session.GetString("LoginStatus") == "LoggedIn" ? View() : (IActionResult)RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public JsonResult GetData(string year)
        {
            List<NormalOvertimeModel> npero = new List<NormalOvertimeModel>();
            string user_name = HttpContext.Session.GetString("UserID");
            UserAuthenticationModel ua = UserInterface.GetUserAuthentication(user_name);
            if (ua.permission == "Admin" || ua.permission == "Human Resource" || ua.permission == "Accounting")
            {
                npero = (year == "ALL") ? NormalOvertime.NormalPerOvertime() : NormalOvertime.NormalPerOvertimeByYear(year);
            }
            else
            {
                npero = (year == "ALL") ? NormalOvertime.NormalPerOvertimeByUser(user_name) : NormalOvertime.NormalPerOvertimeByUser(user_name,year);
            }
            return Json(npero);
        }
    }
}
