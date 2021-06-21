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
    public class LaborCostService : ILaborCost
    {
        IConnectDB DB;

        public LaborCostService()
        {
            this.DB = new ConnectDB();
        }

        public List<LaborCostModel> GetLaborCosts()
        {
            List<LaborCostModel> labor_costs = new List<LaborCostModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select * from Labor_Costs";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    LaborCostModel labor_cost = new LaborCostModel()
                    {
                        job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "",
                        week = dr["Week"] != DBNull.Value ? Convert.ToInt32(dr["Week"]) : 0,
                        month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0,
                        year = dr["Year"] != DBNull.Value ? Convert.ToInt32(dr["Year"]) : 0,
                        week_time = dr["Week_time"] != DBNull.Value ? dr["Week_time"].ToString() : "",
                        labor_cost = dr["Labor_Cost"] != DBNull.Value ? Convert.ToInt32(dr["Labor_Cost"]) : 0,
                        ot_cost = dr["OT_Labor_Cost"] != DBNull.Value ? Convert.ToInt32(dr["OT_Labor_Cost"]) : 0,
                        accomodate = dr["Accommodation_Cost"] != DBNull.Value ? Convert.ToInt32(dr["Accommodation_Cost"]) : 0,
                        compensate = dr["Compensation_Cost"] != DBNull.Value ? Convert.ToInt32(dr["Compensation_Cost"]) : 0,
                        social_security = dr["Social_Security"] != DBNull.Value ? Convert.ToInt32(dr["Social_Security"]) : 0,
                        number_of_labor = dr["No_Of_Labor_Week"] != DBNull.Value ? Convert.ToInt32(dr["No_Of_Labor_Week"]) : 0,
                    };
                    labor_costs.Add(labor_cost);
                }
                dr.Close();
            }
            return labor_costs;
        }

        public string InsertLaborCosts(List<LaborCostModel> labor_costs)
        {
            SqlConnection con = DB.Connect();
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Labor_Costs(" +
                                                                "Job_ID, " +
                                                                "Week, " +
                                                                "Month, " +
                                                                "Year, " +
                                                                "Week_time, " +
                                                                "Labor_Cost, " +
                                                                "OT_Labor_Cost, " +
                                                                "Accommodation_Cost, " +
                                                                "Compensation_Cost, " +
                                                                "Social_Security, " +
                                                                "No_Of_Labor_Week) " +
                                                     "VALUES(@Job_ID," +
                                                            "@Week, " +
                                                            "@Month, " +
                                                            "@Year, " +
                                                            "@Week_time, " +
                                                            "@Labor_Cost, " +
                                                            "@OT_Labor_Cost, " +
                                                            "@Accommodation_Cost, " +
                                                            "@Compensation_Cost, " +
                                                            "@Social_Security, " +
                                                            "@No_Of_Labor_Week)", con))
            {
                con.Open();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Week", SqlDbType.Int);
                cmd.Parameters.Add("@Month", SqlDbType.Int);
                cmd.Parameters.Add("@Year", SqlDbType.Int);
                cmd.Parameters.Add("@Week_time", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Labor_Cost", SqlDbType.NVarChar);
                cmd.Parameters.Add("@OT_Labor_Cost", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Accommodation_Cost", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Compensation_Cost", SqlDbType.NVarChar);
                cmd.Parameters.Add("@Social_Security", SqlDbType.Int);
                cmd.Parameters.Add("@No_Of_Labor_Week", SqlDbType.NVarChar);

                for (int i = 0; i < labor_costs.Count; i++)
                {
                    cmd.Parameters[0].Value = labor_costs[i].job_id.Replace("-",String.Empty).Replace(" ",String.Empty);
                    cmd.Parameters[1].Value = labor_costs[i].week;
                    cmd.Parameters[2].Value = labor_costs[i].month;
                    cmd.Parameters[3].Value = labor_costs[i].year;
                    cmd.Parameters[4].Value = labor_costs[i].week_time;
                    cmd.Parameters[5].Value = labor_costs[i].labor_cost;
                    cmd.Parameters[6].Value = labor_costs[i].ot_cost;
                    cmd.Parameters[7].Value = labor_costs[i].accomodate;
                    cmd.Parameters[8].Value = labor_costs[i].compensate;
                    cmd.Parameters[9].Value = labor_costs[i].social_security;
                    cmd.Parameters[10].Value = labor_costs[i].number_of_labor;
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            return "Done";
        }
    }
}
