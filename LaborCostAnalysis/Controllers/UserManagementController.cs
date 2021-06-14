﻿using LaborCostAnalysis.Interfaces;
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
    public class UserManagementController : Controller
    {
        IUser UserInterface;

        public UserManagementController()
        {
            this.UserInterface = new UserService();
        }

        public IActionResult Index()
        {
            return HttpContext.Session.GetString("LoginStatus") == "LoggedIn" ? View() : (IActionResult)RedirectToAction("Index", "Login");
        }

        [HttpGet]

        public JsonResult GetUsers()
        {
            List<UserAuthenticationModel> users = UserInterface.GetUsers();
            return Json(users);
        }

        [HttpPost]
        public JsonResult AddUser(string id, string name, string role)
        {
            var result = UserInterface.AddUser(id,name,role);
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetJobAccessibility(string user_id)
        {
            var result = UserInterface.GetJobAccessibility(user_id);
            return Json(result);
        }

        [HttpPost]
        public JsonResult EditUser(string id, string name, string role)
        {
            var result = UserInterface.EditUser(id, name, role);
            return Json(result);
        }
    }
}
