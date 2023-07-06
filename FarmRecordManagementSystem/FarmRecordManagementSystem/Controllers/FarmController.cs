﻿using FarmRecordManagementSystem.Models;
using FarmRecordManagementSystem.Services;
using FastReport;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;

namespace FarmRecordManagementSystem.Controllers
{
    public class FarmController : Controller
    {
        private IConfiguration _config;

        private readonly IFarmRepository _farmRepository;
        private readonly IWebHostEnvironment _hostEnvironment;

        public FarmController(IConfiguration config, IFarmRepository farmRepository, IWebHostEnvironment hostEnvironment)
        {
            _config = config;
            _hostEnvironment = hostEnvironment;
            _farmRepository = farmRepository;
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> GetAllFarms()
        {
            var Id = HttpContext.Session.GetInt32("Id");
            var farms = await _farmRepository.GetAllFarms(Id);
            return View(farms);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Farms farm)
        {
            var Id = HttpContext.Session.GetInt32("Id");
            await _farmRepository.CreateFarm(farm, (int)Id);
            TempData["success"] = "Farm Created Successfully";
            return RedirectToAction("GetAllFarms", new { Id = Id });
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public IActionResult AddCrops(int farmId)
        {
            var model = new Crops { FarmId = farmId };
            return View(model);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public IActionResult AddInventory(int farmId)
        {
            var model = new Inventory { FarmId = farmId };
            return View(model);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public IActionResult AddExpenses()
        {
            return View();
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public IActionResult AddTasks()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddExpenses(Expenses expense, int farmId)
        {
            await _farmRepository.AddExpenses(expense, farmId);
            TempData["success"] = "Expense Added Successfully";
            return RedirectToAction("ViewAllEpenses", new { farmId = farmId });
        }

        [HttpPost]
        public async Task<IActionResult> AddCrops(Crops crop, int farmId)
        {
            await _farmRepository.AddCrops(crop, farmId);
            TempData["success"] = "Crops Added Successfully";
            return RedirectToAction("ViewAllCrops", new { farmId = farmId });
        }

        [HttpPost]
        public async Task<IActionResult> AddInventory(Inventory inventory, int farmId)
        {
            await _farmRepository.AddInventoryItem(inventory, farmId);
            TempData["success"] = "Item Added Successfully";
            return RedirectToAction("Inventory", new { farmId = farmId });
        }

        [HttpPost]
        public async Task<IActionResult> AddTasks(Tasks task, int farmId)
        {
            await _farmRepository.AddTasks(task, farmId);
            TempData["success"] = "Task Added Successfully";
            return RedirectToAction("GetAllTasks", new { farmId = farmId });
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> ViewAllCrops(int farmId)
        {
            var crops = await _farmRepository.ViewAllCrops(farmId);
            return View(crops);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> UpdateCropDetails(int Id)
        {
            var crop = await _farmRepository.GetCropById(Id);
            if (crop == null)
            {
                return NotFound();
            }
            return View(crop);
        }

        [HttpPost, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> UpdateCropDetails(Crops crop, int farmId)
        {
            await _farmRepository.UpdateCropDetails(crop);
            TempData["success"] = "Crop Updated Successfully";
            return RedirectToAction("ViewAllCrops", new { farmId = crop.FarmId });
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> ViewAllExpenses(int farmId)
        {
            var expenses = await _farmRepository.ViewAllExpenses(farmId);
            return View(expenses);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> GetAllTasks(int farmId)
        {
            var tasks = await _farmRepository.GetAllTasks(farmId);
            return View(tasks);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("UserAuthentication");
            TempData["success"] = "Logout Successfull";
            return RedirectToAction("Login");
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> GetFarmDetails(int farmId)
        {
            var farm = await _farmRepository.GetFarmDetails(farmId);
            return View(farm);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> Inventory(int farmId)
        {
            var farm = await _farmRepository.GetFarmDetails(farmId);

            var farmInventory = await _farmRepository.GetFarmInventory(farmId);

            var viewModel = new InventoryViewModel
            {
                Farm = farm,
                Inventory = farmInventory
            };

            return View(viewModel);
        }

        // [HttpPost]
        // public IActionResult InventoryPost(int farmId)
        // {
        //     // Redirect to the inventory page with the selected farmId
        //     return RedirectToAction("Inventory", new { farmId = farmId });
        // }


        public async Task<IActionResult> SelectFarm()
        {
            var Id = HttpContext.Session.GetInt32("Id");
            var farms = await _farmRepository.GetAllFarms(Id);
            return View(farms);
        }

        [HttpPost]
        public IActionResult SelectFarm(int farmId)
        {
            return RedirectToAction("Inventory", new { farmId = farmId });
        }

        public async Task<IActionResult> Report()
        {
            var Id = HttpContext.Session.GetInt32("Id");
            var farm = await _farmRepository.GetAllFarms(Id);

            return View(farm);
        }

        public async Task<IActionResult> GenerateReports([FromServices] IWebHostEnvironment webHostEnvironment, int reportTypeId, int farmId)
        {
            // Load the report file
            string templatePath = $"Reports/InventoryItems.frx";
            string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, templatePath);
            using (Report report = new Report())
            {
                report.Load(Path.Combine(webHostEnvironment.ContentRootPath, "Reports", GetReportFileName(reportTypeId)));
                // report.Load(reportPath);


                var farmData = await _farmRepository.GetFarmInventory(farmId);

                // Pass the farm data to the report
                report.RegisterData(farmData, "public_Inventory");

                // Set the farmId parameter value
                report.SetParameterValue("farmId", (int)farmId);


                using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
                connection.Open();

                using (var stream = new MemoryStream())
                using (var export = new PDFSimpleExport())
                {
                    // Prepare the report data
                    report.Prepare();
                    export.Export(report, stream);
                    // Set the position of the MemoryStream back to the beginning
                    stream.Seek(0, SeekOrigin.Begin);
                    // Create a new MemoryStream and copy the contents of the original stream to it
                    var outputStream = new MemoryStream(stream.ToArray());

                    return new FileStreamResult(outputStream, "application/pdf");
                }
            }
        }

        private string GetReportFileName(int reportTypeId)
        {
            // Define a mapping between report type IDs and report file names
            Dictionary<int, string> reportTypeMapping = new Dictionary<int, string>()
            {
                { 1, "InventoryItems.frx" }, // Example mapping for report type ID 1
                { 2, "Crops.frx" },    // Example mapping for report type ID 2
                { 3, "Tasks.frx"},
                { 4, "Expenses.frx"}
            };

            // Retrieve the report file name based on the report type ID
            if (reportTypeMapping.TryGetValue(reportTypeId, out string reportFileName))
            {
                return reportFileName;
            }

            // Default report file name if no mapping is found
            return "DefaultReport.frx";
        }
    }
}