using FarmRecordManagementSystem.Models;
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

        public FarmController(ILogger<HomeController> logger, IConfiguration config)
        {
            _config = config;
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {

            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"Land\" (\"Name\", \"Size\")" +
                        "VALUES(@Name, @Size)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", Land.Name);
                command.Parameters.AddWithValue("@Description", Land.Description);

                await command.ExecuteNonQueryAsync();
            }

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("UserAuthentication");
            TempData["success"] = "Logout Successfull";
            return RedirectToAction("Login");
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}