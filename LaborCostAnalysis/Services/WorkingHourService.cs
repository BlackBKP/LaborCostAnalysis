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
    public class WorkingHourService : IWorkingHour
    {
        IConnectDB DB;

        public WorkingHourService()
        {
            this.DB = new ConnectDB();
        }

        public List<WorkingHoursModel> GetWorkingHours()
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            SqlConnection con = DB.Connect();
            con.Open();
            string str_cmd = "select * from Hour";
            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    WorkingHoursModel wh = new WorkingHoursModel();
                    wh.job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "";
                    wh.employee_id = dr["Employee_ID"] != DBNull.Value ? dr["Employee_ID"].ToString() : "";
                    wh.working_day = dr["Working_Day"] != DBNull.Value ? Convert.ToDateTime(dr["Working_Day"]) : DateTime.MinValue;
                    wh.week = dr["Week"] != DBNull.Value ? Convert.ToInt32(dr["Week"]) : 0;
                    wh.month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0;
                    wh.hours = dr["Hours"] != DBNull.Value ? Convert.ToInt32(dr["Hours"]) : 0;
                    whs.Add(wh);
                }
                dr.Close();
            }
            con.Close();
            return whs;
        }

        public List<WorkingHoursModel> GetWorkingHours(string job_id)
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            SqlConnection con = DB.Connect();
            con.Open();
            string str_cmd = "select * from Hour where Hour.Job_ID = '" + job_id.Replace("-",String.Empty).Replace(" ",String.Empty) + "'";
            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    WorkingHoursModel wh = new WorkingHoursModel();
                    wh.job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "";
                    wh.employee_id = dr["Employee_ID"] != DBNull.Value ? dr["Employee_ID"].ToString() : "";
                    wh.working_day = dr["Working_Day"] != DBNull.Value ? Convert.ToDateTime(dr["Working_Day"]) : DateTime.MinValue;
                    wh.week = dr["Week"] != DBNull.Value ? Convert.ToInt32(dr["Week"]) : 0;
                    wh.month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0;
                    wh.hours = dr["Hours"] != DBNull.Value ? Convert.ToInt32(dr["Hours"]) : 0;
                    whs.Add(wh);
                }
                dr.Close();
            }
            con.Close();
            return whs;
        }

        public List<WorkingHoursModel> GetWorkingHours(string job_id,string year,int month,int week)
        {
            List<WorkingHoursModel> whs = new List<WorkingHoursModel>();
            SqlConnection con = DB.Connect();
            con.Open();
            string str_cmd = "SELECT * FROM Hour WHERE Job_ID = '" + job_id.Replace("-",String.Empty).Replace(" ",String.Empty) + "' AND Working_Day LIKE '" + year + "%' AND Month = " + month +" AND Week = " + week;
            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    WorkingHoursModel wh = new WorkingHoursModel();
                    wh.job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "";
                    wh.employee_id = dr["Employee_ID"] != DBNull.Value ? dr["Employee_ID"].ToString() : "";
                    wh.working_day = dr["Working_Day"] != DBNull.Value ? Convert.ToDateTime(dr["Working_Day"]) : DateTime.MinValue;
                    wh.week = dr["Week"] != DBNull.Value ? Convert.ToInt32(dr["Week"]) : 0;
                    wh.month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0;
                    wh.hours = dr["Hours"] != DBNull.Value ? Convert.ToInt32(dr["Hours"]) : 0;
                    whs.Add(wh);
                }
                dr.Close();
            }
            con.Close();
            return whs;
        }

        public string InsertWorkingHours(List<WorkingHoursModel> whs)
        {
            SqlConnection con = DB.Connect();
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Hour(" +
                                                                "Job_ID, " +
                                                                "Employee_ID, " +
                                                                "Working_Day, " +
                                                                "Week, " +
                                                                "Month, " +
                                                                "Hours) " +
                                                     "VALUES(@Job_ID," +
                                                            "@Employee_ID, " +
                                                            "@Working_Day, " +
                                                            "@Week, " +
                                                            "@Month, " +
                                                            "@Hours)", con))
            {
                con.Open();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Employee_ID", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Working_Day", SqlDbType.Date);
                cmd.Parameters.Add("@Week", SqlDbType.Int);
                cmd.Parameters.Add("@Month", SqlDbType.Int);
                cmd.Parameters.Add("@Hours", SqlDbType.Int);

                for (int i = 0; i < whs.Count; i++)
                {
                    cmd.Parameters[0].Value = whs[i].job_id.Replace("-",String.Empty).Replace(" ",String.Empty);
                    cmd.Parameters[1].Value = whs[i].employee_id;
                    cmd.Parameters[2].Value = whs[i].working_day;
                    cmd.Parameters[3].Value = whs[i].week;
                    cmd.Parameters[4].Value = whs[i].month;
                    cmd.Parameters[5].Value = whs[i].hours;
                    cmd.ExecuteNonQuery();
                }
            }
            return "Done";
        }

        public string DeleteWorkingHours(string job_id, string year, int month, int week)
        {
            SqlConnection con = DB.Connect();
            try
            {
                con.Open();
                string str_cmd = "DELETE FROM Hour WHERE Job_ID = '" + job_id.Replace("-",String.Empty).Replace(" ",String.Empty) + "' AND Working_Day LIKE '" + year + "%' AND Month = " + month + " AND Week = " + week;
                SqlCommand cmd = new SqlCommand(str_cmd, con);
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
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
