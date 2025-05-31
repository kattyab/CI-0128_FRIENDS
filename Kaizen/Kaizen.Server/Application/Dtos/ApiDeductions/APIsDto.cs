namespace Kaizen.Server.Application.Dtos.ApiDeductions;

public class APIsDto
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string HttpMethod { get; set; }
    public string AuthorizationHeader { get; set; }
    public string AuthorizationToken { get; set; }
    public string ParametersJson { get; set; }
    public string ExpectedDataType { get; set; }
}
