using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MusicAndMind2.Data; // това е за ApplicationDbContext.cs

var builder = WebApplication.CreateBuilder(args);

// 🧩 1. Добавяме локализацията (твоят съществуващ код)
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// 🧩 2. Добавяме Entity Framework Core и Identity + роли
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // ✅ Добавяме поддръжка на роли
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 🧩 3. Добавяме MVC с локализация
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// 🧺 3.1 Добавяме поддръжка за сесии (за кошницата)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // колко време да пази сесията
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // нужна за GDPR съвместимост
});

var app = builder.Build();

// 🧩 4. Култури (твоят код, запазен)
var supportedCultures = new[] { new CultureInfo("bg"), new CultureInfo("en") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("bg"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🧺 5. Активираме сесиите (трябва да е преди Authentication)
app.UseSession();

// 🧩 6. Добавяме Authentication и Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// 🧩 7. Запазваме оригиналната маршрутна логика
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 🧩 8. Създаваме админ роля и потребител при първо стартиране
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Създаваме роля Admin, ако не съществува
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Проверяваме дали има админ акаунт
    string adminEmail = "admin@musicmind.com";
    string adminPass = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, adminPass);
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.Run();
