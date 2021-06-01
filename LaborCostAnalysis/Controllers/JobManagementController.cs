using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Models;
using LaborCostAnalysis.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Controllers
{
    public class JobManagementController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        IConnectDB DB;

        public JobManagementController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetJobs()
        {
            List<JobModel> jobs = new List<JobModel>();
            this.DB = new ConnectDB();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Job_ID," +
                                    "Job_Number, " +
                                    "Job_Name, " +
                                    "Estimated_Budget, " +
                                    "Job_Year " +
                                    "from Job";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    JobModel job = new JobModel()
                    {
                        job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "",
                        job_number = dr["Job_Number"] != DBNull.Value ? dr["Job_Number"].ToString() : "",
                        job_name = dr["Job_Name"] != DBNull.Value ? dr["Job_Name"].ToString() : "",
                        estimated_budget = dr["Estimated_Budget"] != DBNull.Value ? Convert.ToInt32(dr["Estimated_Budget"]) : 0,
                        job_year = dr["Job_Year"] != DBNull.Value ? Convert.ToInt32(dr["Job_Year"]) : 0,
                    };
                    jobs.Add(job);
                }
                dr.Close();
            }
            return Json(jobs);
        }

        [HttpPost]
        public JsonResult AddJob(string number, string name, int year)
        {
            this.DB = new ConnectDB();
            SqlConnection con = DB.Connect();
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Job(Job_ID, Job_Number, Job_Name, Job_Year) " +
                                                   "VALUES(@Job_ID, @Job_Number, @Job_Name, @Job_Year)", con))
            {
                con.Open();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Job_Number", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Job_Name", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Job_Year", SqlDbType.Int);
                cmd.Parameters[0].Value = number.Replace("-", String.Empty).Replace(" ", String.Empty);
                cmd.Parameters[1].Value = number;
                cmd.Parameters[2].Value = name;
                cmd.Parameters[3].Value = year;
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return Json("Done");
        }

        public List<string> GetJobID()
        {
            List<string> job_ids = new List<string>();
            this.DB = new ConnectDB();
            SqlConnection con = DB.Connect();
            con.Open();
            string str_cmd = "select Job_ID from Job";
            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string id = dr["Job_ID"].ToString().Trim();
                    job_ids.Add(id);
                }
                dr.Close();
            }
            con.Close();
            return job_ids;
        }
    }
}
