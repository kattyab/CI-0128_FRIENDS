namespace KaizenProto.Server.Models
{
    public class Persona
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Sexo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string OtrasSenas { get; set; }
    }

}
