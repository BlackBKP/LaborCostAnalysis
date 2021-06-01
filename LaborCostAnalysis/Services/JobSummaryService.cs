using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Services
{
    public class JobSummaryService : IJobSummary
    {
        IConnectDB DB;

        public JobSummaryService()
        {
            this.DB = new ConnectDB();
        }

        public List<JobSummaryModel> GetJobsSummary()
        {
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
                                    "s1.Cost_to_Date, " +
                                    "(cast(job.Estimated_Budget as int) - cast(s1.Cost_to_Date as int)) as Remaining_Cost, " +
                                    "((cast(s1.Cost_to_Date as float) / cast(job.Estimated_Budget as float)) *100) as Cost_Usage, " +
                                    "s4.Last_Progress as Work_Completion, " +
                                    "s2.Hours, " +
                                    "CONVERT(NUMERIC(18,2),(s3.OT_1_5/60 + (s3.OT_1_5 %60)/100.0)) as OT_1_5, " +
                                    "CONVERT(NUMERIC(18,2),(s3.OT_3/60 + (s3.OT_3 %60)/100.0)) as OT_3, " +
                                    "(s2.Hours + isnull(CONVERT(NUMERIC(18,2),(s3.OT_1_5/60 + (s3.OT_1_5 %60)/100.0)),0) + isnull(CONVERT(NUMERIC(18,2),(s3.OT_3/60 + (s3.OT_3 %60)/100.0)),0)) as Total_Man_Hour, " +
                                    "s5.No_Of_Labor_Week as No_Of_Labor, " +
                                    "(cast(s1.Cost_to_Date as float) / (s2.Hours + isnull(CONVERT(NUMERIC(18,2),(s3.OT_1_5/60 + (s3.OT_1_5 %60)/100.0)),0) + isnull(CONVERT(NUMERIC(18,2),(s3.OT_3/60 + (s3.OT_3 %60)/100.0)),0))) as avg_labor_cost_per_hour " +
                                    "from job " +
                                    "left join (select job_ID, SUM(cast(Labor_Cost as int)) as Labor_Cost, " +
                                    "SUM(cast(OT_Labor_Cost as int)) as OT_Labor_Cost, " +
                                    "SUM(cast(Accommodation_Cost as int)) as Accommodation_Cost, " +
                                    "SUM(cast(Compensation_Cost as int)) as Compensation_Cost, " +
                                    "SUM(isnull(Social_Security,0)) as Social_Security, " +
                                    "(SUM(cast(Labor_Cost as int)) + SUM(cast(OT_Labor_Cost as int)) + SUM(cast(Accommodation_Cost as int)) + SUM(cast(Compensation_Cost as int)) + SUM(isnull(Social_Security,0))) as Cost_to_Date " +
                                    "from Labor_Costs group by job_ID) as s1 ON s1.job_ID = job.job_ID " +
                                    "left join (select job_ID,SUM(Hours) as Hours from Hour group by Job_ID) as s2 ON s2.job_ID = job.job_ID " +
                                    "left join (select job_ID,SUM(OT_1_5) as OT_1_5 , SUM(OT_3) as OT_3 from OT group by job_ID) as s3 ON s3.job_ID = job.job_ID " +
                                    "left join (select Job_ID,Max(cast(Job_Progress as int)) as Last_Progress from Progress group by Job_ID) as s4 ON s4.Job_ID = job.job_ID " +
                                    "left join (select Job_ID,Max(cast(No_Of_Labor_Week as int)) as No_Of_Labor_Week from Labor_Costs group by Job_ID) as s5 ON s5.Job_ID = job.Job_ID";
            
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
                        hours = dr["Hours"] != DBNull.Value ? Convert.ToDouble(dr["Hours"]) : 0,
                        ot_1_5 = dr["OT_1_5"] != DBNull.Value ? Convert.ToDouble(dr["OT_1_5"]) : 0,
                        ot_3 = dr["OT_3"] != DBNull.Value ? Convert.ToDouble(dr["OT_3"]) : 0,
                        total_man_hour = dr["Total_Man_Hour"] != DBNull.Value ? Convert.ToDouble(dr["Total_Man_Hour"]) : 0,
                        no_of_labor = dr["No_Of_Labor"] != DBNull.Value ? Convert.ToInt32(dr["No_Of_Labor"]) : 0,
                        avg_labor_cost_per_hour = dr["avg_labor_cost_per_hour"] != DBNull.Value ? Convert.ToDouble(dr["avg_labor_cost_per_hour"]) : 0,
                    };
                    jobs.Add(job);
                }
                dr.Close();
            }
            con.Close();
            return jobs;
        }
    }
}
