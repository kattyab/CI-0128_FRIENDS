namespace Kaizen.Server.Application.Interfaces.Services
{
    public interface IBenefitsService
    {
        public void DeleteBenefit(Guid benefitId);
        public void DeleteBenefit(int id, Guid CompanyPK);
    }
}
