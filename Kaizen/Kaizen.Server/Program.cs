using System.Reflection;
using Kaizen.Server.Infrastructure.Services.IncomeTax;
using Kaizen.Server.Application.Interfaces.IncomeTax;
using Kaizen.Server.Infrastructure.Repositories;
using Kaizen.Server.Application.Services.IncomeTax;


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<Login>();
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
builder.Services.AddScoped<IIncomeTaxBracketProvider, IncomeTaxBracketFileProvider>();
builder.Services.AddScoped<IIncomeTaxCalculator, IncomeTaxCalculator>();

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
