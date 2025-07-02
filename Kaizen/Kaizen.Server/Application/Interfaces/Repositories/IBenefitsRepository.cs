using Kaizen.Server.Application.Dtos.Benefits;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Application.Interfaces.Repositories;

public interface IBenefitsRepository
{
    List<BenefitDto> GetBenefits(Guid companyPK);
    BenefitDto? GetBenefit(Guid guid, Guid companyPK);
    void UpdateBenefit(BenefitDto benefit, Guid companyPK);
    void DeleteBenefit(Guid guid, SqlConnection connection, SqlTransaction transaction);
    void DeleteBenefit(int id, Guid companyPK, SqlConnection connection, SqlTransaction transaction);
    void SoftDeleteAndNotifyBenefit(Guid benefitId, SqlConnection connection, SqlTransaction transaction);
    void FullDeleteAndNotifyBenefit(Guid benefitId, SqlConnection connection, SqlTransaction transaction);
    bool GetIfBenefitIsSubscribed(Guid? benefitID, SqlConnection connection, SqlTransaction transaction);
    bool GetIfBenefitIsOnPayroll(Guid? benefitID, SqlConnection connection, SqlTransaction transaction);
}
