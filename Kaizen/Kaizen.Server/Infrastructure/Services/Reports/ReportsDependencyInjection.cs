using Kaizen.Server.Application.Interfaces.Reports;
using Kaizen.Server.Application.Services.Reports;
using Kaizen.Server.Infrastructure.Repositories.Reports;

namespace Kaizen.Server.Infrastructure.Services.Reports
{
    public static class ReportsDependencyInjection
    {
        public static IServiceCollection AddReportsServices(this IServiceCollection services)
        {
            services.AddScoped<IReportsRepository, ReportsRepository>();

            services.AddScoped<IPayrollReportsService, PayrollReportsService>();

            return services;
        }
    }
}
