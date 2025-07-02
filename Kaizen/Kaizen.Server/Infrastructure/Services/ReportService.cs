using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.Auth;
using Kaizen.Server.Application.Dtos.Employees;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Emails;
using Kaizen.Server.Application.Interfaces.Employees;
using Kaizen.Server.Application.Interfaces.Payroll;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Application.Interfaces.Services;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Kaizen.Server.Infrastructure.Repositories;
using Kaizen.Server.Infrastructure.Repositories.Payroll;
using System.Text;

namespace Kaizen.Server.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly IAuthService _authService;
    private readonly ICompaniesRepository _companiesRepository;
    private readonly IEmployeesRepository _employeesRepository;
    private readonly IEmployeeRepository _employeeDetailsRepository;
    private readonly IPayrollRepository _payrollRepository;
    private readonly IEmailService _emailService;

    public ReportService(IAuthService authService,
        ICompaniesRepository companiesRepository,
        IEmployeesRepository employeesRepository,
        IEmployeeRepository employeeDetailsRepository,
        IPayrollRepository payrollRepository,
        IEmailService emailService)
    {
        this._authService = authService;
        this._companiesRepository = companiesRepository;
        this._employeesRepository = employeesRepository;
        this._employeeDetailsRepository = employeeDetailsRepository;
        this._payrollRepository = payrollRepository;
        this._emailService = emailService;
    }

    public HistoricRangeInitialDto GetHistoricRangeInitialDataAsync(Guid companyId)
    {
        HistoricRangeInitialDto historicRangeInitialData = new();

        CompanyDto? company = this._companiesRepository.GetCompany(companyId);

        if (company == null)
        {
            throw new ArgumentException($"Company with ID {companyId} not found.");
        }

        List<EmployeeDto> employees = this._employeesRepository.GetEmployees(companyId, true);

        historicRangeInitialData.CompanyName = company.CompanyName;
        historicRangeInitialData.Employees = [.. employees
            .Select(e => new ReportEmployeeDto()
            {
                Id = e.EmpID,
                Name = e.Name,
                LastName = e.LastName,
            })
        ];

        return historicRangeInitialData;
    }

    public async Task<List<HistoricRangePayroll>> GetHistoricRangePayroll(Guid companyPK, HistoricRangeSearch historicRangeSearch)
    {
        if (historicRangeSearch.EmployeeId == Guid.Empty)
        {
            throw new ArgumentException("Employee ID cannot be empty.");
        }

        if (historicRangeSearch.End < historicRangeSearch.Start)
        {
            throw new ArgumentException("End date cannot be earlier than start date.");
        }

        List<HistoricRangePayroll> payrolls = this._payrollRepository.GetEmployeeHistoricRangePayrolls(historicRangeSearch.EmployeeId, historicRangeSearch.Start, historicRangeSearch.End);

        return payrolls;
    }
    public async Task SendCompaniesHistoricEmail(string csvContent)
    {
        if (csvContent == null)
            throw new ArgumentException("CSV content cannot be null", nameof(csvContent));

        AuthUserDto user = this._authService.GetAuthUser();

        if (user == null)
            throw new InvalidOperationException("Authenticated user cannot be null");

        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.LastName))
            throw new ArgumentException("User name and last name must not be empty");

        if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
            throw new ArgumentException("Invalid email", nameof(user.Email));

        string csvFileName = "reporte_companias_historico.csv";

        ReportPayrollHistoricRangeEmail email = new()
        {
            Start = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"),
            End = DateTime.Now.ToString("yyyy-MM-dd"),
            EmployeeName = $"{user.Name} {user.LastName}",
        };

        email.To.Add(new($"{user.Name} {user.LastName}", user.Email));

        email.Attachments.Add(new EmailAttachment
        {
            FileName = csvFileName,
            ContentType = new("text", "csv"),
            Content = Encoding.UTF8.GetBytes(csvContent)
        });

        await this._emailService.SendEmail(email);
    }



    public async Task SendHistoricRangeEmail(Guid companyPK, HistoricRangeSearch historicRangeSearch)
    {
        if (historicRangeSearch.EmployeeId == Guid.Empty)
        {
            throw new ArgumentException("Employee ID cannot be empty.");
        }

        if (historicRangeSearch.End < historicRangeSearch.Start)
        {
            throw new ArgumentException("End date cannot be earlier than start date.");
        }

        EmployeeDetailsDto? employee = await this._employeeDetailsRepository.GetByIdAsync(historicRangeSearch.EmployeeId);
        if (employee == null)
        {
            throw new ArgumentException($"Employee with ID {historicRangeSearch.EmployeeId} not found.");
        }

        List<HistoricRangePayroll> payrolls = this._payrollRepository.GetEmployeeHistoricRangePayrolls(historicRangeSearch.EmployeeId, historicRangeSearch.Start, historicRangeSearch.End);

        string csvFileName = $"historic_range_payroll.csv";
        string csvHeaders = "Tipo de contrato,Posición,Fecha de pago,Salario Bruto,Deducciones obligatorias,Deducciones voluntarias,Salario neto";
        StringBuilder csvContent = new();
        csvContent.AppendLine(csvHeaders);
        foreach (HistoricRangePayroll payroll in payrolls)
        {
            string line = $"{payroll.ContractType},{payroll.JobPosition},{payroll.PayrollDate:yyyy-MM-dd},{payroll.BruteSalary},{payroll.ObligatoryDeductions},{payroll.OptionalDeductions},{payroll.NetSalary}";
            csvContent.AppendLine(line);
        }
        string totals = $",,,{payrolls.Sum(p => p.BruteSalary)},{payrolls.Sum(p => p.ObligatoryDeductions)},{payrolls.Sum(p => p.OptionalDeductions)},{payrolls.Sum(p => p.NetSalary)}";
        csvContent.AppendLine(totals);

        AuthUserDto user = this._authService.GetAuthUser();

        ReportPayrollHistoricRangeEmail email = new()
        {
            EmployeeName = employee.FirstName + " " + employee.LastName,
            Start = historicRangeSearch.Start.ToString("yyyy-MM-dd"),
            End = historicRangeSearch.End.ToString("yyyy-MM-dd"),
        };
        email.To.Add(new($"{user.Name} {user.LastName}", user.Email));

        email.Attachments.Add(new EmailAttachment()
        {
            FileName = csvFileName,
            ContentType = new("text", "csv"),
            Content = Encoding.UTF8.GetBytes(csvContent.ToString())
        });

        await this._emailService.SendEmail(email);
    }
}
