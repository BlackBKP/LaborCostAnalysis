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
        IJob JobInterface;

        static List<JobModel> import_jobs;
        static List<JobModel> excel_duplicated_jobs;
        static List<JobModel> duplicated_jobs;

        public ImportJobController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            this.JobInterface = new JobService();
        }

        public IActionResult Index()
        {
            return HttpContext.Session.GetString("LoginStatus") == "LoggedIn" ? View() : (IActionResult)RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public JsonResult GetJobs()
        {
            List<JobModel> jobs = JobInterface.GetJobs();
            return Json(jobs);
        }

        public JsonResult Import()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "files";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);

            import_jobs = new List<JobModel>();
            excel_duplicated_jobs = new List<JobModel>();
            duplicated_jobs = new List<JobModel>();
            List<JobModel> duplicate_excel = new List<JobModel>();

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
                    int count = import_jobs.Where(w => w.job_id == job.job_id).Select(s => s).Count();
                    if (count == 0)
                        import_jobs.Add(job);
                    else
                        excel_duplicated_jobs.Add(job);
                }
            }

            List<List<JobModel>> list_jobs = new List<List<JobModel>>();
            List<JobModel> jobs = JobInterface.GetJobs();
            duplicated_jobs = import_jobs.Where(w => jobs.Any(a => a.job_id == w.job_id)).ToList();
            import_jobs = import_jobs.GroupBy(g => g.job_id).Select(s => s.FirstOrDefault()).Where(w => !duplicated_jobs.Any(a => a.job_id == w.job_id)).ToList();
            list_jobs.Add(import_jobs);
            list_jobs.Add(excel_duplicated_jobs);
            list_jobs.Add(duplicated_jobs);
            return Json(list_jobs);
        }

        [HttpPost]
        public JsonResult ConfirmImport()
        {
            string result = JobInterface.InsertJobs(import_jobs);
            return Json(result);
        }
    }
}
