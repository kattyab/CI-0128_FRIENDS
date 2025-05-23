using Kaizen.Server.Infrastructure.Services.IncomeTax;
using Kaizen.Server.Application.Interfaces.IncomeTax;
using MediatR;
using System.Reflection;
using Kaizen.Server.Infrastructure.Repositories;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Registrando repositorios existentes
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
builder.Services.AddScoped<IIncomeTaxCalculator, IncomeTaxCalculator>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

// Cors
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

// DEMO
using (var scope = app.Services.CreateScope())
{
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    var salarioEjemplo = 5_500_000m;
    var resultado = await mediator.Send(new Kaizen.Server.Application.Queries.IncomeTax.CalculateIncomeTax(salarioEjemplo));

    Console.WriteLine($"Salario bruto: ₡{salarioEjemplo:N0}");
    Console.WriteLine($"Impuesto sobre la renta: ₡{resultado.TaxAmount:N0}");
}

app.Run();
