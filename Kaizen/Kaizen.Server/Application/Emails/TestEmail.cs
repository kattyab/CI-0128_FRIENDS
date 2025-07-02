using MimeKit;
using System.Text;

namespace Kaizen.Server.Application.Emails;

public class TestEmail : Email
{
    public string TestProperty { get; set; } = "This property gets replaced in the email content";

    public TestEmail()
    {
        this.Subject = "Email Test";
        this.Content =
@"Email test content
{{TestProperty}}";

        this.Attachments.Add(new EmailAttachment
        {
            FileName = "test.txt",
            Content = Encoding.UTF8.GetBytes("This is a test attachment."),
            ContentType = new ContentType("text", "plain"),
        });
    }
}
