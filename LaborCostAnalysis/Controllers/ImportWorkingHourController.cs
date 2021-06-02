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
        IJob JobInterface;
        IWorkingHour WorkingHourInterface;

        static string job_id;
        static int year;
        static string month;

        public ImportWorkingHourController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            this.JobInterface = new JobService();
            this.WorkingHourInterface = new WorkingHourService();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetJobNumbers()
        {
            var job_ids = JobInterface.GetJobs().OrderByDescending(o => o.job_id).Select(s => s.job_id).ToList();
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
                    if (row.GetCell(4).CellType == CellType.Blank)
                        break;
                    if (row.GetCell(4).NumericCellValue == 0)
                        break;
                    WorkingHoursModel wh = new WorkingHoursModel();
                    wh.job_id = job_id;
                    wh.employee_id = row.GetCell(4).StringCellValue;
                    wh.working_day = row.GetCell(5).DateCellValue;
                    wh.week = Convert.ToInt32(month.Split(' ')[1]);
                    string[] months = new string[] { "", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
                    wh.month = Array.IndexOf(months, month.Split(' ')[0]);
                    wh.hours = Convert.ToInt32(row.GetCell(6).StringCellValue.Split(':')[0]);
                    working_hours.Add(wh);
                }
            }
            List<WorkingHoursModel> whs = WorkingHourInterface.GetWorkingHours(job_id);
            working_hours = working_hours.Where(w => !whs.Any(a => a.job_id == w.job_id && a.employee_id == w.employee_id && a.month == w.month && a.week == w.week)).ToList();
            return Json(working_hours);
        }

        [HttpPost]
        public JsonResult ConfirmImport()
        {
            string result = WorkingHourInterface.InsertWorkingHours(working_hours);
            return Json(result);
        }
    }
}
