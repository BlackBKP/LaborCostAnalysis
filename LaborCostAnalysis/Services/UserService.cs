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

            string str_cmd = "select User_ID," +
                                    "User_Name, " +
                                    "Permission " +
                                    "from User_Authentication";

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
                    };
                    users.Add(user);
                }
                dr.Close();
            }

            return users;
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
    }
}
