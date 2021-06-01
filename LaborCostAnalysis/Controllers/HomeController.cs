using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Models;
using LaborCostAnalysis.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Controllers
{
    public class HomeController : Controller
    {
        IConnectDB DB;
        IJob JobInterface;

        public HomeController()
        {
            JobInterface = new JobService();
        }

        [HttpGet]
        public JsonResult GetJobs()
        {
            List<JobModel> jobs = JobInterface.GetJobs();
            return Json(jobs);
        }

        [HttpGet]
        public JsonResult GetJobsByYear(string year)
        {
            List<JobModel> jobs = JobInterface.GetJobs();
            jobs = jobs.Where(w => w.job_id.Substring(1, 2) == year.Substring(2, 2)).Select(s => s).ToList();
            return Json(jobs);
        }

        [HttpGet]
        public JsonResult GetJobProgress(string job_id)
        {
            this.DB = new ConnectDB();
            List<JobSummaryModel> jobs = new List<JobSummaryModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select job.Job_ID, " +
                                    "job.Estimated_Budget, " +
                                    "s1.Labor_Cost, " +
                                    "s1.OT_Labor_Cost, " +
                                    "s1.Accommodation_Cost, " +
                                    "s1.Compensation_Cost, " +
                                    "isnull(s1.Social_Security,0) as Social_Security, " +
                                    "s1.Cost_to_Date, (cast(job.Estimated_Budget as int) - cast(s1.Cost_to_Date as int)) as Remaining_Cost, " +
                                    "((cast(s1.Cost_to_Date as float) / cast(job.Estimated_Budget as float)) *100) as Cost_Usage, " +
                                    "s4.Last_Progress as Work_Completion, " +
                                    "s2.Hours, " +
                                    "s3.OT_1_5, " +
                                    "s3.OT_3, " +
                                    "(s2.Hours + s3.OT_1_5 + s3.OT_3) as Total_Man_Hour, " +
                                    "s5.No_Of_Labor_Week as No_Of_Labor, " +
                                    "(cast(s1.Cost_to_Date as float) / (s2.Hours + s3.OT_1_5 + s3.OT_3)) as avg_labor_cost_per_hour " +
                                    "from job " +
                                    "left join (select job_ID, SUM(cast(Labor_Cost as int))as Labor_Cost, SUM(cast(OT_Labor_Cost as int)) as OT_Labor_Cost, SUM(cast(Accommodation_Cost as int)) as Accommodation_Cost, SUM(cast(Compensation_Cost as int)) as Compensation_Cost, SUM(isnull(Social_Security,0)) as Social_Security, (SUM(cast(Labor_Cost as int)) + SUM(cast(OT_Labor_Cost as int)) + SUM(cast(Accommodation_Cost as int)) + SUM(cast(Compensation_Cost as int)) + SUM(isnull(Social_Security,0))) as Cost_to_Date from Labor_Costs group by job_ID) as s1 ON s1.job_ID = job.job_ID " +
                                    "left join (select job_ID,SUM(Hours) as Hours from Hour group by Job_ID) as s2 ON s2.job_ID = job.job_ID " +
                                    "left join (select job_ID,SUM(OT_1_5) as OT_1_5 , SUM(OT_3) as OT_3 from OT group by job_ID) as s3 ON s3.job_ID = job.job_ID " +
                                    "left join (select Job_ID,Max(cast(Job_Progress as int)) as Last_Progress from Progress group by Job_ID) as s4 ON s4.Job_ID = job.Job_ID " +
                                    "left join (select Job_ID,Max(cast(No_Of_Labor_Week as int)) as No_Of_Labor_Week from Labor_Costs group by Job_ID) as s5 ON s5.Job_ID = job.Job_ID " +
                                    "where job.Job_ID = '" + job_id + "'";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    JobSummaryModel job = new JobSummaryModel()
                    {
                        job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "",
                        estimated_budget = dr["Estimated_Budget"] != DBNull.Value ? Convert.ToInt32(dr["Estimated_Budget"]) : 0,
                        labor_cost = dr["Labor_Cost"] != DBNull.Value ? Convert.ToInt32(dr["Labor_Cost"]) : 0,
                        ot_labor_cost = dr["OT_Labor_Cost"] != DBNull.Value ? Convert.ToInt32(dr["OT_Labor_Cost"]) : 0,
                        accomodation_cost = dr["Accommodation_Cost"] != DBNull.Value ? Convert.ToInt32(dr["Accommodation_Cost"]) : 0,
                        compensation_cost = dr["Compensation_Cost"] != DBNull.Value ? Convert.ToInt32(dr["Compensation_Cost"]) : 0,
                        social_security = dr["Social_Security"] != DBNull.Value ? Convert.ToInt32(dr["Social_Security"]) : 0,
                        cost_to_date = dr["Cost_to_Date"] != DBNull.Value ? Convert.ToInt32(dr["Cost_to_Date"]) : 0,
                        remainning_cost = dr["Remaining_Cost"] != DBNull.Value ? Convert.ToInt32(dr["Remaining_Cost"]) : 0,
                        cost_usage = dr["Cost_Usage"] != DBNull.Value ? Convert.ToInt32(dr["Cost_Usage"]) : 0,
                        work_completion = dr["Work_Completion"] != DBNull.Value ? Convert.ToInt32(dr["Work_Completion"]) : 0,
                        hours = dr["Hours"] != DBNull.Value ? Convert.ToInt32(dr["Hours"]) : 0,
                        ot_1_5 = dr["OT_1_5"] != DBNull.Value ? Convert.ToInt32(dr["OT_1_5"]) : 0,
                        ot_3 = dr["OT_3"] != DBNull.Value ? Convert.ToInt32(dr["OT_3"]) : 0,
                        total_man_hour = dr["Total_Man_Hour"] != DBNull.Value ? Convert.ToInt32(dr["Total_Man_Hour"]) : 0,
                        no_of_labor = dr["No_Of_Labor"] != DBNull.Value ? Convert.ToInt32(dr["No_Of_Labor"]) : 0,
                        avg_labor_cost_per_hour = dr["avg_labor_cost_per_hour"] != DBNull.Value ? Convert.ToInt32(dr["avg_labor_cost_per_hour"]) : 0,
                    };
                    jobs.Add(job);
                }
                dr.Close();
            }
            con.Close();
            return Json(jobs);
        }

        [HttpGet]
        public JsonResult GetHalfMonthSpent(string job_id)
        {
            List<SpentPerWeekModel> spws = new List<SpentPerWeekModel>();
            this.DB = new ConnectDB();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Labor_Costs.job_ID, " +
                                    "SUM((cast(Labor_Cost as int) + cast(OT_Labor_Cost as int) + cast(Accommodation_Cost as int) + cast(Compensation_Cost as int)) + isnull(Social_Security,0)) OVER(PARTITION BY Labor_Costs.job_ID ORDER BY Labor_Costs.job_ID ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) as Acc_Cost, " +
                                    "week, " +
                                    "Labor_Costs.Month, " +
                                    "Labor_Costs.Year, " +
                                    "(s1.Estimated_Budget * 1.0) as Budget100, " +
                                    "(s1.Estimated_Budget * 0.8) as Budget80, " +
                                    "(s1.Estimated_Budget * 0.7) as Budget70, " +
                                    "(s1.Estimated_Budget * 0.5) as Budget50, " +
                                    "((cast(s2.Job_Progress as int) +  lag(s2.Job_Progress,1,s2.Job_Progress * -1) over (partition by s2.Job_ID order by s2.Job_ID))/2.0) as Progress, " +
                                    "(cast(Labor_Cost as int) + cast(OT_Labor_Cost as int) + cast(Accommodation_Cost as int) + cast(Compensation_Cost as int) + isnull(Social_Security,0)) as spent_cost " +
                                    "from Labor_Costs " +
                                    "left join (select job_ID,isnull(Estimated_Budget,0) as Estimated_Budget from job) as s1 ON s1.job_ID = Labor_Costs.job_ID " +
                                    "left join (select Job_ID,Job_Progress,Month,Year from Progress) as s2 ON s2.Job_ID = Labor_Costs.job_ID and s2.Year = Labor_Costs.Year and s2.Month = Labor_Costs.Month " +
                                    "where Labor_Costs.job_ID = '" + job_id + "' " +
                                    "order by Labor_Costs.job_ID,Labor_Costs.Year,Labor_Costs.Month,Labor_Costs.week";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    SpentPerWeekModel spw = new SpentPerWeekModel()
                    {
                        job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "",
                        week = dr["week"] != DBNull.Value ? Convert.ToInt32(dr["week"]) : 0,
                        month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0,
                        year = dr["Year"] != DBNull.Value ? Convert.ToInt32(dr["Year"]) : 0,
                        budget100 = dr["Budget100"] != DBNull.Value ? Convert.ToInt32(dr["budget100"]) : 0,
                        budget80 = dr["Budget80"] != DBNull.Value ? Convert.ToInt32(dr["Budget80"]) : 0,
                        budget70 = dr["Budget70"] != DBNull.Value ? Convert.ToInt32(dr["Budget70"]) : 0,
                        budget50 = dr["Budget50"] != DBNull.Value ? Convert.ToInt32(dr["Budget50"]) : 0,
                        progress = dr["Progress"] != DBNull.Value ? Convert.ToInt32(dr["Progress"]) : 0,
                        spent_cost = dr["spent_cost"] != DBNull.Value ? Convert.ToInt32(dr["spent_cost"]) : 0,
                        acc_cost = dr["Acc_Cost"] != DBNull.Value ? Convert.ToInt32(dr["Acc_Cost"]) : 0,
                    };
                    spws.Add(spw);
                }
                dr.Close();
            }
            con.Close();
            return Json(spws);
        }

        [HttpGet]
        public JsonResult GetManPower(string job_id)
        {
            this.DB = new ConnectDB();
            List<ManpowerModel> mphs = new List<ManpowerModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "with normal " +
                                    "as ( select Job_ID,FORMAT(Working_Day,'yyyy') as Year,Month,Week,SUM(Hours) as Normal,0 as OT_1_5,0 as OT_3 " +
                                    "FROM Hour group by Job_ID,FORMAT(Working_Day,'yyyy'), Month, Week " +
                                    "union all " +
                                    "select Job_ID,FORMAT(Recording_time,'yyyy') as Year,Month,Week,0 as Normal ,SUM(case when OT_1_5 is null then 0 else OT_1_5 end) as OT_1_5,SUM(case when OT_3 is null then 0 else OT_3 end) as OT_3 " +
                                    "FROM OT group by Job_ID,Format(Recording_time,'yyyy'),Month ,Week )" +
                             "select Job_ID, " +
                                    "Week, " +
                                    "Month, " +
                                    "Year, " +
                                    "SUM(Normal) as Normal, " +
                                    "SUM(OT_1_5) as OT_1_5, " +
                                    "SUM(OT_3) as OT_3, " +
                                    "SUM(SUM(case when Normal is null then 0 else Normal end + case when OT_1_5 is null then 0 else OT_1_5 end  + case when OT_3 is null then 0 else OT_3 end)) OVER (partition by Job_ID ORDER BY Job_ID ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)as Acc_Hour " +
                                    "from normal " +
                                    "where Job_ID = '" + job_id + "' " +
                                    "group by Job_ID,Year,Month,Week";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ManpowerModel mph = new ManpowerModel()
                    {
                        job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "",
                        week = dr["Week"] != DBNull.Value ? Convert.ToInt32(dr["Week"]) : 0,
                        month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0,
                        year = dr["Year"] != DBNull.Value ? Convert.ToInt32(dr["Year"]) : 0,
                        normal = dr["Normal"] != DBNull.Value ? Convert.ToInt32(dr["Normal"]) : 0,
                        ot_1_5 = dr["OT_1_5"] != DBNull.Value ? Convert.ToInt32(dr["OT_1_5"]) : 0,
                        ot_3 = dr["OT_3"] != DBNull.Value ? Convert.ToInt32(dr["OT_3"]) : 0,
                        acc_hour = dr["Acc_Hour"] != DBNull.Value ? Convert.ToInt32(dr["Acc_Hour"]) : 0,
                    };
                    mphs.Add(mph);
                }
                dr.Close();
            }
            con.Close();
            return Json(mphs);
        }

        [HttpGet]
        public JsonResult GetNormalOvertimeRatio(string job_id)
        {
            this.DB = new ConnectDB();
            List<NormalOvertimeModel> nprs = new List<NormalOvertimeModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Hour.Job_ID, " +
                                    "SUM(Hours)as Normal, " +
                                    "s1.OT " +
                                    "from Hour " +
                                    "left join (select job_ID,(SUM(isnull(OT_1_5,0)) + SUM(isnull(OT_3,0))) as OT from OT group by Job_ID) as s1 ON s1.job_ID = Hour.job_ID " +
                                    "where Hour.Job_ID = '" + job_id + "' " +
                                    "group by Hour.Job_ID,s1.OT " +
                                    "order by Job_ID";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    NormalOvertimeModel npr = new NormalOvertimeModel()
                    {
                        job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "",
                        normal = dr["Normal"] != DBNull.Value ? Convert.ToInt32(dr["Normal"]) : 0,
                        overtime = dr["OT"] != DBNull.Value ? Convert.ToInt32(dr["OT"]) : 0,
                    };
                    nprs.Add(npr);
                }
                dr.Close();
            }
            con.Close();
            return Json(nprs);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
