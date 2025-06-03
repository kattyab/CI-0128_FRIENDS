using System.Reflection;
using Microsoft.Data.SqlClient;
using Kaizen.Server.Infrastructure.Services.IncomeTax;
using Kaizen.Server.Application.Interfaces.IncomeTax;
using Kaizen.Server.Infrastructure.Repositories;
using Kaizen.Server.Application.Services.IncomeTax;
using Kaizen.Server.Application.Interfaces.CCSS;
using Kaizen.Server.Application.Services.CCSS;
using Kaizen.Server.Infrastructure.Services.CCSS;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Kaizen.Server.Infrastructure.Services.Auth;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Infrastructure.Services.ApiDeductions;
using Kaizen.Server.Infrastructure.Repositories.ApiDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Kaizen.Server.Application.Services.BenefitDeductions;
using Kaizen.Server.Infrastructure.Repositories.BenefitDeductions;
using Kaizen.Server.Application.Services.ApiDeductions;
using Kaizen.Server.Application.Services.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;
using Kaizen.Server.Infrastructure.Services.Payroll;


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .AddEnvironmentVariables();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SqlConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connStr = config.GetConnectionString("KaizenDb");
    return new SqlConnection(connStr);
});
builder.Services.AddScoped<Kaizen.Server.Infrastructure.Repositories.PayrollRepository>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connStr = config.GetConnectionString("KaizenDb");
    return new Kaizen.Server.Infrastructure.Repositories.PayrollRepository(connStr);
});

builder.Services.AddMemoryCache();

builder.Services.AddScoped<Login>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<RegisterEmployeeRepository>();
builder.Services.AddScoped<CompaniesRepository>();
builder.Services.AddScoped<RegisterCompanyRepository>();
builder.Services.AddScoped<NotificationsRepository>();
builder.Services.AddScoped<EmployeesRepository>();
builder.Services.AddScoped<CommonHomepageRepository>();
builder.Services.AddScoped<CompaniesListRepository>();
builder.Services.AddScoped<EmployeeDetailsRepository>();
builder.Services.AddScoped<BenefitCreationRepository>();
builder.Services.AddScoped<CompanyDetailsRepository>();
builder.Services.AddScoped<CompanyEmployeesRepository>();
builder.Services.AddScoped<UserInfoRepository>();
builder.Services.AddScoped<ApprovedHoursRepository>();

builder.Services.AddScoped<IIncomeTaxBracketProvider, IncomeTaxBracketFileProvider>();
builder.Services.AddScoped<IIncomeTaxCalculator, IncomeTaxCalculator>();

builder.Services.AddScoped<ICCSSRateProvider, CCSSRateFileProvider>();
builder.Services.AddScoped<ICCSSCalculator, CCSSCalculator>();
builder.Services.AddScoped<BenefitsRepository>();

builder.Services.AddScoped<IApiDeductionServiceFactory, ApiDeductionServiceFactory>();
builder.Services.AddScoped<ApiBenefitDeductionRepository>();
builder.Services.AddScoped<IApiBenefitRepository, CachedApiBenefitRepository>();
builder.Services.AddScoped<IExternalApiCaller, ExternalApiCaller>();

builder.Services.AddScoped<IBenefitDeductionServiceFactory, BenefitDeductionServiceFactory>();
builder.Services.AddScoped<IBenefitDeductionRepository, BenefitDeductionRepository>();
builder.Services.AddScoped<IEmployeeDeductionRepository, EmployeeDeductionRepository>();

builder.Services.AddScoped<IPayrollSummaryCalculator, PayrollSummaryCalculator>();
builder.Services.AddScoped<IDaysWorkedCalculator, DaysWorkedCalculator>();
builder.Services.AddScoped<ISalaryCalculator, SalaryCalculator>();
builder.Services.AddScoped<IDeductionAggregator, DeductionAggregator>();

builder.Services.AddPayrollServices();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:55281")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "Kaizen.AuthCookie";
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<GeneralPayrollRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
