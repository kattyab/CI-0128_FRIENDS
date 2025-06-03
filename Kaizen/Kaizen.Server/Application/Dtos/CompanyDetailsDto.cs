namespace Kaizen.Server.Application.Dtos;
public class CompanyDetailsDto
{
    public string CompanyPK { get; set; }
    public string CompanyID { get; set; }
    public string OwnerName { get; set; }
    public string CompanyName { get; set; }
    public string BrandName { get; set; }
    public string Type { get; set; }
    public DateTime? FoundationDate { get; set; }
    public int MaxBenefits { get; set; }
    public string WebPage { get; set; }
    public string Description { get; set; }
    public string PO { get; set; }
    public string Province { get; set; }
    public string Canton { get; set; }
    public string Distrito { get; set; }
    public string OtherSigns { get; set; }
    public string Logo { get; set; }
}