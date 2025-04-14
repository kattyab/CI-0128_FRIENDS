namespace KaizenProto.Server.Models
{
    public class Usuario
    {
        public int UserId { get; set; }
        public string Cedula { get; set; }
        public string Contrasena { get; set; }
        public string Correo { get; set; }

        public Persona Persona { get; set; }
    }

}
