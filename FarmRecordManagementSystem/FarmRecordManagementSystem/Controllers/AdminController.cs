using FarmRecordManagementSystem.Models;
using FarmRecordManagementSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics;
using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;

namespace FarmRecordManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IConfiguration _config;

        public AdminController(IConfiguration config, IAdminRepository adminRepository)
        {
            _config = config;
            _adminRepository = adminRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFarms()
        {
            var farms = await _adminRepository.GetAllFarms();
            return View(farms);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminRepository.GetAllUsers();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddUsers(AppUsers user)
        {
            await _adminRepository.AddUser(user);
            TempData["success"] = "User Created Successfully";
            return RedirectToAction("GetAllUsers");
        }

    }
}