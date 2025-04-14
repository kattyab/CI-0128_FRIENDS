namespace KaizenProto.Server.Models
{
    public class Telefono
    {
        public int Id { get; set; }
        public string CedulaPersona { get; set; }
        public string TelefonoNumero { get; set; }

        public Persona Persona { get; set; }
    }

}
