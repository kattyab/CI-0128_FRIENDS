public class RegisterCompanyDto
{
    public string CompanyID { get; set; }
    public string CompanyName { get; set; }
    public string BrandName { get; set; }
    public string Type { get; set; }
    public DateTime? FoundationDate { get; set; }
    public int MaxBenefits { get; set; }
    public string WebPage { get; set; }
    public string Logo { get; set; }
    public string Description { get; set; }
    public string PO { get; set; }
    public string Province { get; set; }
    public string Canton { get; set; }
    public string Distrito { get; set; }
    public string OtherSigns { get; set; }

    // Añadir estas propiedades anidadas:
    public OwnerDto owner { get; set; }
    public UserDto user { get; set; }
}

public class OwnerDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Sex { get; set; }
    public DateTime BirthDate { get; set; }
    public string Province { get; set; }
    public string Canton { get; set; }
    public string OtherSigns { get; set; }
}

public class UserDto
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool Active { get; set; }
    public string Role { get; set; }
}
