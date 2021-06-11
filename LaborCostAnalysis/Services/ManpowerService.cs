using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Services
{
    public class ManpowerService : IManpower
    {
        IConnectDB DB;

        public ManpowerService()
        {
            this.DB = new ConnectDB();
        }

        public List<List<ManpowerModel>> GetMPHModels()
        {
            List<ManpowerModel> mphs = new List<ManpowerModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "with normal as ( " +
                                    "select Job_ID," +
                                            "FORMAT(Working_Day,'yyyy') as Year, " +
                                            "Month, " +
                                            "Week, " +
                                            "SUM(Hours) as Normal, " +
                                            "0 as OT_1_5, " +
                                            "0 as OT_3 " +
                                    "FROM Hour " +
                                    "group by Job_ID, FORMAT(Working_Day,'yyyy'), Month, Week " +
                                    "union all " +
                                    "select Job_ID, " +
                                            "FORMAT(Recording_time,'yyyy') as Year, " +
                                            "Month, " +
                                            "Week, " +
                                            "0 as Normal, " +
                                            "SUM(case when OT_1_5 is null then 0 else OT_1_5 end) as OT_1_5, " +
                                            "SUM(case when OT_3 is null then 0 else OT_3 end) as OT_3 " +
                                    "FROM OT " +
                                    "group by Job_ID, Format(Recording_time,'yyyy'), Month , Week ) " +
                                    "select normal.Job_ID, " +
                                            "Week, " +
                                            "Month, " +
                                            "Year, " +
                                            "job.Job_Year, " +
                                            "SUM(Normal) as Normal, " +
                                            "SUM(CONVERT(NUMERIC(18,2),(OT_1_5/60 + (OT_1_5 %60)/100.0))) as OT_1_5, " +
                                            "SUM(CONVERT(NUMERIC(18,2),(OT_3/60 + (OT_3 %60)/100.0))) as OT_3, " +
                                            "SUM(SUM(case when Normal is null then 0.00 else Normal end + case when OT_1_5 is null then 0.00 else CONVERT(NUMERIC(18,2),(OT_1_5/60 + (OT_1_5 %60)/100.0)) end + case when OT_3 is null then 0.00 else CONVERT(NUMERIC(18,2),(OT_3/60 + (OT_3 %60)/100.0)) end)) OVER (partition by normal.Job_ID ORDER BY normal.Job_ID ROWS BETWEEN UNBOUNDED PRECEDING AND  CURRENT ROW) as Acc_Hour " +
                                    "from normal " +
                                    "left join Job ON Job.Job_ID = normal.Job_ID " +
                                    "group by job.Job_Year, normal.Job_ID, Year, Month, Week";

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
                        normal = dr["Normal"] != DBNull.Value ? Convert.ToDouble(dr["Normal"]) : 0,
                        ot_1_5 = dr["OT_1_5"] != DBNull.Value ? Convert.ToDouble(dr["OT_1_5"]) : 0,
                        ot_3 = dr["OT_3"] != DBNull.Value ? Convert.ToDouble(dr["OT_3"]) : 0,
                        acc_hour = dr["Acc_Hour"] != DBNull.Value ? Convert.ToDouble(dr["Acc_Hour"]) : 0,
                    };
                    mphs.Add(mph);
                }
                dr.Close();
            }
            con.Close();

            List<List<ManpowerModel>> lmphs = new List<List<ManpowerModel>>();
            string[] job_id = mphs.Select(s => s.job_id).Distinct().ToArray();
            for (int i = 0; i < job_id.Count(); i++)
            {
                lmphs.Add(mphs.Where(w => w.job_id == job_id[i]).Select(s => s).OrderByDescending(y => y.year).ThenBy(m => m.month).ThenBy(w => w.week).ToList());
            }
            return lmphs;
        }

        public List<List<ManpowerModel>> GetMPHModelsByYear(string year)
        {
            List<ManpowerModel> mphs = new List<ManpowerModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "with normal as ( " +
                                    "select Job_ID," +
                                            "FORMAT(Working_Day,'yyyy') as Year, " +
                                            "Month, " +
                                            "Week, " +
                                            "SUM(Hours) as Normal, " +
                                            "0 as OT_1_5, " +
                                            "0 as OT_3 " +
                                    "FROM Hour " +
                                    "group by Job_ID, FORMAT(Working_Day,'yyyy'), Month, Week " +
                                    "union all " +
                                    "select Job_ID, " +
                                            "FORMAT(Recording_time,'yyyy') as Year, " +
                                            "Month, " +
                                            "Week, " +
                                            "0 as Normal, " +
                                            "SUM(case when OT_1_5 is null then 0 else OT_1_5 end) as OT_1_5, " +
                                            "SUM(case when OT_3 is null then 0 else OT_3 end) as OT_3 " +
                                    "FROM OT " +
                                    "group by Job_ID, Format(Recording_time,'yyyy'), Month , Week ) " +
                                    "select normal.Job_ID, " +
                                            "Week, " +
                                            "Month, " +
                                            "Year, " +
                                            "job.Job_Year, " +
                                            "SUM(Normal) as Normal, " +
                                            "SUM(CONVERT(NUMERIC(18,2),(OT_1_5/60 + (OT_1_5 %60)/100.0))) as OT_1_5, " +
                                            "SUM(CONVERT(NUMERIC(18,2),(OT_3/60 + (OT_3 %60)/100.0))) as OT_3, " +
                                            "SUM(SUM(case when Normal is null then 0.00 else Normal end + case when OT_1_5 is null then 0.00 else CONVERT(NUMERIC(18,2),(OT_1_5/60 + (OT_1_5 %60)/100.0)) end + case when OT_3 is null then 0.00 else CONVERT(NUMERIC(18,2),(OT_3/60 + (OT_3 %60)/100.0)) end)) OVER (partition by normal.Job_ID ORDER BY normal.Job_ID ROWS BETWEEN UNBOUNDED PRECEDING AND  CURRENT ROW) as Acc_Hour " +
                                    "from normal " +
                                    "left join Job ON Job.Job_ID = normal.Job_ID " +
                                    "where job.Job_year = " + year + " " +
                                    "group by job.Job_Year, normal.Job_ID, Year, Month, Week";

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
                        normal = dr["Normal"] != DBNull.Value ? Convert.ToDouble(dr["Normal"]) : 0,
                        ot_1_5 = dr["OT_1_5"] != DBNull.Value ? Convert.ToDouble(dr["OT_1_5"]) : 0,
                        ot_3 = dr["OT_3"] != DBNull.Value ? Convert.ToDouble(dr["OT_3"]) : 0,
                        acc_hour = dr["Acc_Hour"] != DBNull.Value ? Convert.ToDouble(dr["Acc_Hour"]) : 0,
                    };
                    mphs.Add(mph);
                }
                dr.Close();
            }
            con.Close();

            List<List<ManpowerModel>> lmphs = new List<List<ManpowerModel>>();
            string[] job_id = mphs.Select(s => s.job_id).Distinct().ToArray();
            for (int i = 0; i < job_id.Count(); i++)
            {
                lmphs.Add(mphs.Where(w => w.job_id == job_id[i]).Select(s => s).OrderByDescending(y => y.year).ThenBy(m => m.month).ThenBy(w => w.week).ToList());
            }
            return lmphs;
        }

        public List<List<ManpowerModel>> GetMPHModels(string job_id)
        {
            List<ManpowerModel> mphs = new List<ManpowerModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "with normal as ( " +
                                    "select Job_ID," +
                                            "FORMAT(Working_Day,'yyyy') as Year, " +
                                            "Month, " +
                                            "Week, " +
                                            "SUM(Hours) as Normal, " +
                                            "0 as OT_1_5, " +
                                            "0 as OT_3 " +
                                    "FROM Hour " +
                                    "group by Job_ID, FORMAT(Working_Day,'yyyy'), Month, Week " +
                                    "union all " +
                                    "select Job_ID, " +
                                            "FORMAT(Recording_time,'yyyy') as Year, " +
                                            "Month, " +
                                            "Week, " +
                                            "0 as Normal, " +
                                            "SUM(case when OT_1_5 is null then 0 else OT_1_5 end) as OT_1_5, " +
                                            "SUM(case when OT_3 is null then 0 else OT_3 end) as OT_3 " +
                                    "FROM OT " +
                                    "group by Job_ID, Format(Recording_time,'yyyy'), Month , Week ) " +
                                    "select normal.Job_ID, " +
                                            "Week, " +
                                            "Month, " +
                                            "Year, " +
                                            "job.Job_Year, " +
                                            "SUM(Normal) as Normal, " +
                                            "SUM(CONVERT(NUMERIC(18,2),(OT_1_5/60 + (OT_1_5 %60)/100.0))) as OT_1_5, " +
                                            "SUM(CONVERT(NUMERIC(18,2),(OT_3/60 + (OT_3 %60)/100.0))) as OT_3, " +
                                            "SUM(SUM(case when Normal is null then 0.00 else Normal end + case when OT_1_5 is null then 0.00 else CONVERT(NUMERIC(18,2),(OT_1_5/60 + (OT_1_5 %60)/100.0)) end + case when OT_3 is null then 0.00 else CONVERT(NUMERIC(18,2),(OT_3/60 + (OT_3 %60)/100.0)) end)) OVER (partition by normal.Job_ID ORDER BY normal.Job_ID ROWS BETWEEN UNBOUNDED PRECEDING AND  CURRENT ROW) as Acc_Hour " +
                                    "from normal " +
                                    "left join Job ON Job.Job_ID = normal.Job_ID " +
                                    "where job.Job_ID = '" + job_id + "' " +
                                    "group by job.Job_Year, normal.Job_ID, Year, Month, Week";

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
                        normal = dr["Normal"] != DBNull.Value ? Convert.ToDouble(dr["Normal"]) : 0,
                        ot_1_5 = dr["OT_1_5"] != DBNull.Value ? Convert.ToDouble(dr["OT_1_5"]) : 0,
                        ot_3 = dr["OT_3"] != DBNull.Value ? Convert.ToDouble(dr["OT_3"]) : 0,
                        acc_hour = dr["Acc_Hour"] != DBNull.Value ? Convert.ToDouble(dr["Acc_Hour"]) : 0,
                    };
                    mphs.Add(mph);
                }
                dr.Close();
            }
            con.Close();

            List<List<ManpowerModel>> lmphs = new List<List<ManpowerModel>>();
            string[] job = mphs.Select(s => s.job_id).Distinct().ToArray();
            for (int i = 0; i < job.Count(); i++)
            {
                lmphs.Add(mphs.Where(w => w.job_id == job[i]).Select(s => s).OrderByDescending(y => y.year).ThenBy(m => m.month).ThenBy(w => w.week).ToList());
            }
            return lmphs;
        }

        public List<ManpowerModel> GetManpower(string job_id)
        {
            List<ManpowerModel> mphs = new List<ManpowerModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "with normal as ( " +
                                    "select Job_ID," +
                                            "FORMAT(Working_Day,'yyyy') as Year, " +
                                            "Month, " +
                                            "Week, " +
                                            "SUM(Hours) as Normal, " +
                                            "0 as OT_1_5, " +
                                            "0 as OT_3 " +
                                    "FROM Hour " +
                                    "group by Job_ID, FORMAT(Working_Day,'yyyy'), Month, Week " +
                                    "union all " +
                                    "select Job_ID, " +
                                            "FORMAT(Recording_time,'yyyy') as Year, " +
                                            "Month, " +
                                            "Week, " +
                                            "0 as Normal, " +
                                            "SUM(case when OT_1_5 is null then 0 else OT_1_5 end) as OT_1_5, " +
                                            "SUM(case when OT_3 is null then 0 else OT_3 end) as OT_3 " +
                                    "FROM OT " +
                                    "group by Job_ID, Format(Recording_time,'yyyy'), Month , Week ) " +
                                    "select normal.Job_ID, " +
                                            "Week, " +
                                            "Month, " +
                                            "Year, " +
                                            "job.Job_Year, " +
                                            "SUM(Normal) as Normal, " +
                                            "SUM(CONVERT(NUMERIC(18,2),(OT_1_5/60 + (OT_1_5 %60)/100.0))) as OT_1_5, " +
                                            "SUM(CONVERT(NUMERIC(18,2),(OT_3/60 + (OT_3 %60)/100.0))) as OT_3, " +
                                            "SUM(SUM(case when Normal is null then 0.00 else Normal end + case when OT_1_5 is null then 0.00 else CONVERT(NUMERIC(18,2),(OT_1_5/60 + (OT_1_5 %60)/100.0)) end + case when OT_3 is null then 0.00 else CONVERT(NUMERIC(18,2),(OT_3/60 + (OT_3 %60)/100.0)) end)) OVER (partition by normal.Job_ID ORDER BY normal.Job_ID ROWS BETWEEN UNBOUNDED PRECEDING AND  CURRENT ROW) as Acc_Hour " +
                                    "from normal " +
                                    "left join Job ON Job.Job_ID = normal.Job_ID " +
                                    "where job.Job_ID = '" + job_id + "' " +
                                    "group by job.Job_Year, normal.Job_ID, Year, Month, Week";

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
                        normal = dr["Normal"] != DBNull.Value ? Convert.ToDouble(dr["Normal"]) : 0,
                        ot_1_5 = dr["OT_1_5"] != DBNull.Value ? Convert.ToDouble(dr["OT_1_5"]) : 0,
                        ot_3 = dr["OT_3"] != DBNull.Value ? Convert.ToDouble(dr["OT_3"]) : 0,
                        acc_hour = dr["Acc_Hour"] != DBNull.Value ? Convert.ToDouble(dr["Acc_Hour"]) : 0,
                    };
                    mphs.Add(mph);
                }
                dr.Close();
            }
            con.Close();
            return mphs;
        }
    }
}
