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

        [HttpPost]
        public async Task<IActionResult> Create(Land farm)
        {

            await _farmRepository.CreateFarm(farm);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> AddCrops(int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "SELECT * FROM public.\"Land\" WHERE public.\"Land\".\"Id\" =@FarmId";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", farmId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        ViewBag.FarmId = farmId;
                        return View();
                    }
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddCrops(Crops crop, int farmId)
        {
            await _farmRepository.AddCrops(crop, farmId);
            return View(crop);
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