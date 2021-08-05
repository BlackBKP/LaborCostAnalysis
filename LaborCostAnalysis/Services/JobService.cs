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
    public class JobService :IJob
    {
        IConnectDB DB;

        public JobService()
        {
            this.DB = new ConnectDB();
        }

        public List<JobModel> GetJobs()
        {
            List<JobModel> jobs = new List<JobModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Job_ID," +
                                    "Job_Number, " +
                                    "Job_Name, " +
                                    "Estimated_Budget, " +
                                    "Job_Year, " +
                                    "Job_Type " +
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
                        job_type = dr["Job_Type"] != DBNull.Value ? dr["Job_Type"].ToString() : ""
                    };
                    jobs.Add(job);
                }
                dr.Close();
            }
            return jobs;
        }

        public List<JobModel> GetJobsWithAuthentication(string user_id)
        {
            List<JobModel> jobs = new List<JobModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Job.Job_ID, " +
                                    "Job.Job_Number, " +
                                    "Job.Job_Name, " +
                                    "Job.Estimated_Budget, " +
                                    "Job.Job_Year, " +
                                    "Job.Job_Type, " +
                                    "User_Accessibility.User_ID, " +
                                    "User_Authentication.User_Name, " +
                                    "User_Authentication.Permission " +
                             "from Job " +
                             "left join User_Accessibility on Job.Job_ID = User_Accessibility.Job_ID " +
                             "left join User_Authentication on User_Accessibility.User_ID = User_Authentication.User_ID " +
                             "where User_Authentication.User_Name = '" + user_id + "'";

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
                        job_type = dr["Job_Type"] != DBNull.Value ? dr["Job_Type"].ToString() : ""
                    };
                    jobs.Add(job);
                }
                dr.Close();
            }
            return jobs;
        }

        public string UpdateJobName(string job_number, string job_name)
        {
            SqlConnection con = DB.Connect();
            try
            {
                con.Open();
                string job_id = job_number.Replace("-", String.Empty).Replace(" ", String.Empty);
                string str_cmd = "Update Job Set Job_Name = '" + job_name + "' where Job_ID = '" + job_id + "'";
                SqlCommand cmd = new SqlCommand(str_cmd, con);
                SqlDataReader dr = cmd.ExecuteReader();
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

        public string UpdateJobType(string job_number, string job_type)
        {
            SqlConnection con = DB.Connect();
            try
            {
                con.Open();
                string job_id = job_number.Replace("-", String.Empty).Replace(" ", String.Empty);
                string str_cmd = "Update Job Set Job_Type = '" + job_type + "' where Job_ID = '" + job_id +"'";
                SqlCommand cmd = new SqlCommand(str_cmd, con);
                SqlDataReader dr = cmd.ExecuteReader();
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

        public string UpdateJobBudget(string job_number, int job_budget)
        {
            SqlConnection con = DB.Connect();
            try
            {
                con.Open();
                string job_id = job_number.Replace("-", String.Empty).Replace(" ", String.Empty);
                string str_cmd = "Update Job Set Estimated_Budget = '" + job_budget + "' where Job_ID = '" + job_id + "'";
                SqlCommand cmd = new SqlCommand(str_cmd, con);
                SqlDataReader dr = cmd.ExecuteReader();
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

        public string AddJob(string job_number, string job_name, int job_year, string job_type)
        {
            SqlConnection con = DB.Connect();
            try
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Job(Job_ID, Job_Number, Job_Name, Job_Year, Job_Type) " +
                                                   "VALUES(@Job_ID, @Job_Number, @Job_Name, @Job_Year, @Job_Type)", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Job_Number", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Job_Name", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Job_Year", SqlDbType.Int);
                    cmd.Parameters.Add("@Job_Type", SqlDbType.NVarChar);
                    cmd.Parameters[0].Value = job_number.Replace("-", String.Empty).Replace(" ", String.Empty);
                    cmd.Parameters[1].Value = job_number;
                    cmd.Parameters[2].Value = job_name;
                    cmd.Parameters[3].Value = job_year;
                    cmd.Parameters[4].Value = job_type;
                    cmd.ExecuteNonQuery();
                }
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

        public string InsertJobs(List<JobModel> import_jobs)
        {
            SqlConnection con = DB.Connect();
            try
            {
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

                    for (int i = 0; i < import_jobs.Count; i++)
                    {
                        cmd.Parameters[0].Value = import_jobs[i].job_id.Replace("-",String.Empty).Replace(" ",String.Empty);
                        cmd.Parameters[1].Value = import_jobs[i].job_number;
                        cmd.Parameters[2].Value = import_jobs[i].job_name;
                        cmd.Parameters[3].Value = import_jobs[i].job_year;
                        cmd.ExecuteNonQuery();
                    }
                }
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
