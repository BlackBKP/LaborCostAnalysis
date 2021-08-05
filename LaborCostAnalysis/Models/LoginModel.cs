using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Models
{
    public static class LoggedinUser
    {
        public static LoginModel Logged { get; set; } = new LoginModel();
    }

    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public byte[] user_image { get; set; }
    }
}
