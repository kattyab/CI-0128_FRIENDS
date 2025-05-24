using Kaizen.Server.Infrastructure.Repositories;
using System.Reflection;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .AddEnvironmentVariables();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            // Due to authentication, as far as I know, we need to specify the origin
            // instead of using AllowAnyOrigin(), so you might need to change port
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

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
