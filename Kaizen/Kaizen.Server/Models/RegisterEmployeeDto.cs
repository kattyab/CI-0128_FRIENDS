namespace Kaizen.Server.Models
{
    public class RegisterEmployeeDto
    {
        public int Id { get; set; }
        public required string Adminrole { get; set; }
        public required string Adminemail { get; set; }
        public required string Name { get; set; }
        public required string Lastname { get; set; }
        public required string Personid { get; set; }
        public required string Sex { get; set; }
        public required string Phonenumber { get; set; }
        public required string Birthdate { get; set; }
        public required string Province { get; set; }
        public required string Canton { get; set; }
        public required string Othersigns { get; set; }
        public required string Role { get; set; }
        public required string Jobposition { get; set; }
        public required string Contract { get; set; }
        public required string Paycycle { get; set; }
        public required string Brutesalary { get; set; }
        public required string Startdate { get; set; }
        public required string Bankaccount { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
