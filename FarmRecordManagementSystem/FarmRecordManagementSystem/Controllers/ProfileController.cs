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
    public class ProfileController : Controller
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IConfiguration _config;

        public ProfileController(IConfiguration config, IProfileRepository profileRepository)
        {
            _config = config;
            _profileRepository = profileRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var Id = HttpContext.Session.GetInt32("Id");
            var user = await _profileRepository.GetUser((int)Id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserProfile(AppUsers user)
        {
            var Id = HttpContext.Session.GetInt32("Id");
            await _profileRepository.UpdateUser(user, (int)Id);
            TempData["success"] = "User Updated Successfully";
            return RedirectToAction("GetUserProfile", new { Id = Id });
        }

    }
}