using Kaizen.Server.Application.Dtos;

namespace Kaizen.Server.Application.Interfaces.Repositories
{
    public interface IApprovedHoursRepository
    {
        void InsertApprovedHour(ApprovedHoursDto dto);
        List<ApprovedHoursDto> GetApprovedHoursByEmpId(Guid empId);
        List<ApprovedHoursDto> GetAllApprovedHours();
        Task<bool> UpdateStatusAndSentAsync(Guid approvalID, string status, bool isSent);
        Task<bool> UpdateStatusAsync(Guid approvalID, string status);
    }
}