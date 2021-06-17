using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;
using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Services;

namespace LaborCostAnalysis.Controllers
{
    public class LoginController : Controller
    {
        IUser UserInterface;
        string User = "";
        byte[] image = null;

        public LoginController()
        {
            this.UserInterface = new UserService();
        }

        public IActionResult Index()
        {
            return View();
        }

        public bool ActiveDirectoryAuthenticate(string username, string password)
        {
            bool userOk = false;
            try
            {
                using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://192.168.15.1", username, password))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(directoryEntry))
                    {
                        searcher.Filter = "(samaccountname=" + username + ")";
                        searcher.PropertiesToLoad.Add("displayname");
                        searcher.PropertiesToLoad.Add("thumbnailPhoto");

                        SearchResult adsSearchResult = searcher.FindOne();

                        if (adsSearchResult != null)
                        {
                            if (adsSearchResult.Properties["displayname"].Count == 1)
                            {
                                User = (string)adsSearchResult.Properties["displayname"][0];
                                image = adsSearchResult.Properties["thumbnailPhoto"][0] as byte[];
                            }
                            userOk = true;
                            HttpContext.Session.SetString("UserID", User);
                            HttpContext.Session.Set("Image", image);
                        }
                        return userOk;
                    }
                }
            }
            catch
            {
                return userOk;
            }
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if(ActiveDirectoryAuthenticate(username, password))
            {
                HttpContext.Session.SetString("LoginStatus", "LoggedIn");
                UserAuthenticationModel ua = UserInterface.GetUserAuthentication(User);
                if (ua.permission == "Admin")
                    HttpContext.Session.SetString("Admin", "True");
                else
                    HttpContext.Session.SetString("Admin", "False");
                return RedirectToAction("Index", "Home");
            }
            else
                return View("Index");
        }
    }
}
