using FarmRecordManagementSystem.Models;
using FarmRecordManagementSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics;
using System.Security.Claims;

namespace FarmRecordManagementSystem.Controllers
{
    public class FarmController : Controller
    {
        private IConfiguration _config;

        private readonly IFarmRepository _farmRepository;

        public FarmController(IConfiguration config, IFarmRepository farmRepository)
        {
            _config = config;
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
            var farms = await _farmRepository.GetAllFarms();
            return View(farms);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Land farm)
        {

            await _farmRepository.CreateFarm(farm);
            TempData["success"] = "Farm Created Successfully";
            return RedirectToAction("GetAllFarms");
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public IActionResult AddCrops()
        {
            return View();
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public IActionResult AddExpenses()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddExpenses(Expenses expense, int farmId)
        {
            await _farmRepository.AddExpenses(expense, farmId);
            TempData["success"] = "Expense Added Successfully";
            return RedirectToAction("ViewAllEpenses");
        }

        [HttpPost]
        public async Task<IActionResult> AddCrops(Crops crop, int farmId)
        {
            await _farmRepository.AddCrops(crop, farmId);
            TempData["success"] = "Crops Added Successfully";
            return RedirectToAction("ViewAllCrops");
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
    }
}