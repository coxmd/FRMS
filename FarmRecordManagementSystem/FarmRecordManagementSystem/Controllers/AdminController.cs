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
        private readonly IFarmRepository _farmRepository;
        private readonly IConfiguration _config;

        public AdminController(IConfiguration config, IFarmRepository farmRepository)
        {
            _config = config;
            _farmRepository = farmRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}