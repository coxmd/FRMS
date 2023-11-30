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

        public async Task<IActionResult> Index()
        {
            var numberOfFarms = await _adminRepository.GetFarmCount();
            var numberOfUsers = await _adminRepository.GetUserCount();
            var totalRevenue = await _adminRepository.GetTotalRevenue();

            ViewBag.NumberOfFarms = numberOfFarms;
            ViewBag.NumberOfUsers = numberOfUsers;
            ViewBag.TotalRevenue = FormatNumberWithCommas(totalRevenue);
            return View();
        }

        public string FormatNumberWithCommas(decimal number)
        {
            string formattedNumber;

            if (number >= 1000000)
            {
                formattedNumber = number.ToString("#,##0");
            }
            else if (number >= 1000)
            {
                formattedNumber = number.ToString("#,##0");
            }
            else
            {
                formattedNumber = number.ToString("#,##0");
            }

            return formattedNumber;
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

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> ViewExpenseCategory()
        {
            var category = await _adminRepository.GetAllExpenseCategory();
            return View(category);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> ViewCropTypes()
        {
            var cropTypes = await _adminRepository.GetAllCropTypes();
            return View(cropTypes);
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> ViewCropVariety()
        {
            var category = await _adminRepository.GetAllCropVariety();
            return View(category);
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