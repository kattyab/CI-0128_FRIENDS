using KaizenProto.Server.Handlers;

namespace KaizenProto.Server.Models
{
    public class Beneficio
    {
        public int Id { get; set; }
        public string CedulaEmpleado { get; set; }
        public string BeneficioTipo { get; set; }

        public EmpleadoHandler Empleado { get; set; }
    }

}
