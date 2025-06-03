using Kaizen.Server.Application.Interfaces.Payroll;
using Kaizen.Server.Application.Services.Payroll;
using Kaizen.Server.Infrastructure.Helpers.Payroll;
using Kaizen.Server.Infrastructure.Repositories.Payroll;

namespace Kaizen.Server.Infrastructure.Services.Payroll
{
    public static class PayrollServiceCollection
    {
        public static IServiceCollection AddPayrollServices(this IServiceCollection services)
        {
            services.AddScoped<IPayrollProcessingService, PayrollProcessingService>();

            services.AddScoped<IEmployeePayrollRepository, EmployeePayrollRepository>();
            services.AddScoped<IPayrollRepository, PayrollRepository>();

            services.AddScoped<IPayrollCore, PayrollCore>();

            services.AddScoped<IPayrollDataTransformer, PayrollDataTransformer>();
            services.AddScoped<IPayrollOutputService, PayrollOutputService>();

            return services;
        }
    }
}
