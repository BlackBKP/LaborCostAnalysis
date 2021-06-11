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
    public class ImportOvertimeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        IJob JobInterface;
        IOvertime OvertimeInterface;

        static List<OvertimeModel> ots;
        static List<OvertimeModel> excel_duplicate_ots;
        static List<OvertimeModel> duplicate_ots;
        static List<OvertimeModel> delete_overtimes;

        static string job_id;
        static int year;
        static string month;

        public ImportOvertimeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            this.JobInterface = new JobService();
            this.OvertimeInterface = new OvertimeService();
        }

        public IActionResult Index()
        {
            return HttpContext.Session.GetString("LoginStatus") == "LoggedIn" ? View() : (IActionResult)RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public JsonResult GetJobNumbers()
        {
            return Json(JobInterface.GetJobs().OrderByDescending(o => o.job_id).ToList());
        }

        [HttpGet]
        public JsonResult GetJobNumbersDelete()
        {
            var job_ids = JobInterface.GetJobs().OrderByDescending(o => o.job_id).ToList();
            return Json(job_ids);
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
            ots = new List<OvertimeModel>();
            excel_duplicate_ots = new List<OvertimeModel>();
            duplicate_ots = new List<OvertimeModel>();
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
                    if (row.Cells.All(d => d.CellType == CellType.Blank))
                        break;
                    if (row.GetCell(1).NumericCellValue == 0)
                        break;

                    OvertimeModel ot = new OvertimeModel();
                    DateTime rt = row.GetCell(1).DateCellValue;
                    ot.job_id = job_id;
                    ot.employee_id = row.GetCell(2).StringCellValue;
                    ot.ot_1_5 = Convert.ToInt32(row.GetCell(3).NumericCellValue);
                    ot.ot_3 = Convert.ToInt32(row.GetCell(4).NumericCellValue);
                    ot.ot_sum = ot.ot_1_5 + ot.ot_3;
                    ot.week = Convert.ToInt32(month.Split(' ')[1]);
                    string[] months = new string[] { "", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
                    ot.month = Array.IndexOf(months, month.Split(' ')[0]);
                    ot.recording_time = rt;
                    int count = ots.Where(w => w.job_id == ot.job_id && w.employee_id == ot.employee_id && w.week == ot.week && w.month == ot.month && w.recording_time == ot.recording_time).Select(s => s).Count();
                    if (count == 0)
                        ots.Add(ot);
                    else
                        excel_duplicate_ots.Add(ot);
                }
            }
            List<OvertimeModel> overtimes = OvertimeInterface.GetOvertimes(job_id);
            duplicate_ots = ots.Where(w => overtimes.Any(a => a.job_id == w.job_id && a.employee_id == w.employee_id && a.month == w.month && a.week == w.week && w.recording_time == a.recording_time)).ToList();
            ots = ots.Where(w => !overtimes.Any(a => a.job_id == w.job_id && a.employee_id == w.employee_id && a.month == w.month && a.week == w.week && w.recording_time == a.recording_time)).ToList();

            List<List<OvertimeModel>> list_overtimes = new List<List<OvertimeModel>>();
            list_overtimes.Add(ots);
            list_overtimes.Add(excel_duplicate_ots);
            list_overtimes.Add(duplicate_ots);
            return Json(list_overtimes);
        }

        [HttpPost]
        public JsonResult ConfirmImport()
        {
            string result = OvertimeInterface.InsertOvertimes(ots);
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetDataToDelete(string delete_year, string delete_id, string delete_month)
        {
            delete_overtimes = new List<OvertimeModel>();
            string[] months = new string[] { "", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            int m = Array.IndexOf(months, delete_month.Split(' ')[0]);
            int w = Convert.ToInt32(delete_month.Split(' ')[1]);
            delete_overtimes = OvertimeInterface.GetOvertimes(delete_id, delete_year, m, w);
            return Json(delete_overtimes);
        }

        [HttpGet]
        public JsonResult DeleteOvertimes(string delete_year, string delete_id, string delete_month)
        {
            string[] months = new string[] { "", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            int m = Array.IndexOf(months, delete_month.Split(' ')[0]);
            int w = Convert.ToInt32(delete_month.Split(' ')[1]);
            var result = OvertimeInterface.DeleteOvertimes(delete_id, delete_year, m, w);
            return Json(result);
        }
    }
}
