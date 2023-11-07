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

        [HttpGet]
        public IActionResult AddUsers()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUsers(AppUsers user)
        {
            await _adminRepository.AddUser(user);
            TempData["success"] = "User Created Successfully";
            return RedirectToAction("GetAllUsers");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int Id)
        {
            var user = await _adminRepository.GetUser(Id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(AppUsers user)
        {
            await _adminRepository.UpdateUser(user);
            TempData["success"] = "User Updated Successfully";
            return RedirectToAction("GetAllUsers");
        }

        public async Task<IActionResult> DeactivateUser(int Id)
        {
            await _adminRepository.DeactivateUser(Id);
            TempData["success"] = "User Deactivated Successfully";
            return RedirectToAction("GetAllUsers");
        }


    }
}