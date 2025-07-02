
using MimeKit;

namespace Kaizen.Server.Application.Emails;

public class EmailAttachment
{
    public string FileName { get; set; } = default!;
    public byte[] Content { get; set; } = default!;
    public ContentType ContentType { get; set; } = default!;
}
