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
    public class ImportProgressController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        IJob JobInterface;
        IProgress ProgressInterface;

        static int year;
        static string month;
        static List<ProgressModel> ipgs;
        static List<ProgressModel> excel_duplicate_pgs;
        static List<ProgressModel> duplicate_pgs;

        public ImportProgressController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            this.JobInterface = new JobService();
            this.ProgressInterface = new ProgressService();
        }

        public IActionResult Index()
        {
            return HttpContext.Session.GetString("LoginStatus") == "LoggedIn" ? View() : (IActionResult)RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public JsonResult GetData()
        {
            List<ProgressModel> pgs = ProgressInterface.GetProgressViewModels();
            List<List<ProgressModel>> lpgs = new List<List<ProgressModel>>();
            string[] job_id = pgs.Select(s => s.job_id).Distinct().ToArray();
            for (int i = 0; i < job_id.Count(); i++)
            {
                lpgs.Add(pgs.Where(w => w.job_id == job_id[i]).Select(s => s).OrderBy(y => y.year).ThenBy(m => m.month).ToList());
            }
            return Json(lpgs);
        }

        [HttpPost]
        public void SetJobDetails(int y, string m)
        {
            year = y;
            month = m;
        }

        public JsonResult Import()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "files";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            ipgs = new List<ProgressModel>();
            excel_duplicate_pgs = new List<ProgressModel>();
            duplicate_pgs = new List<ProgressModel>();
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
                for (int i = 1; i < sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);
                    if (row == null)
                        break;
                    if (row.Cells.All(d => d.CellType == CellType.Blank))
                        break;
                    if (row.GetCell(0).CellType == CellType.Blank)
                        break;

                    ProgressModel ipg = new ProgressModel();
                    DateTime update_time = DateTime.Now;

                    ipg.job_id = row.GetCell(0).StringCellValue.Replace("-", String.Empty).Replace(" ", String.Empty);
                    ipg.estimated_budget = Convert.ToInt32(row.GetCell(1).NumericCellValue);
                    ipg.job_progress = Convert.ToInt32(row.GetCell(2).NumericCellValue);
                    ipg.year = year;
                    string[] months = new string[] { "", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
                    ipg.month = Array.IndexOf(months, month);
                    int count = ipgs.Where(w => w.job_id == ipg.job_id && w.year == ipg.year && w.month == ipg.month).Select(s => s).Count();
                    if (count == 0)
                        ipgs.Add(ipg);
                    else
                        excel_duplicate_pgs.Add(ipg);
                }
            }
            List<ProgressModel> progress = ProgressInterface.GetProgressViewModels();
            duplicate_pgs = ipgs.Where(w => progress.Any(a => a.job_id == w.job_id && a.year == w.year && a.month == w.month)).ToList();
            ipgs = ipgs.Where(w => !progress.Any(a => a.job_id == w.job_id && a.year == w.year && a.month == w.month)).ToList();
            List<List<ProgressModel>> list_progress = new List<List<ProgressModel>>();
            list_progress.Add(ipgs);
            list_progress.Add(excel_duplicate_pgs);
            list_progress.Add(duplicate_pgs);
            return Json(list_progress);
        }

        [HttpPost]
        public JsonResult ConfirmImport()
        {
            string result = ProgressInterface.InsertProgress(ipgs);
            result = ProgressInterface.UpdateDuplicateProgress(duplicate_pgs);
            return Json(result);
        }

        [HttpPost]
        public JsonResult EditProgress(string job_number, int job_year, string job_month, string job_budget, int job_progress)
        {
            string[] months = new string[] { "", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            try
            {
                ProgressModel progress = new ProgressModel()
                {
                    job_id = job_number.Replace("-", String.Empty).Replace(" ", String.Empty),
                    job_number = job_number,
                    month = Array.IndexOf(months, job_month),
                    year = job_year,
                    job_progress = job_progress,
                    estimated_budget = Convert.ToInt32(job_budget.Replace(",", String.Empty))
                };
                string result = ProgressInterface.UpdateProgress(progress);
                return Json(result);
            }
            catch(Exception ex)
            {
                return Json(ex);
            }
        }
    }
}
