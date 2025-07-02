using Kaizen.Server.Application.Dtos.Emails;
using Kaizen.Server.Application.Emails;
using Kaizen.Server.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace Kaizen.Server.API.Controllers.Emails
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailService emailService;

        public EmailsController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        [HttpPost("send-pdf")]
        public async Task<IActionResult> SendPayrollPdfEmail([FromBody] EmailPdfRequest request)
        {
            try
            {
                var attachment = new EmailAttachment
                {
                    FileName = request.FileName,
                    Content = Convert.FromBase64String(request.Base64Content),
                    ContentType = new ContentType("application", "pdf")
                };

                var email = new Email
                {
                    To = request.To.Select(addr => MailboxAddress.Parse(addr)).ToList(),
                    Subject = request.Subject,
                    Content = request.Body,
                    Attachments = new List<EmailAttachment> { attachment }
                };

                await emailService.SendEmail(email);
                return Ok(new { message = "Email sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
