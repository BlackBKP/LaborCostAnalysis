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
using System.Text;
using System.Threading.Tasks;

namespace LaborCostAnalysis.Controllers
{
    public class ImportLaborCostController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        ILaborCost LaborCostInterface;

        static List<LaborCostModel> import_costs;
        static List<LaborCostModel> duplicate_costs;

        public ImportLaborCostController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            this.LaborCostInterface = new LaborCostService();
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult ImportSpent()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "files";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            import_costs = new List<LaborCostModel>();
            duplicate_costs = new List<LaborCostModel>();
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
                IRow headerRow = sheet.GetRow(2);
                int cellCount = headerRow.LastCellNum;
                IRow row;
                for (int i = 3; i <= sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);
                    if (row.GetCell(8).StringCellValue == "")
                        break;
                    if (row == null)
                        break;
                    if (row.Cells.All(d => d.CellType == CellType.Blank))
                        break;
                    if (row.Cells.All(c => c.NumericCellValue == 0))
                        break;

                    LaborCostModel job = new LaborCostModel()
                    {
                        job_id = row.GetCell(8).StringCellValue.Replace("-",String.Empty).Replace(" ",String.Empty),
                        job_name = row.GetCell(9).StringCellValue,
                        week = Convert.ToInt32(row.GetCell(5).NumericCellValue),
                        month = Convert.ToInt32(row.GetCell(6).NumericCellValue),
                        year = Convert.ToInt32(row.GetCell(7).NumericCellValue),
                        week_time = row.GetCell(10).StringCellValue,
                        labor_cost = Convert.ToInt32(row.GetCell(11).NumericCellValue),
                        ot_cost = Convert.ToInt32(row.GetCell(12).NumericCellValue),
                        accomodate = Convert.ToInt32(row.GetCell(13).NumericCellValue),
                        compensate = Convert.ToInt32(row.GetCell(14).NumericCellValue),
                        social_security = Convert.ToInt32(row.GetCell(15).NumericCellValue),
                        number_of_labor = Convert.ToInt32(row.GetCell(16).NumericCellValue)
                    };
                    import_costs.Add(job);
                }
            }
            List<List<LaborCostModel>> list_labor_costs = new List<List<LaborCostModel>>();
            List<LaborCostModel> labor_costs = LaborCostInterface.GetLaborCosts();
            duplicate_costs = import_costs.Where(w => labor_costs.Any(a => a.job_id == w.job_id && a.week == w.week && a.month == w.month && a.year == w.year)).ToList();
            import_costs = import_costs.Where(w => !duplicate_costs.Any(a => a.job_id == w.job_id && a.week == w.week && a.month == w.month && a.year == w.year)).ToList();
            list_labor_costs.Add(import_costs);
            list_labor_costs.Add(duplicate_costs);
            return Json(list_labor_costs);
        }

        [HttpPost]
        public JsonResult ConfirmUpload()
        {
            string result = LaborCostInterface.InsertLaborCosts(import_costs);
            return Json(result);
        }
    }
}
