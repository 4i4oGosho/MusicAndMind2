using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MusicAndMind2.Data;
using MusicAndMind2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<SmsSettings>(
    builder.Configuration.GetSection("SMS"));

builder.Services.AddScoped<ISmsSender, TwilioSmsSender>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MusicAndMind2.Data.ApplicationDbContext>();

    // ако базата не е мигрирана/създадена, това помага
    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new MusicAndMind2.Models.Product
            {
                Name = "Тибетска купа (432 Hz)",
                Description = "Кристална купа, чийто звук насърчава дълбока релаксация и вътрешен баланс.",
                Price = 49.99m,
                ImageUrl = "/shop/bowl1.png",
                SoundUrl = "/shop/audio/bowl1.mp3",
                FrequencyInfo = "Подпомага алфа мозъчни вълни (8–12 Hz).",
                Category = "Купи",
                LongDescription = "Тази тибетска купа е изработена ръчно в подножието на Хималаите и създава меки, дълбоки вибрации, които хармонизират тялото и ума.",
                IsAvailable = true,
                CreatedAt = DateTime.Now
            },
            new MusicAndMind2.Models.Product
            {
                Name = "Камертон (528 Hz)",
                Description = "Камертон за енергийно пречистване и „честотата на любовта“.",
                Price = 29.99m,
                ImageUrl = "/shop/fork1.png",
                SoundUrl = "/shop/audio/fork1.mp3",
                FrequencyInfo = "528 Hz се асоциира с усещане за мир и вътрешна хармония.",
                Category = "Камертон",
                LongDescription = "Този камертон е настроен на 528 Hz — честота, често наричана „честотата на любовта“.",
                IsAvailable = true,
                CreatedAt = DateTime.Now
            },
            new MusicAndMind2.Models.Product
            {
                Name = "Гонг – Златен резонанс",
                Description = "Малък гонг с богат, вибрационен тон, подходящ за дълбоки медитации.",
                Price = 79.99m,
                ImageUrl = "/shop/gong1.png",
                SoundUrl = "/shop/audio/gong1.mp3",
                FrequencyInfo = "Тета честоти (4–7 Hz), подходящи за сън и релакс.",
                Category = "Гонгове",
                LongDescription = "Златният гонг е изработен от висококачествен бронз и произвежда дълбок, хармоничен тон.",
                IsAvailable = true,
                CreatedAt = DateTime.Now
            },
            new MusicAndMind2.Models.Product
            {
                Name = "Калимба – Звуков поток",
                Description = "Инструмент с топъл, дървен тон, който стимулира креативността.",
                Price = 39.99m,
                ImageUrl = "/shop/kalimba1.png",
                SoundUrl = "/shop/audio/kalimba1.mp3",
                FrequencyInfo = "Стимулира бета вълни (13–30 Hz) – фокус и вдъхновение.",
                Category = "Калимби",
                LongDescription = "Калимбата е африкански музикален инструмент с дървен резонатор и метални езичета.",
                IsAvailable = true,
                CreatedAt = DateTime.Now
            },
            new MusicAndMind2.Models.Product
            {
                Name = "Кристален камертон (639 Hz)",
                Description = "Камертон, свързан със състрадание и хармония в отношенията.",
                Price = 34.99m,
                ImageUrl = "/shop/fork2.png",
                SoundUrl = "/shop/audio/fork2.mp3",
                FrequencyInfo = "639 Hz – честота на връзката и сърдечната чакра.",
                Category = "Камертони",
                LongDescription = "639 Hz се използва за подобряване на комуникацията и доверие в отношенията.",
                IsAvailable = true,
                CreatedAt = DateTime.Now
            },
            new MusicAndMind2.Models.Product
            {
                Name = "Шамански барабан (7 Hz)",
                Description = "Ръчно изработен барабан, който резонира с пулса на земята.",
                Price = 89.99m,
                ImageUrl = "/shop/drum1.png",
                SoundUrl = "/shop/audio/drum1.mp3",
                FrequencyInfo = "7 Hz – тета резонанс за дълбока медитация.",
                Category = "Барабани",
                LongDescription = "Инструмент за дълбоки практики, който успокоява ума и подпомага навлизане в транс.",
                IsAvailable = true,
                CreatedAt = DateTime.Now
            },
            new MusicAndMind2.Models.Product
            {
                Name = "Пеещ кристал (963 Hz)",
                Description = "Фин инструмент от кварцов кристал за духовна връзка и яснота.",
                Price = 69.99m,
                ImageUrl = "/shop/crystal1.png",
                SoundUrl = "/shop/audio/crystal1.mp3",
                FrequencyInfo = "963 Hz – активира връзката с висшето съзнание.",
                Category = "Кристали",
                LongDescription = "Чист звук за баланс на енергията и коронната чакра.",
                IsAvailable = true,
                CreatedAt = DateTime.Now
            },
            new MusicAndMind2.Models.Product
            {
                Name = "Звукова пирамида (888 Hz)",
                Description = "Медна пирамида, която излъчва стабилизираща вибрация.",
                Price = 59.99m,
                ImageUrl = "/shop/pyramid1.png",
                SoundUrl = "/shop/audio/pyramid1.mp3",
                FrequencyInfo = "888 Hz – баланс между материя и дух.",
                Category = "Пирамиди",
                LongDescription = "Стабилизира енергийните центрове и хармонизира околната среда.",
                IsAvailable = true,
                CreatedAt = DateTime.Now
            },
            new MusicAndMind2.Models.Product
            {
                Name = "Дигитална арфа „Аура“ (528 Hz)",
                Description = "Модерна електронна арфа с честоти за фокус и релаксация.",
                Price = 119.99m,
                ImageUrl = "/shop/harp1.png",
                SoundUrl = "/shop/audio/harp1.mp3",
                FrequencyInfo = "528 Hz – честота на изцеление и любов.",
                Category = "Електронни инструменти",
                LongDescription = "Комбинира електронна музика и честотна терапия – за фокус, медитация и креативност.",
                IsAvailable = true,
                CreatedAt = DateTime.Now
            }
        );

        db.SaveChanges();
    }
}


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
app.UseSession();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Identity/Account/Login"))
    {
        context.Response.Redirect("/Auth/Login");
        return;
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    var adminEmail = app.Configuration["AdminSeed:Email"];
    var adminPass = app.Configuration["AdminSeed:Password"];

    // Avoid hardcoded credentials in source control.
    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPass))
    {
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
}

app.Run();
