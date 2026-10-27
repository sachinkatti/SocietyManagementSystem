using Microsoft.AspNetCore.Authentication.Cookies;
using SMS.Application;
using SMS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// MVC
builder.Services.AddControllersWithViews();

// Connection String
var connectionString =
    builder.Configuration.GetConnectionString("SocietyDbConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "SocietyDbConnection is not configured.");
}

// ADO.NET Connection Factory
builder.Services.AddSingleton(
    new DbConnectionFactory(connectionString)
);

// Repository
builder.Services.AddTransient<ILanguageRepository, LanguageRepository>();
builder.Services.AddTransient<IRegionRepository, RegionRepository>();
builder.Services.AddTransient<ICountryRepository, CountryRepository>();
builder.Services.AddTransient<IStateRepository, StateRepository>();
builder.Services.AddTransient<IDistrictRepository, DistrictRepository>();
builder.Services.AddTransient<ITalukRepository, TalukRepository>();
builder.Services.AddTransient<ICityRepository, CityRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IPermissionRepository, PermissionRepository>();
builder.Services.AddTransient<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddTransient<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddTransient<ITenantRepository, TenantRepository>();
builder.Services.AddTransient<IPlanRepository, PlanRepository>();
builder.Services.AddTransient<IFeatureRepository, FeatureRepository>();
builder.Services.AddTransient<IPlanFeatureRepository, PlanFeatureRepository>();
builder.Services.AddTransient<IAdminRepository, AdminRepository>();
builder.Services.AddTransient<IFlatRepository, FlatRepository>();
builder.Services.AddTransient<IWingRepository, WingRepository>();
builder.Services.AddTransient<IResidentRepository, ResidentRepository>();
builder.Services.AddTransient<ISecurityGuardRepository, SecurityGuardRepository>();
builder.Services.AddTransient<IVehicleRegistryRepository, VehicleRegistryRepository>();
builder.Services.AddTransient<IAmenityRepository, AmenityRepository>();
builder.Services.AddTransient<IAmenityBookingRepository, AmenityBookingRepository>();
builder.Services.AddTransient<IVisitorLogRepository, VisitorLogRepository>();
builder.Services.AddTransient<IComplaintRepository, ComplaintRepository>();
builder.Services.AddTransient<INoticeRepository, NoticeRepository>();
builder.Services.AddTransient<IStaffAttendanceRepository, StaffAttendanceRepository>();
builder.Services.AddTransient<IMaintenanceBillRepository, MaintenanceBillRepository>();
builder.Services.AddTransient<IExpenseLedgerRepository, ExpenseLedgerRepository>();
builder.Services.AddTransient<ITenantSubscriptionRepository, TenantSubscriptionRepository>();
builder.Services.AddTransient<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();

// Service
builder.Services.AddTransient<ILanguageService, LanguageService>();
builder.Services.AddTransient<IRegionService, RegionService>();
builder.Services.AddTransient<ICountryService, CountryService>();
builder.Services.AddTransient<IStateService, StateService>();
builder.Services.AddTransient<IDistrictService, DistrictService>();
builder.Services.AddTransient<ITalukService, TalukService>();
builder.Services.AddTransient<ICityService, CityService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IPermissionService, PermissionService>();
builder.Services.AddTransient<IRolePermissionService, RolePermissionService>();
builder.Services.AddTransient<IUserRoleService, UserRoleService>();
builder.Services.AddTransient<ITenantService, TenantService>();
builder.Services.AddTransient<IPlanService, PlanService>();
builder.Services.AddTransient<IFeatureService, FeatureService>();
builder.Services.AddTransient<IPlanFeatureService, PlanFeatureService>();
builder.Services.AddTransient<IAdminService, AdminService>();
builder.Services.AddTransient<IFlatService, FlatService>();
builder.Services.AddTransient<IWingService, WingService>();
builder.Services.AddTransient<IResidentService, ResidentService>();
builder.Services.AddTransient<ISecurityGuardService, SecurityGuardService>();
builder.Services.AddTransient<IVehicleRegistryService, VehicleRegistryService>();
builder.Services.AddTransient<IAmenityService, AmenityService>();
builder.Services.AddTransient<IAmenityBookingService, AmenityBookingService>();
builder.Services.AddTransient<IVisitorLogService, VisitorLogService>();
builder.Services.AddTransient<IComplaintService, ComplaintService>();
builder.Services.AddTransient<INoticeService, NoticeService>();
builder.Services.AddTransient<IStaffAttendanceService, StaffAttendanceService>();
builder.Services.AddTransient<IMaintenanceBillService, MaintenanceBillService>();
builder.Services.AddTransient<IExpenseLedgerService, ExpenseLedgerService>();
builder.Services.AddTransient<ITenantSubscriptionService, TenantSubscriptionService>();
builder.Services.AddTransient<IInvoiceService, InvoiceService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();

builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "SMS.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();



var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();