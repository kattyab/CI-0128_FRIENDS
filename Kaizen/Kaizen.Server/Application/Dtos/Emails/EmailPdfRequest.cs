namespace Kaizen.Server.Application.Dtos.Emails
{
    public class EmailPdfRequest
    {
        public List<string> To { get; set; } = new();
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Base64Content { get; set; } = string.Empty;
    }
}
