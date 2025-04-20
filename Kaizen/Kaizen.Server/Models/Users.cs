namespace Kaizen.Server.Models
{
    public class Users
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public string Role { get; set; }
        public string PersonID { get; set; }
    }

}
