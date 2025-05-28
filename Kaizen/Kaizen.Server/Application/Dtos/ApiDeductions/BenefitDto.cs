namespace Kaizen.Server.Application.Dtos.ApiDeductions;

public class BenefitDto
{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string HttpMethod { get; set; }
    public string AuthHeaderName { get; set; }
    public string AuthToken { get; set; }
    public string ParametersJson { get; set; }
    public string ExpectedDataType { get; set; }
}
