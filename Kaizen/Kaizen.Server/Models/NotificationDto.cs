namespace Kaizen.Server.Models;

public class NotificationDto
{
    public Guid Id { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime NotificationDate { get; set; } = default!;
    public Guid UserPK { get; set; } = default!;
}
