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
    public class ImportWorkingHourController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        static List<WorkingHoursModel> working_hours;
        IConnectDB DB;

        static string job_id;
        static int year;
        static string month;

        public ImportWorkingHourController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
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

        [HttpGet]
        public JsonResult GetJobNumbers()
        {
            return Json(GetJobID());
        }

        [HttpPost]
        public void SetJobDetails(string j, int y, string m)
        {
            job_id = j;
            year = y;
            month = m;
        }

        public JsonResult Import()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "files";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            working_hours = new List<WorkingHoursModel>();
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
                    sheet = hssfwb.GetSheetAt(1);
                }
                else
                {
                    XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format
                    sheet = hssfwb.GetSheetAt(1);
                }
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                IRow row;
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);
                    if (row == null)
                        break;
                    if (row.GetCell(5).CellType == CellType.Blank)
                        break;
                    WorkingHoursModel wh = new WorkingHoursModel();
                    wh.job_id = job_id;
                    wh.employee_id = row.GetCell(4).StringCellValue;
                    wh.working_day = row.GetCell(5).DateCellValue;
                    wh.week = Convert.ToInt32(month.Split(' ')[1]);
                    string[] months = new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
                    wh.month = Array.IndexOf(months, month.Split(' ')[0]) + 1;
                    wh.hours = Convert.ToInt32(row.GetCell(6).StringCellValue.Split(':')[0]);
                    working_hours.Add(wh);
                }
            }
            return Json(working_hours);
        }

        [HttpPost]
        public JsonResult ConfirmImport()
        {
            this.DB = new ConnectDB();
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

                for (int i = 0; i < working_hours.Count; i++)
                {
                    cmd.Parameters[0].Value = working_hours[i].job_id;
                    cmd.Parameters[1].Value = working_hours[i].employee_id;
                    cmd.Parameters[2].Value = working_hours[i].working_day;
                    cmd.Parameters[3].Value = working_hours[i].week;
                    cmd.Parameters[4].Value = working_hours[i].month;
                    cmd.Parameters[5].Value = working_hours[i].hours;
                    cmd.ExecuteNonQuery();
                }
            }
            return Json("Done");
        }
    }
}
