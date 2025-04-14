namespace KaizenProto.Server.Models
{
    public class Empleado
    {
        public string Cedula { get; set; }
        public string TipoContrato { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string CuentaBancaria { get; set; }
        public decimal SalarioBruto { get; set; }
        public string Periodicidad { get; set; }
        public int HorasTrabajadas { get; set; }
        public int HorasExtra { get; set; }
        public string Estado { get; set; }
        public string Puesto { get; set; }
        public string Rol { get; set; }

        public Persona Persona { get; set; }
        public List<Beneficio> Beneficios { get; set; }
    }

}
