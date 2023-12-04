using FarmRecordManagementSystem.Models;
using FarmRecordManagementSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics;
using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
            using (NpgsqlCommand command = new(query, connection))
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

            string query = "INSERT INTO public.\"AppUsers\" (\"Email\", \"UserName\", \"Password\", \"Role\", \"AccountStatus\") VALUES (@Email, @UserName, @Password, @Role, @Status)";

            using (NpgsqlCommand command = new(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@UserName", UserName);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Role", "User");
                command.Parameters.AddWithValue("@Status", "Active");

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

        // private string GenerateResetToken()
        // {
        //     // Generate a unique token for password reset using Guid.NewGuid().ToString()
        //     string resetToken = Guid.NewGuid().ToString();

        //     return resetToken;
        // }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(AppUsers model)
        {
            bool isEmailInUse = await CheckIfEmailExists(model.Email);

            if (!isEmailInUse)
            {
                // If the email doesn't exist in the database, return an error message
                ModelState.AddModelError(string.Empty, "Email does not exist. You can Create a new account");
                return View();
            }
            // string resetToken = GenerateResetToken();
            Configuration.Default.ApiKey.Add("api-key", "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");

            var apiInstance = new TransactionalEmailsApi();
            string SenderName = "Cox Musyoki";
            string SenderEmail = "coxmusyoki@gmail.com";
            SendSmtpEmailSender emailSender = new SendSmtpEmailSender(SenderName, SenderEmail);


            SendSmtpEmailTo emailReciever1 = new SendSmtpEmailTo(model.Email, model.Email);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            To.Add(emailReciever1);



            string HtmlContent = "<html><body><h1>Use the link to reset your password: </h1>" +
            "<a href='https://localhost:7170/Home/ResetPasswordUser'>Click This Link to reset your Password</a></body></html>";

            string TextContent = null;
            string Subject = "Password Reset";
            try
            {
                var sendSmtpEmail = new SendSmtpEmail(emailSender, To, null, null, HtmlContent, TextContent, Subject);
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                Console.WriteLine("Response:\n" + result.ToJson());
                TempData["success"] = "Reset Link Has been sent to your email";
                return RedirectToAction("Login", "Home");
            }
            catch (Exception e)
            {
                TempData["error"] = "An Error Occured try again later";
                Console.WriteLine(e.Message);
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordUser(AppUsers model)
        {
            bool isEmailInUse = await CheckIfEmailExists(model.Email);

            if (!isEmailInUse)
            {
                // If the email doesn't exist in the database, return an error message
                ModelState.AddModelError(string.Empty, "Email does not exist. You can Create a new account");
                return View();
            }

            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            // Start with the basic update query
            string query = "UPDATE public.\"AppUsers\" SET \"Password\" = @NewPassword WHERE public.\"AppUsers\".\"Email\" = @Email";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NewPassword", BC.HashPassword(model.Password));
                command.Parameters.AddWithValue("@Email", model.Email);

                await command.ExecuteNonQueryAsync();
            }
            TempData["success"] = "Password Reset Successful";
            return RedirectToAction("Login", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var query = $"SELECT * FROM public.\"AppUsers\" WHERE public.\"AppUsers\". \"Email\" = @Email";

            var parameters = new List<NpgsqlParameter>
            {
                new("@Email", Email),
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
                    new(ClaimTypes.Surname, appUsers.UserName),
                    new(ClaimTypes.Role, appUsers.Role),
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
                    HttpContext.Session.SetInt32("Id", (int)appUsers.Id);
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

            using (NpgsqlCommand command = new(query, connection))
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

            if (Id != null)
            {
                var farms = await _farmRepository.GetAllFarms(Id);
                return View(farms);
            }
            else
            {
                // Handle the case where the Id is null or missing in the session
                return RedirectToAction("Login"); // Redirect to login page
            }
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