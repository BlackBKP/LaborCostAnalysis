using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Models
{
    public class UserAuthenticationModel
    {
        public string user_id { get; set; }

        public string user_name { get; set; }

        public string permission { get; set; }
    }
}
