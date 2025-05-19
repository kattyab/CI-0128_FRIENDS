namespace Kaizen.Server.Application.Dtos
{
    public class CompanyDto
    {
        public Guid CompanyPK { get; set; } = default!;
        public string CompanyID { get; set; } = default!;
        public Guid OwnerPK { get; set; } = default!;
        public string CompanyName { get; set; } = default!;
        public string BrandName { get; set; } = default!;
        public string Type { get; set; } = default!;
        public DateTime? FoundationDate { get; set; }
        public int MaxBenefits { get; set; } = default!;
        public string? WebPage { get; set; }
        public string? Logo { get; set; }
        public string? Description { get; set; }
        public string? PO { get; set; }
        public string? Province { get; set; }
        public string? Canton { get; set; }
        public string? OtherSigns { get; set; }

        public string? OwnerName { get; set; } = default!;
    }
}
