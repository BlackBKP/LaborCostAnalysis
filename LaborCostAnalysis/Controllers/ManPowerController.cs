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
    public class ManPowerController : Controller
    {
        IManpower Manpower;
        IUser UserInterface;

        public ManPowerController()
        {
            this.Manpower = new ManpowerService();
            this.UserInterface = new UserService();
        }

        public IActionResult Index()
        {
            return HttpContext.Session.GetString("LoginStatus") == "LoggedIn" ? View() : (IActionResult)RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public JsonResult GetData(string year)
        {
            string user_name = HttpContext.Session.GetString("UserID");
            UserAuthenticationModel ua = UserInterface.GetUserAuthentication(user_name);
            List<List<ManpowerModel>> lmphs = new List<List<ManpowerModel>>();
            if(ua.permission == "Admin")
            {
                lmphs = (year == "ALL") ? Manpower.GetMPHModels() : Manpower.GetMPHModelsByYear(year);
            }
            else
            {
                lmphs = (year == "ALL") ? Manpower.GetManpowerByUser(user_name) : Manpower.GetManpowerByUser(user_name,year);
            }
            return Json(lmphs);
        }
    }
}
