using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Services
{
    public class SpentPerWeekService : ISpentPerWeek
    {
        IConnectDB DB;

        public SpentPerWeekService()
        {
            this.DB = new ConnectDB();
        }

        public List<SpentPerWeekModel> GetSpentCostPerWeeks()
        {
            List<SpentPerWeekModel> spws = new List<SpentPerWeekModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Labor_Costs.job_ID, " +
                                    "s1.Job_Name, " +
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
                                    "left join (select job_ID,Job_Name,isnull(Estimated_Budget,0) as Estimated_Budget from job) as s1 ON s1.job_ID = Labor_Costs.job_ID " +
                                    "left join (select Job_ID,Job_Progress,Month,Year from Progress) as s2 ON s2.Job_ID = Labor_Costs.job_ID and s2.Year = Labor_Costs.Year and s2.Month = Labor_Costs.Month " +
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
                        job_name = dr["Job_Name"] != DBNull.Value ? dr["Job_Name"].ToString() : "",
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
            return spws;
        }

        public List<SpentPerWeekModel> GetSpentPerWeeksByJob(string job_id)
        {
            List<SpentPerWeekModel> spws = new List<SpentPerWeekModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Labor_Costs.job_ID, " +
                                    "s1.Job_Name, " +
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
                                    "left join (select job_ID,Job_Name,isnull(Estimated_Budget,0) as Estimated_Budget from job) as s1 ON s1.job_ID = Labor_Costs.job_ID " +
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
                        job_name = dr["Job_Name"] != DBNull.Value ? dr["Job_Name"].ToString() : "",
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
            return spws;
        }
    }
}
