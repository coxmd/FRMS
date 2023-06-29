using FarmRecordManagementSystem.Models;
using FarmRecordManagementSystem.Services;
using FastReport.Data;
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

            var reportTypes = await _farmRepository.GetAllReports();

            var viewModel = new ReportFormViewModel
            {
                Farms = farm,
                ReportTypes = reportTypes
            };

            return View(viewModel);
        }

        public async Task<IActionResult> GenerateReports(int farmId, int reportTypeId)
        {
            var webReport = new WebReport();
            var connectionString = _config.GetConnectionString("DefaultConnection");
            var inventory = await _farmRepository.GetFarmInventory(farmId);
            webReport.Report.RegisterData((System.Collections.IEnumerable)inventory, "Inventory");
            webReport.Report.Load(Path.Combine(_hostEnvironment.ContentRootPath, "Reports", GetReportFileName(reportTypeId)));

            // Modify the connection string of the report
            foreach (var connection in webReport.Report.Dictionary.Connections)
            {
                if (connection is PostgresDataConnection postgresConnection && postgresConnection.Alias == "DefaultConnection")
                {
                    postgresConnection.ConnectionString = connectionString;
                    break;
                }
            }

            return View(webReport);

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


        // public IActionResult InventoryItemsReport(int farmId)
        // {
        //     var webReport = new WebReport();
        //     var pgsqlDataConnection = new PostgresDataConnection();
        //     pgsqlDataConnection.ConnectionString = _config.GetConnectionString("DefaultConnection");
        //     webReport.Report.Dictionary.Connections.Add(pgsqlDataConnection);
        //     webReport.Report.Load(Path.Combine(_hostEnvironment.ContentRootPath, "Reports", "InventoryItems.frx"));
        //     var inventory = _farmRepository.GetFarmInventory(farmId);
        //     webReport.Report.RegisterData((System.Collections.IEnumerable)inventory, "Inventory");
        //     return View(webReport);
        // }
    }
}