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
        IConnectDB DB;
        IJob JobInterface;
        IOvertime OvertimeInterface;

        static List<OvertimeModel> ots;
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
            return View();
        }

        [HttpGet]
        public JsonResult GetJobNumbers()
        {
            return Json(JobInterface.GetJobs().OrderByDescending(o => o.job_id).Select(s => s.job_id).ToList());
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
                    ots.Add(ot);
                }
            }
            return Json(ots);
        }

        [HttpPost]
        public JsonResult ConfirmImport()
        {
            string result = OvertimeInterface.InsertOvertimes(ots);
            return Json(result);
        }
    }
}
