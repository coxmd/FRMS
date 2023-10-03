using FarmRecordManagementSystem.Models;
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
        public async Task<IActionResult> Create(Farms farm, List<FarmPartitions> partitions)
        {
            var Id = HttpContext.Session.GetInt32("Id");
            await _farmRepository.CreateFarm(farm, partitions, (int)Id);
            TempData["success"] = "Farm Created Successfully";
            return RedirectToAction("GetAllFarms", new { Id = Id });
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> AddCrops(int farmId)
        {
            var farm = await _farmRepository.GetFarmDetails(farmId);
            var partitions = await _farmRepository.GetAllPartitions(farmId);
            var crops = new Crops { FarmId = farmId };

            var viewModel = new CropsFarmViewModel
            {
                Farms = farm,
                Crops = crops,
                Partitions = partitions
            };
            return View(viewModel);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public IActionResult AddPartition(int farmId)
        {
            var model = new FarmPartitions { FarmId = farmId };
            return View(model);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> AddInventory(int farmId)
        {
            var model = new Inventory { FarmId = farmId };

            List<StorageLocation> locations = await _farmRepository.ViewAllStorageLocations(farmId);
            ViewBag.Locations = locations;
            return View(model);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public IActionResult AddStorageLocation(int farmId)
        {
            var model = new StorageLocation { FarmId = farmId };
            return View(model);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> AddExpenses(int farmId)
        {
            bool farmHasPartitions = await _farmRepository.CheckPartitions(farmId);

            if (farmHasPartitions)
            {
                List<FarmPartitions> partitions = await _farmRepository.GetAllPartitions(farmId);
                ViewBag.Partitions = partitions;
            }

            ViewBag.FarmHasPartitions = farmHasPartitions;

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
            return RedirectToAction("ViewAllExpenses", new { farmId = farmId });
        }

        [HttpPost]
        public async Task<IActionResult> AddStorageLocation(StorageLocation location, int farmId)
        {
            await _farmRepository.AddStorageLocation(location, farmId);
            TempData["success"] = "Storage Location Added Successfully";
            return RedirectToAction("Inventory", new { farmId = farmId });
        }

        [HttpPost]
        public async Task<IActionResult> AddPartition(FarmPartitions partition, int farmId)
        {
            await _farmRepository.AddPartition(partition, farmId);
            TempData["success"] = "Farm Partition Added Successfully";
            return RedirectToAction("GetAllPartitions", new { farmId = farmId });
        }

        [HttpPost]
        public async Task<IActionResult> AddCrops(CropsFarmViewModel crop, int farmId)
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

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> EditInventoryItem(int Id)
        {
            var item = await _farmRepository.GetInventoryItem(Id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> EditInventoryItem(Inventory inventory, int farmId)
        {
            await _farmRepository.UpdateInventoryItem(inventory);
            TempData["success"] = "Inventory Item Updated Successfully";
            return RedirectToAction("Inventory", new { farmId = farmId });
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> EditExpense(int Id)
        {
            var expense = await _farmRepository.GetExpense(Id);
            if (expense == null)
            {
                return NotFound();
            }
            return View(expense);
        }

        [HttpPost]
        public async Task<IActionResult> EditExpense(Expenses expense, int farmId)
        {
            await _farmRepository.UpdateExpense(expense);
            TempData["success"] = "Expense Updated Successfully";
            return RedirectToAction("ViewAllExpenses", new { farmId = farmId });
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> UpdateTask(int Id)
        {
            var tasks = await _farmRepository.GetTask(Id);
            if (tasks == null)
            {
                return NotFound();
            }
            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(Tasks tasks, int farmId)
        {
            await _farmRepository.UpdateTask(tasks);
            TempData["success"] = "Task Updated Successfully";
            return RedirectToAction("GetAllTasks", new { farmId = farmId });
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
            return RedirectToAction("ViewAllCrops", new { farmId = farmId });
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> ViewAllExpenses(int farmId)
        {
            var expenses = await _farmRepository.ViewAllExpenses(farmId);
            return View(expenses);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> ViewAllStorageLocations(int farmId)
        {
            var locations = await _farmRepository.ViewAllStorageLocations(farmId);
            return View(locations);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> GetAllTasks(int farmId)
        {
            var tasks = await _farmRepository.GetAllTasks(farmId);
            return View(tasks);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> GetAllPartitions(int farmId)
        {
            var partitions = await _farmRepository.GetAllPartitions(farmId);
            return View(partitions);
        }

        [HttpPost, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> DeleteExpense(int Id, int farmId)
        {
            try
            {
                await _farmRepository.DeleteExpense(Id);
                TempData["success"] = "Expense Deleted Successfully";
            }
            catch (PostgresException ex) when (ex.SqlState == "23503")
            {
                TempData["error"] = "Cannot delete this Expense since It is linked to a farm";
            }
            catch (Exception)
            {
                TempData["error"] = "An error occurred, please try again later";
            }

            return RedirectToAction("ViewAllExpenses", new { farmId = farmId });
        }

        [HttpPost, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> DeleteCrop(int Id, int farmId)
        {
            try
            {
                await _farmRepository.DeleteCrop(Id);
                TempData["success"] = "Crop Deleted Successfully";
            }
            catch (PostgresException ex) when (ex.SqlState == "23503")
            {
                TempData["error"] = "Cannot delete this Crop since It is linked to a farm";
            }
            catch (Exception)
            {
                TempData["error"] = "An error occurred, please try again later";
            }

            return RedirectToAction("ViewAllCrops", new { farmId = farmId });
        }

        [HttpPost, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> DeleteTask(int Id, int farmId)
        {
            try
            {
                await _farmRepository.DeleteTask(Id);
                TempData["success"] = "Task Deleted Successfully";
            }
            catch (PostgresException ex) when (ex.SqlState == "23503")
            {
                TempData["error"] = "Cannot delete this Task since It is linked to a farm";
            }
            catch (Exception)
            {
                TempData["error"] = "An error occurred, please try again later";
            }

            return RedirectToAction("GetAllTasks", new { farmId = farmId });
        }

        [HttpPost, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> DeleteInventoryItem(int Id, int farmId)
        {
            try
            {
                await _farmRepository.DeleteInventoryItem(Id);
                TempData["success"] = "Item Deleted Successfully";
            }
            catch (PostgresException ex) when (ex.SqlState == "23503")
            {
                TempData["error"] = "Cannot delete this Item since It is linked to a farm";
            }
            catch (Exception)
            {
                TempData["error"] = "An error occurred, please try again later";
            }

            return RedirectToAction("Inventory", new { farmId = farmId });
        }

        public async Task<IActionResult> MarkAsFinished(Tasks task, int Id, int farmId)
        {
            await _farmRepository.MarkAsFinished(Id);
            TempData["success"] = "Task Marked as Completed";
            return RedirectToAction("GetAllTasks", new { farmId = farmId });
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

        // public async Task<IActionResult> GenerateReports([FromServices] IWebHostEnvironment webHostEnvironment, int reportTypeId, int farmId)
        // {
        //     // Load the report file
        //     string templatePath = $"Reports/InventoryItems.frx";
        //     string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, templatePath);
        //     using (Report report = new Report())
        //     {
        //         report.Load(Path.Combine(webHostEnvironment.ContentRootPath, "Reports", GetReportFileName(reportTypeId)));
        //         // report.Load(reportPath);


        //         // var farmData = await _farmRepository.GetFarmInventory(farmId);

        //         // // Pass the farm data to the report
        //         // report.RegisterData(farmData, "public_Inventory");

        //         // Set the farmId parameter value
        //         // report.SetParameterValue("farmId", (int)farmId);
        //         report.SetParameterValue("farmId", Convert.ToInt32(farmId));


        //         using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        //         connection.Open();


        //         using (var stream = new MemoryStream())
        //         using (var export = new PDFSimpleExport())
        //         {
        //             // Prepare the report data
        //             report.Prepare();
        //             export.Export(report, stream);
        //             // Set the position of the MemoryStream back to the beginning
        //             stream.Seek(0, SeekOrigin.Begin);
        //             // Create a new MemoryStream and copy the contents of the original stream to it
        //             var outputStream = new MemoryStream(stream.ToArray());

        //             return new FileStreamResult(outputStream, "application/pdf");
        //         }
        //     }
        // }

        public async Task<IActionResult> GenerateReports([FromServices] IWebHostEnvironment webHostEnvironment, int reportTypeId, int farmId)
        {
            // Load the report file
            string templatePath = $"Reports/InventoryItems.frx";
            string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, templatePath);
            using (Report report = new Report())
            {
                var reportName = GetReportFileName(reportTypeId);
                report.Load(Path.Combine(webHostEnvironment.ContentRootPath, "Reports", reportName));

                using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
                connection.Open();
                string sql;
                if (reportName == "InventoryItems.frx")
                {
                    sql = "SELECT * FROM public.\"Inventory\" WHERE public.\"Inventory\".\"FarmId\" = @farmId";
                }
                else if (reportName == "Crops.frx")
                {
                    sql = "SELECT * FROM public.\"Crops\" WHERE public.\"Crops\".\"FarmId\" = @farmId";
                }
                else if (reportName == "Tasks.frx")
                {
                    sql = "SELECT * FROM public.\"Tasks\" WHERE public.\"Tasks\".\"FarmId\" = @farmId";
                }
                else if (reportName == "Revenue.frx")
                {
                    sql = "SELECT * FROM public.\"Inventory\" WHERE public.\"Inventory\".\"FarmId\" = @farmId";

                }
                else if (reportName == "Revenue-Farms-Summary.frx")
                {
                    sql = "SELECT * FROM public.\"Farms\" WHERE public.\"Farms\".\"FarmerId\" = @farmerId";

                }
                else
                {
                    sql = "SELECT * FROM public.\"Expenses\" WHERE public.\"Expenses\".\"FarmId\" = @farmId";
                }

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    var FarmerId = HttpContext.Session.GetInt32("Id");
                    command.Parameters.AddWithValue("@farmerId", FarmerId);
                    command.Parameters.AddWithValue("@farmId", farmId);
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        if (reportName == "InventoryItems.frx")
                        {
                            // Pass the farm data to the report
                            report.RegisterData(dataTable, "public_Inventory");
                        }
                        else if (reportName == "Crops.frx")
                        {
                            report.RegisterData(dataTable, "public_Crops");
                        }
                        else if (reportName == "Tasks.frx")
                        {
                            report.RegisterData(dataTable, "public_Tasks");
                        }
                        else if (reportName == "Revenue.frx")
                        {
                            report.RegisterData(dataTable, "public_Inventory");
                        }
                        else if (reportName == "Revenue-Farms-Summary.frx")
                        {
                            report.RegisterData(dataTable, "public_Farms");
                        }
                        else
                        {
                            report.RegisterData(dataTable, "public_Expenses");
                        }
                    }
                }

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
                { 4, "Expenses.frx"},
                { 5, "Revenue.frx"},
                { 6, "Revenue-Farms-Summary.frx"}
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