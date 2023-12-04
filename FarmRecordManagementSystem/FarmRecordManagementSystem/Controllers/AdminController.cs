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

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> EditExpenseCategory(int Id)
        {
            var expense = await _adminRepository.GetExpenseCategory(Id);
            if (expense == null)
            {
                return NotFound();
            }
            return View(expense);
        }

        [HttpPost]
        public async Task<IActionResult> EditExpenseCategory(ExpenseCategory expense)
        {
            await _adminRepository.UpdateExpenseCategory(expense);
            TempData["success"] = "Expense Category Updated Successfully";
            return RedirectToAction("ViewExpenseCategory");
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> EditCropVariety(int Id)
        {
            var variety = await _adminRepository.GetCropVariety(Id);
            if (variety == null)
            {
                return NotFound();
            }
            return View(variety);
        }

        [HttpPost]
        public async Task<IActionResult> EditCropVariety(CropVariety variety)
        {
            await _adminRepository.UpdateCropVariety(variety);
            TempData["success"] = "Crop Variety Updated Successfully";
            return RedirectToAction("ViewCropVariety");
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> EditCropType(int Id)
        {
            var cropType = await _adminRepository.GetCropType(Id);
            if (cropType == null)
            {
                return NotFound();
            }
            return View(cropType);
        }

        [HttpPost]
        public async Task<IActionResult> EditCropType(CropTypes type)
        {
            await _adminRepository.UpdateCropTypes(type);
            TempData["success"] = "Crop Type Updated Successfully";
            return RedirectToAction("ViewCropTypes");
        }

        [HttpGet]
        public IActionResult AddCropTypes()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCropTypes(CropTypes cropTypes)
        {
            await _adminRepository.AddCropTypes(cropTypes);
            TempData["success"] = "Crop Type Added Successfully";
            return RedirectToAction("ViewCropTypes");
        }

        [HttpGet]
        public IActionResult AddExpenseCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddExpenseCategory(ExpenseCategory category)
        {
            await _adminRepository.AddExpenseCategory(category);
            TempData["success"] = "Expense Category Added Successfully";
            return RedirectToAction("ViewExpenseCategory");
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