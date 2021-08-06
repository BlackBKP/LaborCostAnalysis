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
    public class ProgressService : IProgress
    {
        IConnectDB DB;

        public ProgressService()
        {
            this.DB = new ConnectDB();
        }

        public List<JobSummaryModel> GetJobProgress(string job_id)
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
                                    "s1.Cost_to_Date, (cast(job.Estimated_Budget as int) - cast(s1.Cost_to_Date as int)) as Remaining_Cost, " +
                                    "((cast(s1.Cost_to_Date as float) / cast(job.Estimated_Budget as float)) *100) as Cost_Usage, " +
                                    "s4.Last_Progress as Work_Completion, " +
                                    "s4.Last_Invoice as Invoice, " +
                                    "s2.Hours, " +
                                    "s3.OT_1_5, " +
                                    "s3.OT_3, " +
                                    "(s2.Hours + s3.OT_1_5 + s3.OT_3) as Total_Man_Hour, " +
                                    "s5.No_Of_Labor_Week as No_Of_Labor, " +
                                    "(cast(s1.Cost_to_Date as float) / (s2.Hours + s3.OT_1_5 + s3.OT_3)) as avg_labor_cost_per_hour " +
                                    "from job " +
                                    "left join (select job_ID, SUM(cast(Labor_Cost as int))as Labor_Cost, SUM(cast(OT_Labor_Cost as int)) as OT_Labor_Cost, SUM(cast(Accommodation_Cost as int)) as Accommodation_Cost, SUM(cast(Compensation_Cost as int)) as Compensation_Cost, SUM(isnull(Social_Security,0)) as Social_Security, (SUM(cast(Labor_Cost as int)) + SUM(cast(OT_Labor_Cost as int)) + SUM(cast(Accommodation_Cost as int)) + SUM(cast(Compensation_Cost as int)) + SUM(isnull(Social_Security,0))) as Cost_to_Date from Labor_Costs group by job_ID) as s1 ON s1.job_ID = job.job_ID " +
                                    "left join (select job_ID,SUM(Hours) as Hours from Hour group by Job_ID) as s2 ON s2.job_ID = job.job_ID " +
                                    "left join (select job_ID,SUM(OT_1_5) as OT_1_5 , SUM(OT_3) as OT_3 from OT group by job_ID) as s3 ON s3.job_ID = job.job_ID " +
                                    "left join (select Job_ID,Max(cast(Job_Progress as int)) as Last_Progress,Max(Invoice) as Last_Invoice from Progress group by Job_ID) as s4 ON s4.Job_ID = job.Job_ID " +
                                    "left join (select Job_ID,Max(cast(No_Of_Labor_Week as int)) as No_Of_Labor_Week from Labor_Costs group by Job_ID) as s5 ON s5.Job_ID = job.Job_ID " +
                                    "where job.Job_ID = '" + job_id + "'";

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
                        hours = dr["Hours"] != DBNull.Value ? Convert.ToInt32(dr["Hours"]) : 0,
                        ot_1_5 = dr["OT_1_5"] != DBNull.Value ? Convert.ToInt32(dr["OT_1_5"]) : 0,
                        ot_3 = dr["OT_3"] != DBNull.Value ? Convert.ToInt32(dr["OT_3"]) : 0,
                        total_man_hour = dr["Total_Man_Hour"] != DBNull.Value ? Convert.ToInt32(dr["Total_Man_Hour"]) : 0,
                        no_of_labor = dr["No_Of_Labor"] != DBNull.Value ? Convert.ToInt32(dr["No_Of_Labor"]) : 0,
                        avg_labor_cost_per_hour = dr["avg_labor_cost_per_hour"] != DBNull.Value ? Convert.ToInt32(dr["avg_labor_cost_per_hour"]) : 0,
                        invoice = dr["Invoice"] != DBNull.Value ? Convert.ToInt32(dr["Invoice"]) : 0,
                    };
                    jobs.Add(job);
                }
                dr.Close();
            }
            con.Close();
            return jobs;
        }

        public List<ProgressModel> GetProgressViewModels()
        {
            List<ProgressModel> pgs = new List<ProgressModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Progress.Job_ID, " +
                                    "job.Job_Number, " +
                                    "job.Job_Name, " +
                                    "job.Estimated_Budget, " +
                                    "Progress.Job_Progress, " +
                                    "Progress.Invoice, " +
                                    "Progress.Month, " +
                                    "Progress.Year, " +
                                    "((cast(s1.Labor_Cost as int) +cast(s1.OT_Labor_Cost as int) + cast(s1.Accommodation_Cost as int) + cast(s1.Compensation_Cost as int) + isnull(s1.Social_Security,0))) as spent_cost, " +
                                    "sum((cast(s1.Labor_Cost as int) + cast(s1.OT_Labor_Cost as int) + cast(s1.Accommodation_Cost as int) + cast(s1.Compensation_Cost as int) + isnull(s1.Social_Security,0))) OVER(PARTITION BY s1.job_ID ORDER BY s1.job_ID ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) as acc_spent_cost, " +
                                    "(job.Estimated_Budget - sum((cast(s1.Labor_Cost as int) + cast(s1.OT_Labor_Cost as int) + cast(s1.Accommodation_Cost as int) + cast(s1.Compensation_Cost as int) + isnull(s1.Social_Security,0))) OVER(PARTITION BY s1.job_ID ORDER BY s1.job_ID ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)) as remaining_cost " +
                                    "from Progress " +
                                    "left join job on job.Job_ID = Progress.Job_ID " +
                                    "left join ( select Job_ID,Month,Year,sum(cast(Labor_Cost as int)) as Labor_Cost,sum(cast(OT_Labor_Cost as int)) as OT_Labor_Cost,sum(cast(Accommodation_Cost as int)) as Accommodation_Cost,sum(cast(Compensation_Cost as int)) as Compensation_Cost,sum(isnull(Social_Security,0)) as Social_Security " +
                                    "from Labor_Costs group by Job_ID,Year,Month) as s1 ON s1.Job_ID = Progress.Job_ID and s1.Year = Progress.Year and s1.Month = Progress.Month";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ProgressModel pg = new ProgressModel()
                    {
                        job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "",
                        job_number = dr["Job_Number"] != DBNull.Value ? dr["Job_Number"].ToString() : "",
                        job_name = dr["Job_Name"] != DBNull.Value ? dr["Job_Name"].ToString() : "",
                        estimated_budget = dr["Estimated_Budget"] != DBNull.Value ? Convert.ToInt32(dr["Estimated_Budget"]) : 0,
                        job_progress = dr["Job_Progress"] != DBNull.Value ? Convert.ToInt32(dr["Job_Progress"]) : 0,
                        month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0,
                        year = dr["Year"] != DBNull.Value ? Convert.ToInt32(dr["Year"]) : 0,
                        spent_cost = dr["spent_cost"] != DBNull.Value ? Convert.ToInt32(dr["spent_cost"]) : 0,
                        remainning_cost = dr["remaining_cost"] != DBNull.Value ? Convert.ToInt32(dr["remaining_cost"]) : 0,
                        invoice = dr["Invoice"] != DBNull.Value ? Convert.ToInt32(dr["Invoice"]) : 0,
                    };
                    pgs.Add(pg);
                }
                dr.Close();
            }
            con.Close();
            return pgs;
        }

        public List<ProgressModel> GetProgressViewModelsByUser(string user_name)
        {
            List<ProgressModel> pgs = new List<ProgressModel>();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select s2.Job_ID, " +
                                    "job.Job_Number, " +
                                    "job.Job_Name, " +
                                    "job.Estimated_Budget, " +
                                    "Progress.Job_Progress, " +
                                    "Progress.Invoice, " +
                                    "Progress.Month, " +
                                    "Progress.Year, " +
                                    "((cast(s1.Labor_Cost as int) + cast(s1.OT_Labor_Cost as int) + cast(s1.Accommodation_Cost as int) + cast(s1.Compensation_Cost as int) + isnull(s1.Social_Security,0))) as spent_cost, " +
                                    "sum((cast(s1.Labor_Cost as int) + cast(s1.OT_Labor_Cost as int) + cast(s1.Accommodation_Cost as int) + cast(s1.Compensation_Cost as int) + isnull(s1.Social_Security,0))) OVER(PARTITION BY s1.job_ID ORDER BY s1.job_ID ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) as acc_spent_cost, " +
                                    "(job.Estimated_Budget - sum((cast(s1.Labor_Cost as int) + cast(s1.OT_Labor_Cost as int) + cast(s1.Accommodation_Cost as int) + cast(s1.Compensation_Cost as int) + isnull(s1.Social_Security,0))) OVER(PARTITION BY s1.job_ID ORDER BY s1.job_ID ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)) as remaining_cost " +
                              "from Progress " +
                              "left join job on job.Job_ID = Progress.Job_ID " +
                              "left join ( select " +
                                                "Job_ID, " +
                                                "Month, " +
                                                "Year, " +
                                                "sum(cast(Labor_Cost as int)) as Labor_Cost, " +
                                                "sum(cast(OT_Labor_Cost as int)) as OT_Labor_Cost, " +
                                                "sum(cast(Accommodation_Cost as int)) as Accommodation_Cost, " +
                                                "sum(cast(Compensation_Cost as int)) as Compensation_Cost, " +
                                                "sum(isnull(Social_Security,0)) as Social_Security " +
                                           "from Labor_Costs " +
                                           "group by Job_ID, Year, Month ) as s1 ON s1.Job_ID = Progress.Job_ID and s1.Year = Progress.Year and s1.Month = Progress.Month " +
                              "left join( select " +
                                                "User_Accessibility.User_ID, " +
                                                "User_Authentication.User_Name, " +
                                                "User_Authentication.Permission, " +
                                                "User_Accessibility.Job_ID " +
                                          "from User_Accessibility " +
                                          "left join User_Authentication ON User_Authentication.User_ID = User_Accessibility.User_ID " +
                                          "where User_Authentication.User_Name='" + user_name + "') as s2 ON s2.Job_ID = Progress.Job_ID " +
                              "where s2.Job_ID is not null";

            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ProgressModel pg = new ProgressModel()
                    {
                        job_id = dr["Job_ID"] != DBNull.Value ? dr["Job_ID"].ToString() : "",
                        job_number = dr["Job_Number"] != DBNull.Value ? dr["Job_Number"].ToString() : "",
                        job_name = dr["Job_Name"] != DBNull.Value ? dr["Job_Name"].ToString() : "",
                        estimated_budget = dr["Estimated_Budget"] != DBNull.Value ? Convert.ToInt32(dr["Estimated_Budget"]) : 0,
                        job_progress = dr["Job_Progress"] != DBNull.Value ? Convert.ToInt32(dr["Job_Progress"]) : 0,
                        month = dr["Month"] != DBNull.Value ? Convert.ToInt32(dr["Month"]) : 0,
                        year = dr["Year"] != DBNull.Value ? Convert.ToInt32(dr["Year"]) : 0,
                        spent_cost = dr["spent_cost"] != DBNull.Value ? Convert.ToInt32(dr["spent_cost"]) : 0,
                        remainning_cost = dr["remaining_cost"] != DBNull.Value ? Convert.ToInt32(dr["remaining_cost"]) : 0,
                        invoice = dr["Invoice"] != DBNull.Value ? Convert.ToInt32(dr["Invoice"]) : 0,
                    };
                    pgs.Add(pg);
                }
                dr.Close();
            }
            con.Close();
            return pgs;
        }

        public string InsertProgress(List<ProgressModel> progress)
        {
            SqlConnection con = DB.Connect();
            try
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Progress(" +
                                                                    "Job_ID, " +
                                                                    "Job_Progress, " +
                                                                    "Invoice, " +
                                                                    "Month, " +
                                                                    "Year) " +
                                                         "VALUES(@Job_ID," +
                                                                "@Job_Progress, " +
                                                                "@Invoice, " +
                                                                "@Month, " +
                                                                "@Year)", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Job_Progress", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Invoice", SqlDbType.Int);
                    cmd.Parameters.Add("@Month", SqlDbType.Int);
                    cmd.Parameters.Add("@Year", SqlDbType.Int);

                    for (int i = 0; i < progress.Count; i++)
                    {
                        cmd.Parameters[0].Value = progress[i].job_id.Replace("-",String.Empty).Replace(" ",String.Empty);
                        cmd.Parameters[1].Value = progress[i].job_progress;
                        cmd.Parameters[2].Value = progress[i].invoice;
                        cmd.Parameters[3].Value = progress[i].month;
                        cmd.Parameters[4].Value = progress[i].year;
                        cmd.ExecuteNonQuery();
                    }
                }
                using (SqlCommand cmd = new SqlCommand("UPDATE Job SET Estimated_Budget = @Estimated_Budget WHERE Job_ID = @Job_ID", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@Estimated_Budget", SqlDbType.Int);
                    cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);

                    for (int i = 0; i < progress.Count; i++)
                    {
                        cmd.Parameters[0].Value = progress[i].estimated_budget;
                        cmd.Parameters[1].Value = progress[i].job_id;
                        cmd.ExecuteNonQuery();
                    }
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

        public string InsertProgressInvoice(List<ProgressModel> progress)
        {
            SqlConnection con = DB.Connect();
            try
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Progress(" +
                                                                    "Job_ID, " +
                                                                    "Job_Progress, " +
                                                                    "Invoice, " +
                                                                    "Month, " +
                                                                    "Year) " +
                                                         "VALUES(@Job_ID," +
                                                                "@Job_Progress, " +
                                                                "@Invoice, " +
                                                                "@Month, " +
                                                                "@Year)", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Job_Progress", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Invoice", SqlDbType.Int);
                    cmd.Parameters.Add("@Month", SqlDbType.Int);
                    cmd.Parameters.Add("@Year", SqlDbType.Int);

                    for (int i = 0; i < progress.Count; i++)
                    {
                        cmd.Parameters[0].Value = progress[i].job_id.Replace("-", String.Empty).Replace(" ", String.Empty);
                        cmd.Parameters[1].Value = progress[i].job_progress;
                        cmd.Parameters[2].Value = progress[i].invoice;
                        cmd.Parameters[3].Value = progress[i].month;
                        cmd.Parameters[4].Value = progress[i].year;
                        cmd.ExecuteNonQuery();
                    }
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

        public string UpdateDuplicateProgress(List<ProgressModel> progress)
        {
            SqlConnection con = DB.Connect();
            try
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE Progress SET Job_Progress = @Job_Progress " +
                                                       "WHERE Job_ID = @Job_ID AND Month = @Month AND Year = @Year", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@Job_Progress", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Month", SqlDbType.Int);
                    cmd.Parameters.Add("@Year", SqlDbType.Int);

                    for (int i = 0; i < progress.Count; i++)
                    {
                        cmd.Parameters[0].Value = progress[i].job_progress;
                        cmd.Parameters[1].Value = progress[i].job_id;
                        cmd.Parameters[2].Value = progress[i].month;
                        cmd.Parameters[3].Value = progress[i].year;
                        cmd.ExecuteNonQuery();
                    }
                }
                using (SqlCommand cmd = new SqlCommand("UPDATE Job SET Estimated_Budget = @Estimated_Budget WHERE Job_ID = @Job_ID", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@Estimated_Budget", SqlDbType.Int);
                    cmd.Parameters.Add("@Job_ID", SqlDbType.NVarChar);

                    for (int i = 0; i < progress.Count; i++)
                    {
                        cmd.Parameters[0].Value = progress[i].estimated_budget;
                        cmd.Parameters[1].Value = progress[i].job_id;
                        cmd.ExecuteNonQuery();
                    }
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

        public string UpdateProgress(ProgressModel progress)
        {
            SqlConnection con = DB.Connect();
            try
            {
                con.Open();
                string str_update_progress = "UPDATE Progress SET Job_Progress = " + progress.job_progress + " WHERE Job_ID = '" + progress.job_id + 
                                             "' AND Month = " + progress.month + " AND Year = " + progress.year;
                SqlCommand cmd_update_progress = new SqlCommand(str_update_progress, con);
                cmd_update_progress.ExecuteNonQuery();

                string str_update_budget = "UPDATE Job SET Estimated_Budget = " + progress.estimated_budget + " WHERE Job_ID = '" + progress.job_id + "'";
                SqlCommand cmd_update_budget = new SqlCommand(str_update_budget, con);
                cmd_update_budget.ExecuteNonQuery();
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
