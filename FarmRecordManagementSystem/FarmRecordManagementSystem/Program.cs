using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

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
