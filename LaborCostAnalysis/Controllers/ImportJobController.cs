using LaborCostAnalysis.Interfaces;
using LaborCostAnalysis.Models;
using LaborCostAnalysis.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Controllers
{
    public class ImportJobController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        IConnectDB DB;
        static List<JobModel> import_jobs;

        public ImportJobController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetJobs()
        {
            List<JobModel> jobs = new List<JobModel>();
            this.DB = new ConnectDB();
            SqlConnection con = DB.Connect();
            con.Open();

            string str_cmd = "select Job_ID," +
                                    "Job_Number, " +
                                    "Job_Name, " +
                                    "Estimated_Budget, " +
                                    "Job_Year " +
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
                    };
                    jobs.Add(job);
                }
                dr.Close();
            }
            return Json(jobs);
        }

        public JsonResult Import()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "files";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            List<string> job_ids = GetJobID();
            import_jobs = new List<JobModel>();
            List<JobModel> duplicated = new List<JobModel>();
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                var stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);
                stream.Position = 0;
                if (sFileExtension == ".xls")
                {
                    HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats
                    sheet = hssfwb.GetSheetAt(0);
                }
                else
                {
                    XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format
                    sheet = hssfwb.GetSheetAt(0);
                }

                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                IRow row;
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);
                    if (row == null)
                        break;
                    if (row.Cells.All(d => d.CellType == CellType.Blank))
                        break;
                    if (row.GetCell(0).StringCellValue == "")
                        break;

                    JobModel job = new JobModel();
                    job.job_id = row.GetCell(0).StringCellValue.Replace("-", String.Empty).Replace(" ", String.Empty);
                    job.job_number = row.GetCell(0).StringCellValue.Trim();
                    job.job_name = row.GetCell(1).StringCellValue;
                    job.job_year = Convert.ToInt32(row.GetCell(2).NumericCellValue);
                    int ind = import_jobs.FindIndex(f => f.job_id == job.job_id);
                    int ind2 = job_ids.IndexOf(job.job_id);
                    if (ind < 0 && ind2 < 0)
                        import_jobs.Add(job);
                    else
                        duplicated.Add(job);
                }
            }
            List<List<JobModel>> gg = new List<List<JobModel>>();
            gg.Add(import_jobs);
            gg.Add(duplicated);
            return Json(gg);
        }

        public List<string> GetJobID()
        {
            List<string> job_ids = new List<string>();
            this.DB = new ConnectDB();
            SqlConnection con = DB.Connect();
            con.Open();
            string str_cmd = "select Job_ID from Job";
            SqlCommand cmd = new SqlCommand(str_cmd, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string id = dr["Job_ID"].ToString().Trim();
                    job_ids.Add(id);
                }
                dr.Close();
            }
            con.Close();
            return job_ids;
        }

        [HttpPost]
        public JsonResult ConfirmImport()
        {
            this.DB = new ConnectDB();
            SqlConnection con = DB.Connect();
            List<string> job_ids = GetJobID();
            List<JobModel> inserted_jobs = new List<JobModel>();
            List<JobModel> duplicated_jobs = new List<JobModel>();
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
                    cmd.Parameters[0].Value = import_jobs[i].job_id;
                    cmd.Parameters[1].Value = import_jobs[i].job_number;
                    cmd.Parameters[2].Value = import_jobs[i].job_name;
                    cmd.Parameters[3].Value = import_jobs[i].job_year;
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            return Json("Done");
        }
    }
}
