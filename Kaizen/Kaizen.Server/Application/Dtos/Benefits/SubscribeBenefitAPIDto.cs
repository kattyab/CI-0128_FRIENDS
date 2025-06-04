namespace Kaizen.Server.Application.Dtos.Benefits
{
    public class SubscribeBenefitAPIDto
    {
        public string? AssocName { get; set; }
        public string? Dependents { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}
