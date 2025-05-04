namespace Kaizen.Server.Models
{
    // Model class representing the data structure for a role change request
    public class RolChangeModel
    {
        // The email of the user whose role will be changed
        public string Email { get; set; }

        // The new role to assign to the user
        public string NuevoRol { get; set; }
    }
}
