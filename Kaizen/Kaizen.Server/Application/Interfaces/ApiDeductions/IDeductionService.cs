namespace Kaizen.Server.Application.Interfaces.ApiDeductions
{
    public interface IDeductionService
    {
        Task<Dictionary<string, decimal>> GetDeductionsForEmployeeAsync(Guid employeeId);
    }
}
