using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Services
{
    public class NormalOvertimeService : INormalOvertime
    {
        IConnectDB DB;

        public NormalOvertimeService()
        {
            this.DB = new ConnectDB();
        }

        public List<NormalOvertimeModel> NormalPerOvertime()
        {
            List<NormalOvertimeModel> nprs = new List<NormalOvertimeModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Hour.Job_ID, " +
                                    "Job.Job_Year, " +
                                    "SUM(Hours) as Normal, " +
                                    "CONVERT(NUMERIC(18,2),(s1.OT/60 + (s1.OT %60)/100.0)) as OT " +
                             "from Hour " +
                             "left join (select job_ID,(SUM(isnull(OT_1_5,0)) + SUM(isnull(OT_3,0))) as OT from OT group by Job_ID) as s1 ON s1.job_ID = Hour.job_ID " +
                             "left join Job ON Job.Job_ID = Hour.Job_ID " +
                             "group by Job.Job_Year,Hour.Job_ID,s1.OT " +
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
                        normal = dr["Normal"] != DBNull.Value ? Convert.ToDouble(dr["Normal"]) : 0,
                        overtime = dr["OT"] != DBNull.Value ? Convert.ToDouble(dr["OT"]) : 0,
                    };
                    nprs.Add(npr);
                }
                dr.Close();
            }
            con.Close();
            nprs = nprs.OrderByDescending(o => o.job_id).ToList();
            return nprs;
        }

        public List<NormalOvertimeModel> NormalPerOvertime(string job_id)
        {
            List<NormalOvertimeModel> nprs = new List<NormalOvertimeModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Hour.Job_ID, " +
                                    "Job.Job_Year, " +
                                    "SUM(Hours) as Normal, " +
                                    "CONVERT(NUMERIC(18,2),(s1.OT/60 + (s1.OT %60)/100.0)) as OT " +
                             "from Hour " +
                             "left join (select job_ID,(SUM(isnull(OT_1_5,0)) + SUM(isnull(OT_3,0))) as OT from OT group by Job_ID) as s1 ON s1.job_ID = Hour.job_ID " +
                             "left join Job ON Job.Job_ID = Hour.Job_ID " +
                             "where Hour.Job_ID = '" + job_id + "' " +
                             "group by Job.Job_Year, Hour.Job_ID, s1.OT " +
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
                        normal = dr["Normal"] != DBNull.Value ? Convert.ToDouble(dr["Normal"]) : 0,
                        overtime = dr["OT"] != DBNull.Value ? Convert.ToDouble(dr["OT"]) : 0,
                    };
                    nprs.Add(npr);
                }
                dr.Close();
            }
            con.Close();
            nprs = nprs.OrderByDescending(o => o.job_id).ToList();
            return nprs;
        }

        public List<NormalOvertimeModel> NormalPerOvertimeByYear(string year)
        {
            List<NormalOvertimeModel> nprs = new List<NormalOvertimeModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Hour.Job_ID, " +
                                    "Job.Job_Year, " +
                                    "SUM(Hours) as Normal, " +
                                    "CONVERT(NUMERIC(18,2),(s1.OT/60 + (s1.OT %60)/100.0)) as OT " +
                             "from Hour " +
                             "left join (select job_ID,(SUM(isnull(OT_1_5,0)) + SUM(isnull(OT_3,0))) as OT from OT group by Job_ID) as s1 ON s1.job_ID = Hour.job_ID " +
                             "left join Job ON Job.Job_ID = Hour.Job_ID " +
                             "where Job.Job_Year = " + year + " " +
                             "group by Job.Job_Year, Hour.Job_ID, s1.OT " +
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
                        normal = dr["Normal"] != DBNull.Value ? Convert.ToDouble(dr["Normal"]) : 0,
                        overtime = dr["OT"] != DBNull.Value ? Convert.ToDouble(dr["OT"]) : 0,
                    };
                    nprs.Add(npr);
                }
                dr.Close();
            }
            con.Close();
            nprs = nprs.OrderByDescending(o => o.job_id).ToList();
            return nprs;
        }

        public List<NormalOvertimeModel> NormalPerOvertimeByUser(string user_name)
        {
            List<NormalOvertimeModel> nprs = new List<NormalOvertimeModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select s2.Job_ID, " +
                                    "Job.Job_Year, " +
                                    "SUM(Hours) as Normal, " +
                                    "CONVERT(NUMERIC(18,2),(s1.OT/60 + (s1.OT %60)/100.0)) as OT " +
                             "from Hour " +
                             "left join ( select " +
                                            "job_ID, " +
                                            "(SUM(isnull(OT_1_5,0)) + SUM(isnull(OT_3,0))) as OT " +
                                         "from OT " +
                                         "group by Job_ID ) as s1 ON s1.job_ID = Hour.job_ID " +
                             "left join ( select " +
                                            "User_Accessibility.User_ID, " +
                                            "User_Authentication.User_Name, " +
                                            "User_Authentication.Permission, " +
                                            "User_Accessibility.Job_ID " +
                                         "from User_Accessibility " +
                                         "left join User_Authentication ON User_Authentication.User_ID = User_Accessibility.User_ID " +
                                         "where User_Authentication.User_Name='" + user_name + "') as s2 ON s2.Job_ID = Hour.Job_ID " +
                             "left join Job ON Job.Job_ID = Hour.Job_ID where s2.Job_ID is not null " +
                             "group by Job.Job_Year, s2.Job_ID, s1.OT " +
                             "order by s2.Job_ID";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    NormalOvertimeModel npr = new NormalOvertimeModel()
                    {
                        job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "",
                        normal = dr["Normal"] != DBNull.Value ? Convert.ToDouble(dr["Normal"]) : 0,
                        overtime = dr["OT"] != DBNull.Value ? Convert.ToDouble(dr["OT"]) : 0,
                    };
                    nprs.Add(npr);
                }
                dr.Close();
            }

            con.Close();
            nprs = nprs.OrderByDescending(o => o.job_id).ToList();
            return nprs;
        }

        public List<NormalOvertimeModel> NormalPerOvertimeByUser(string user_name, string year)
        {
            List<NormalOvertimeModel> nprs = new List<NormalOvertimeModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select s2.Job_ID, " +
                                    "Job.Job_Year, " +
                                    "SUM(Hours) as Normal, " +
                                    "CONVERT(NUMERIC(18,2),(s1.OT/60 + (s1.OT %60)/100.0)) as OT " +
                             "from Hour " +
                             "left join ( select " +
                                            "job_ID, " +
                                            "(SUM(isnull(OT_1_5,0)) + SUM(isnull(OT_3,0))) as OT " +
                                         "from OT " +
                                         "group by Job_ID ) as s1 ON s1.job_ID = Hour.job_ID " +
                             "left join ( select " +
                                            "User_Accessibility.User_ID, " +
                                            "User_Authentication.User_Name, " +
                                            "User_Authentication.Permission, " +
                                            "User_Accessibility.Job_ID " +
                                         "from User_Accessibility " +
                                         "left join User_Authentication ON User_Authentication.User_ID = User_Accessibility.User_ID " +
                                         "where User_Authentication.User_Name = '" + user_name + "') as s2 ON s2.Job_ID = Hour.Job_ID " +
                             "left join Job ON Job.Job_ID = Hour.Job_ID " +
                             "where s2.Job_ID is not null and Job.Job_Year = '" + year + "' " +
                             "group by Job.Job_Year, s2.Job_ID, s1.OT " +
                             "order by s2.Job_ID";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    NormalOvertimeModel npr = new NormalOvertimeModel()
                    {
                        job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "",
                        normal = dr["Normal"] != DBNull.Value ? Convert.ToDouble(dr["Normal"]) : 0,
                        overtime = dr["OT"] != DBNull.Value ? Convert.ToDouble(dr["OT"]) : 0,
                    };
                    nprs.Add(npr);
                }
                dr.Close();
            }

            con.Close();
            nprs = nprs.OrderByDescending(o => o.job_id).ToList();
            return nprs;
        }

    }
}
