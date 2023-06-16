using FarmRecordManagementSystem.Repositories;
using FarmRecordManagementSystem.Services;
using FarmRecordManagementSystem.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Load the appsettings.json file
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

// Get the plain-text connection string from the configuration
string plainConnectionString = configuration.GetConnectionString("DefaultConnection");

// Encrypt the connection string
string encryptedConnectionString = ConnectionStringEncryptor.EncryptConnectionString(plainConnectionString);


// Update the configuration with the encrypted connection string
configuration.GetSection("ConnectionStrings")["DefaultConnection"] = encryptedConnectionString;

// Save the updated configuration back to the appsettings.json file
var configurationPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
configuration.SaveJsonToFile(configurationPath);
// File.WriteAllText(configurationPath, configuration.GetDebugView());



builder.Services.AddSingleton<IFarmRepository, FarmRepository>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie("UserAuthentication", options =>
{
    options.LoginPath = "/";
    options.LogoutPath = "/Logout";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin", "User");
    });
});

// Add session middleware
builder.Services.AddSession(options =>
{
    // Set session timeout to 20 minutes
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session middleware
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "User",
        pattern: "/Home/Index",
        defaults: new { controller = "Home", action = "Index" })
        .RequireAuthorization("UserPolicy");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
