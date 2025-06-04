namespace Kaizen.Server.Application.Dtos.Payroll;

public sealed class PayrollHistoryRowDto
{
    public Guid Id { get; set; }
    public string Manager { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Period { get; set; } = default!;
    public decimal Deductions { get; set; }
    public decimal SocialCharges { get; set; }
    public decimal Total { get; set; }
    public decimal Gross { get; set; }
    public decimal Net { get; set; }
}
