namespace Kaizen.Server.Application.Emails;

public class ReportPayrollHistoricRangeEmail : Email
{
    public string EmployeeName { get; set; } = default!;
    public string Start { get; set; } = default!;
    public string End { get; set; } = default!;

    public ReportPayrollHistoricRangeEmail()
    {
        this.Subject = "Reporte historico de planilla para {{EmployeeName}}, Fechas {{Start}} - {{End}}";
        this.Content = $"Los datos del reporte se encuentran adjuntos en este correo.";
    }
}
