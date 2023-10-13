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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFarmRepository _farmRepository;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, IFarmRepository farmRepository)
        {
            _logger = logger;
            _config = config;
            _farmRepository = farmRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(AppUsers model)
        {
            if (ModelState.IsValid)
            {
                // Check if the email is already in use (you can add this logic)
                bool isEmailInUse = await CheckIfEmailExists(model.Email);

                if (isEmailInUse)
                {
                    ModelState.AddModelError(string.Empty, "Email is already in use.");
                    return View(model);
                }

                // Create a new user (you can hash and salt the password for security)
                var user = new AppUsers
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    Password = BC.HashPassword(model.Password) // Implement this function
                };

                // Save the user to your database (implement your data access logic)
                bool userCreated = await CreateUser(user.Email, user.UserName, user.Password);

                if (userCreated)
                {
                    // Redirect to the login page after successful signup
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    // Handle errors in user creation
                    ModelState.AddModelError(string.Empty, "Signup failed. Please try again.");
                }
            }

            return View(model);
        }

        // Check if an email already exists in your database
        private async Task<bool> CheckIfEmailExists(string email)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "SELECT COUNT(*) FROM public.\"AppUsers\" WHERE public.\"AppUsers\".\"Email\" = @Email";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                var result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result) > 0;

            }
        }

        // Create a new user and insert them into your database
        private async Task<bool> CreateUser(string email, string UserName, string password)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"AppUsers\" (\"Email\", \"UserName\", \"Password\", \"Role\") VALUES (@Email, @UserName, @Password, @Role)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@UserName", UserName);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Role", "User");

                int rowsAffected = await command.ExecuteNonQueryAsync();

                // Return true if at least one row was affected (user was created), otherwise false
                return rowsAffected > 0;
            }
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string UserName, string Password)
        {
            var query = $"SELECT * FROM public.\"AppUsers\" WHERE public.\"AppUsers\". \"UserName\" = @UserName";

            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@UserName", UserName),
            };

            // if (!string.IsNullOrEmpty(Password))
            // {
            //     parameters.Add(new NpgsqlParameter("@Password", Password));
            // }

            AppUsers appUsers = await AuthenticateUser(query, parameters);

            if (appUsers == null || !BC.Verify(Password, appUsers.Password))
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
                    return RedirectToAction("Index", "Admin");
                }
                else if (appUsers.Role == "User")
                {
                    HttpContext.Session.SetInt32("Id", (int)appUsers.Id);
                    TempData["success"] = "Login Successfully";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        public async Task<AppUsers> AuthenticateUser(string query, List<NpgsqlParameter> parameters)
        {
            AppUsers appUsers = null!;

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
                            Id = (int)dataReader["Id"],
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
            var Id = HttpContext.Session.GetInt32("Id");
            var farms = await _farmRepository.GetAllFarms(Id);
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