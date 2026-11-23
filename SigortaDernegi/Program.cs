using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using SigortaDernegi.Areas.Admin.Services;
using SigortaDernegi.Data;
using SigortaDernegi.Areas.Admin.Authorization;
using SigortaDernegi.Areas.Member.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
	.AddDefaultIdentity<IdentityUser>(options => //Kullanıcıların kayıt olmasını, giriş çıkış yapmasını ve şifrelerinin güvenli şekilde saklanmasını sağlar.
	{
		options.SignIn.RequireConfirmedAccount = true; //Kullanıcı hesabı doğrulanmadan giriş yapılmasını engeller. İleride e-posta doğrulaması ekleyecez.
	})
	.AddRoles<IdentityRole>()   //Kullanıcılara rol vermemizi sağlar.
	.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        MemberPortalPolicies.ActiveMember,
        policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(IdentitySeedData.MemberRole);
            policy.AddRequirements(new ActiveMemberRequirement());
        });
});
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("member-contact", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    options.AddPolicy("public-forms", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
builder.Services.AddScoped<AdminImageStorage>();

builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IAuthorizationHandler, ActiveMemberHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    await dbContext.Database.MigrateAsync();
	await IdentitySeedData.SeedAsync(
	scope.ServiceProvider,
	builder.Configuration);
    await RolePermissionSeedData.SeedAsync(dbContext);

    await EventSeedData.SeedAsync(dbContext);

    await NewsSeedData.SeedAsync(dbContext);

    await CorporateContentSeedData.SeedAsync(dbContext);

    await MultimediaSeedData.SeedAsync(dbContext);

    await ContactAndFaqSeedData.SeedAsync(dbContext);

    await VolunteerSeedData.SeedAsync(dbContext);

    await HomeMissionSeedData.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Yerel ağdan (Wi-Fi) bağlanan cihazların SSL sertifika hatası almaması için HTTPS yönlendirmesi Development ortamında kapalıdır.
if (!app.Environment.IsDevelopment() && !builder.Configuration.GetValue<bool>("DisableHttpsRedirection"))
{
    // app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();         //Açılmak istenen sayfayı belirler.
app.UseRateLimiter();
app.UseAuthentication(); //“Bu kullanıcı giriş yapmış mı ve kim? (kullanıcıyı tanır).
app.UseAuthorization(); //Kullanıcının yetkisini kontrol eder.

// Uygulamanın kök adresi her zaman ziyaretçi ana sayfasını açar.
app.MapControllerRoute(
    name: "visitor-home",
    pattern: "",
    defaults: new
    {
        area = "",
        controller = "Home",
        action = "Index"
    });

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
