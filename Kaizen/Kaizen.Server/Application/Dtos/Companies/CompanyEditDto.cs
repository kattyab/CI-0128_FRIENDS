namespace Kaizen.Server.Application.Dtos.Companies;

public class CompanyEditDto
{
    public string CompanyName { get; set; } = default!;
    public string BrandName { get; set; } = default!;
    public int MaxBenefits { get; set; } = default!;
    public string? WebPage { get; set; }
    public string? Logo { get; set; }
    public string? Description { get; set; }
    public string? PO { get; set; }
    public string? Province { get; set; }
    public string? Canton { get; set; }
    public string? Distrito { get; set; }
    public string? OtherSigns { get; set; }
}