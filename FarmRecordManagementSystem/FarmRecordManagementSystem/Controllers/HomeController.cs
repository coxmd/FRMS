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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFarmRepository _farmRepository;
        private IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, IFarmRepository farmRepository)
        {
            _logger = logger;
            _config = config;
            _farmRepository = farmRepository;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string UserName, string Password)
        {
            var query = $"SELECT * FROM public.\"AppUsers\" WHERE public.\"AppUsers\". \"UserName\" = @UserName AND public.\"AppUsers\".\"Password\" = @Password";

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@UserName", UserName),
                new NpgsqlParameter("@Password", Password),
            };

            // if (!string.IsNullOrEmpty(Password))
            // {
            //     parameters.Add(new NpgsqlParameter("@Password", Password));
            // }

            AppUsers appUsers = await AuthenticateUser(query, parameters);

            if (appUsers == null)
            {
                TempData["error"] = "Invalid Login Credentials";
                return View();
            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Surname, appUsers.UserName),
                    new Claim(ClaimTypes.Role, appUsers.Role),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, "UserAuthentication"
                );

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(
                    "UserAuthentication",
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                if (appUsers.Role == "Admin")
                {
                    TempData["success"] = "Login Successfully";
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (appUsers.Role == "User")
                {
                    TempData["success"] = "Login Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        public async Task<AppUsers> AuthenticateUser(string query, List<NpgsqlParameter> parameters)
        {
            AppUsers appUsers = null;

            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                using (NpgsqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    if (await dataReader.ReadAsync())
                    {
                        appUsers = new AppUsers
                        {
                            Role = (string)dataReader["Role"],
                            UserName = (string)dataReader["UserName"],
                            Password = (string)dataReader["Password"],
                        };
                    }
                }
            }
            return appUsers;
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("UserAuthentication");
            TempData["success"] = "Logout Successfull";
            return RedirectToAction("Login");
        }

        [HttpGet, Authorize(AuthenticationSchemes = "UserAuthentication")]
        public async Task<IActionResult> Index()
        {
            var farms = await _farmRepository.GetAllFarms();
            return View(farms);
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