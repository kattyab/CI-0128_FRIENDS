using MimeKit;
using System.Reflection;

namespace Kaizen.Server.Application.Emails;

public class Email
{
    public List<MailboxAddress> To { get; set; } = [];
    public string Subject { get; set; } = default!;
    public string Content { get; set; } = "Content";
    public List<EmailAttachment> Attachments { get; set; } = [];

    public Email()
    {
    }

    public string GetSubject()
    {
        return this.ReplacePlaceholders(this.Subject);
    }

    public string GetText()
    {
        return this.ReplacePlaceholders(this.Content);
    }

    private string ReplacePlaceholders(string text)
    {
        PropertyInfo[] properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo property in properties)
        {
            text = text.Replace($"{{{{{property.Name}}}}}", property.GetValue(this)!.ToString());
        }

        return text;
    }
}
