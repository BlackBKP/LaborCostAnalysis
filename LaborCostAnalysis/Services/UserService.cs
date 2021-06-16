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
    public class UserService : IUser
    {
        IConnectDB DB;

        public UserService()
        {
            this.DB = new ConnectDB();
        }

        public List<UserAuthenticationModel> GetUsers()
        {
            List<UserAuthenticationModel> users = new List<UserAuthenticationModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select User_Authentication.User_ID, " +
                                    "User_Authentication.User_Name, " +
                                    "User_Authentication.Permission, " +
                                    "Count(User_Accessibility.Job_ID) as Jobs " +
                             "from User_Authentication " +
                             "left join User_Accessibility on User_Authentication.User_ID = User_Accessibility.User_ID " +
                             "Group by User_Authentication.User_ID, User_Authentication.User_Name, User_Authentication.Permission";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    UserAuthenticationModel user = new UserAuthenticationModel()
                    {
                        user_id = dr["User_ID"] != DBNull.Value ? dr["User_ID"].ToString() : "",
                        user_name = dr["User_Name"] != DBNull.Value ? dr["User_Name"].ToString() : "",
                        permission = dr["Permission"] != DBNull.Value ? dr["Permission"].ToString() : "",
                        jobs = dr["Jobs"] != DBNull.Value ? Convert.ToInt32(dr["Jobs"]) : 0
                    };
                    users.Add(user);
                }
                dr.Close();
            }
            return users;
        }

        public UserAuthenticationModel GetUserAuthentication(string user_name)
        {
            UserAuthenticationModel user_authen = new UserAuthenticationModel();

            SqlConnection con = DB.Connect();
            con.Open();
            string str_cmd = "select TOP 1 User_Accessibility.User_ID, " +
                                    "User_Authentication.User_Name, " +
                                    "User_Authentication.Permission " +
                             "from User_Accessibility " +
                             "left join User_Authentication ON User_Authentication.User_ID = User_Accessibility.User_ID " +
                             "where User_Authentication .User_Name ='" + user_name + "'";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    user_authen.user_id = dr["User_ID"] != DBNull.Value ? dr["User_ID"].ToString() : "";
                    user_authen.user_name = dr["User_Name"] != DBNull.Value ? dr["User_Name"].ToString() : "";
                    user_authen.permission = dr["Permission"] != DBNull.Value ? dr["Permission"].ToString() : "";
                }
                dr.Close();
            }
            return user_authen;
        }

        public string AddUser(string user_id,string user_name, string role)
        {
            SqlConnection con = DB.Connect();
            try
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO User_Authentication(User_ID, User_Name, Permission) " +
                                                       "VALUES(@User_ID, @User_Name, @Permission)", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@User_ID", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@User_Name", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Permission", SqlDbType.NVarChar);
                    cmd.Parameters[0].Value = user_id;
                    cmd.Parameters[1].Value = user_name;
                    cmd.Parameters[2].Value = role;
                    cmd.ExecuteNonQuery();
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

        public string EditUser(string user_id, string user_name, string role)
        {
            SqlConnection con = DB.Connect();
            try
            {
                con.Open();
                string str_cmd = "Update User_Authentication Set User_Name = '" + user_name + "', Permission = '" + role + "' where User_ID = '" + user_id + "'";
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

        public List<JobAccessibilityModel> GetJobAccessibility(string user_id)
        {
            List<JobAccessibilityModel> jobs_accessibility = new List<JobAccessibilityModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Job.Job_ID, " +
                                     "Job.Job_Number, " +
                                     "Job.Job_Name, " +
                                     "case when s1.Job_ID is null then 0 else 1 end as Job_Available " +
                             "from Job " +
                             "left join ( select User_ID, Job_ID from User_Accessibility where User_Accessibility.User_ID = '" + user_id + "' ) as s1 on Job.Job_ID = s1.Job_ID";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    JobAccessibilityModel acc = new JobAccessibilityModel()
                    {
                        job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "",
                        job_number = dr["Job_Number"] != DBNull.Value ? dr["Job_Number"].ToString() : "",
                        job_name = dr["Job_Name"] != DBNull.Value ? dr["Job_Name"].ToString() : "",
                        job_available = dr["Job_Available"] != DBNull.Value ? Convert.ToInt32(dr["Job_Available"]) : 0
                    };
                    jobs_accessibility.Add(acc);
                }
                dr.Close();
            }
            return jobs_accessibility;
        }

        public string UpdateAccessibility(string user_id, string[] job_acc)
        {
            SqlConnection con = DB.Connect();
            try
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM User_Accessibility WHERE User_ID = '" + user_id + "'", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                using (SqlCommand cmd = new SqlCommand("INSERT INTO User_Accessibility (User_ID, Job_ID) VALUES(@User_ID, @Job_ID)", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@User_ID", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);

                    for (int i = 0; i < job_acc.Count(); i++)
                    {
                        cmd.Parameters[0].Value = user_id;
                        cmd.Parameters[1].Value = job_acc[i].Replace("-",String.Empty).Replace(" ",String.Empty);
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

        public string AddJobAccessibility(string user_id, string job_id)
        {
            SqlConnection con = DB.Connect();
            try
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO User_Accessibility (User_ID, Job_ID) VALUES(@User_ID, @Job_ID)", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@User_ID", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);
                    cmd.Parameters[0].Value = user_id;
                    cmd.Parameters[1].Value = job_id.Replace("-",String.Empty).Replace(" ",String.Empty);
                    cmd.ExecuteNonQuery();
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

        public string RemoveJobAccessibility(string user_id, string job_id)
        {
            SqlConnection con = DB.Connect();
            try
            {
                con.Open();
                string str_cmd = "DELETE FROM User_Accessibility WHERE User_ID = '" + user_id + "' AND Job_ID = '" + job_id.Replace("-", String.Empty).Replace(" ", String.Empty) + "'";
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
