using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Services
{
    public class OvertimeService : IOvertime
    {
        IConnectDB DB;

        public OvertimeService()
        {
            this.DB = new ConnectDB();
        }

        public List<OvertimeModel> GetOvertimes()
        {
            List<OvertimeModel> ots = new List<OvertimeModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select * from OT";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    OvertimeModel ot = new OvertimeModel();
                    ot.job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "";
                    ot.employee_id = dr["Employee_ID"] != DBNull.Value ? dr["Employee_ID"].ToString() : "";
                    ot.ot_1_5 = dr["OT_1_5"] != DBNull.Value ? Convert.ToDouble(dr["OT_1_5"]) : 0;
                    ot.ot_3 = dr["OT_3"] != DBNull.Value ? Convert.ToDouble(dr["OT_3"]) : 0;
                    ot.ot_sum = ot.ot_1_5 + ot.ot_3;
                    ot.week = dr["Week"] != DBNull.Value ? Convert.ToInt32(dr["Week"]) : 0;
                    ot.month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0;
                    ot.recording_time = dr["Recording_time"] != DBNull.Value ? Convert.ToDateTime(dr["Recording_time"]) : DateTime.MinValue;
                    ots.Add(ot);
                }
                dr.Close();
            }
            con.Close();
            return ots;
        }

        public List<OvertimeModel> GetOvertimes(string job_id)
        {
            List<OvertimeModel> ots = new List<OvertimeModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select * from OT where OT.Job_ID = '" + job_id.Replace("-",String.Empty).Replace(" ",String.Empty) + "'";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    OvertimeModel ot = new OvertimeModel();
                    ot.job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "";
                    ot.employee_id = dr["Employee_ID"] != DBNull.Value ? dr["Employee_ID"].ToString() : "";
                    ot.ot_1_5 = dr["OT_1_5"] != DBNull.Value ? Convert.ToDouble(dr["OT_1_5"]) : 0;
                    ot.ot_3 = dr["OT_3"] != DBNull.Value ? Convert.ToDouble(dr["OT_3"]) : 0;
                    ot.ot_sum = ot.ot_1_5 + ot.ot_3;
                    ot.week = dr["Week"] != DBNull.Value ? Convert.ToInt32(dr["Week"]) : 0;
                    ot.month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0;
                    ot.recording_time = dr["Recording_time"] != DBNull.Value ? Convert.ToDateTime(dr["Recording_time"]) : DateTime.MinValue;
                    ots.Add(ot);
                }
                dr.Close();
            }
            con.Close();
            return ots;
        }

        public List<OvertimeModel> GetOvertimes(string job_id, string year, int month, int week)
        {
            List<OvertimeModel> ots = new List<OvertimeModel>();
            SqlConnection con = DB.Connect();
            con.Open();
            string str_cmd = "SELECT * FROM OT WHERE Job_ID = '" + job_id.Replace("-",String.Empty).Replace(" ",String.Empty) + "' AND Recording_time LIKE '" + year + "%' AND Month = " + month + " AND Week = " + week;
            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    OvertimeModel ot = new OvertimeModel();
                    ot.job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "";
                    ot.employee_id = dr["Employee_ID"] != DBNull.Value ? dr["Employee_ID"].ToString() : "";
                    ot.recording_time = dr["Recording_time"] != DBNull.Value ? Convert.ToDateTime(dr["Recording_time"]) : DateTime.MinValue;
                    ot.week = dr["Week"] != DBNull.Value ? Convert.ToInt32(dr["Week"]) : 0;
                    ot.month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0;
                    ot.ot_1_5 = dr["OT_1_5"] != DBNull.Value ? Convert.ToInt32(dr["OT_1_5"]) : 0;
                    ot.ot_3 = dr["OT_3"] != DBNull.Value ? Convert.ToInt32(dr["OT_3"]) : 0;
                    ot.ot_sum = dr["OT_SUM"] != DBNull.Value ? Convert.ToInt32(dr["OT_SUM"]) : 0;
                    ots.Add(ot);
                }
                dr.Close();
            }
            con.Close();
            return ots;
        }

        public string InsertOvertimes(List<OvertimeModel> ots)
        {
            SqlConnection con = DB.Connect();
            using (SqlCommand cmd = new SqlCommand("INSERT INTO OT(" +
                                                                "Job_ID, " +
                                                                "Employee_ID, " +
                                                                "OT_1_5, " +
                                                                "OT_3, " +
                                                                "OT_Sum, " +
                                                                "Week, " +
                                                                "Month, " +
                                                                "Recording_time) " +
                                                     "VALUES(@Job_ID," +
                                                            "@Employee_ID, " +
                                                            "@OT_1_5, " +
                                                            "@OT_3, " +
                                                            "@OT_Sum, " +
                                                            "@Week, " +
                                                            "@Month, " +
                                                            "@Recording_time)", con))
            {
                con.Open();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Employee_ID", SqlDbType.NVarChar);
                cmd.Parameters.Add("@OT_1_5", SqlDbType.Int);
                cmd.Parameters.Add("@OT_3", SqlDbType.Int);
                cmd.Parameters.Add("@OT_Sum", SqlDbType.Int);
                cmd.Parameters.Add("@Week", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Month", SqlDbType.Int);
                cmd.Parameters.Add("@Recording_time", SqlDbType.Date);

                for (int i = 0; i < ots.Count; i++)
                {
                    cmd.Parameters[0].Value = ots[i].job_id.Replace("-",String.Empty).Replace(" ",String.Empty);
                    cmd.Parameters[1].Value = ots[i].employee_id;
                    cmd.Parameters[2].Value = ots[i].ot_1_5;
                    cmd.Parameters[3].Value = ots[i].ot_3;
                    cmd.Parameters[4].Value = ots[i].ot_sum;
                    cmd.Parameters[5].Value = ots[i].week;
                    cmd.Parameters[6].Value = ots[i].month;
                    cmd.Parameters[7].Value = ots[i].recording_time;
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            return "Done";
        }

        public string DeleteOvertimes(string job_id, string year, int month, int week)
        {
            SqlConnection con = DB.Connect();
            try
            {
                con.Open();
                string str_cmd = "DELETE FROM OT WHERE Job_ID = '" + job_id.Replace("-",String.Empty).Replace(" ",String.Empty) + "' AND Recording_time LIKE '" + year + "%' AND Month = " + month + " AND Week = " + week;
                SqlCommand cmd = new SqlCommand(str_cmd, con);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                con.Close();
            }
            return "Done";
        }
    }
}
