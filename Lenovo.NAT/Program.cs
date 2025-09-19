using Lenovo.NAT.Infrastructure.AutoMapper;
using Lenovo.NAT.Infrastructure.Context;
using Lenovo.NAT.Infrastructure.Entities;
using Lenovo.NAT.Infrastructure.Repositories.Admin;
using Lenovo.NAT.Infrastructure.Repositories.Logistic;
using Lenovo.NAT.Services;
using Lenovo.NAT.Services.Admin;
using Lenovo.NAT.Services.Logistic;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configure culture for decimal handling
var cultureInfo = new CultureInfo("pt-BR");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
cultureInfo.NumberFormat.NumberGroupSeparator = ",";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
});
builder.Services.AddMvc();

builder.Services.AddDbContext<ThinkToolContext>();

builder.Services.AddDefaultIdentity<User>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ThinkToolContext>()
                .AddDefaultTokenProviders();

builder.Services.AddIdentityCore<User>(opt =>
{
    opt.Password.RequireDigit = false;
});

builder.Services.Configure<PasswordHasherOptions>(options =>
    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3
);

builder.Services.AddSingleton<IUserSessionService, UserSessionService>();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddAutoMapper(typeof(ConfigurationMapping));
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<IPickingService, PickingService>();
builder.Services.AddScoped<IOnlTicketService, OnlTicketService>();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IPickingRepository, PickingRepository>();
builder.Services.AddTransient<IAdminRepository, AdminRepository>();

// Novos repositories para entidades Order
builder.Services.AddTransient<IOrderNotLoadedRepository, OrderNotLoadedRepository>();
builder.Services.AddTransient<IOrderSoldTORepository, OrderSoldTORepository>();
builder.Services.AddTransient<IOrderShipToRepository, OrderShipToRepository>();
builder.Services.AddTransient<IOrderProductRepository, OrderProductRepository>();
builder.Services.AddTransient<IOrderAttachmentRepository, OrderAttachmentRepository>();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();

using (var app = builder.Build())
{
    var scope = app.Services.CreateScope();

    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ThinkToolContext>();

    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseSession();

    // Configure culture middleware
    app.Use(async (context, next) =>
    {
        var cultureInfo = new CultureInfo("pt-BR");
        cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
        cultureInfo.NumberFormat.NumberGroupSeparator = ",";
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
        await next();
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();


    app.MapAreaControllerRoute(
        name: "Logistic",
        areaName: "Logistic",
        pattern: "Logistic/{controller=Picking}/{action=Index}/{id?}"
    );

    app.UseMiddleware<UserTrackingMiddleware>();

    app.MapControllerRoute(
        name: "Login",
        pattern: "{controller=Signin}/{action=Index}/{id?}"
    );

    app.Run();

}