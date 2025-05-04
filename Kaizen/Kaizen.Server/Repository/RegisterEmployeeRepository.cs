using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;


namespace Kaizen.Server.Repository
{
    public class RegisterEmployeeForm
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Personid { get; set; }
        public string Sex { get; set; }
        public string Phonenumber { get; set; }
        public string Birthdate { get; set; }
        public string Province{ get; set; }
        public string Canton { get; set; }
        public string Othersigns{ get; set; }
        public string Role { get; set; }
        public string Jobposition { get; set; }
        public string Contract { get; set; }
        public string Paycicle { get; set; }
        public string Brutesalary { get; set; }
        public string Startdate { get; set; }
        public string Bankaccount { get; set; }
        public string Email { get; set; }
    }

    public class RegisterEmployeeRepository
    {
        private readonly string _connectionString;

        public RegisterEmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("EmployeeDetails")
                ?? throw new InvalidOperationException(
                    "La cadena de conexion 'EmployeeDetails' no está definida en appsettings.json");
        }

        public async Task<bool> CreateEmployee(RegisterEmployeeForm employee)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    var hasher = new PasswordHasher<string>();
                    string hashedPassword = hasher.HashPassword(employee.Email, "changeme"); //TODO: Generate random placeholder passwords
                    string companyPK = "AC8250B9-A153-4F49-9691-7F4AD4C3DB69"; //TODO: Extract WorksFor (CompanyPK) from session. Hardcoded for now.
                    DateTime birthdate = DateTime.Parse(employee.Birthdate);
                    DateTime startdate = DateTime.Parse(employee.Startdate);

                    string formattedBirthdate = birthdate.ToString("yyyy-MM-dd");
                    string formattedStartdate = startdate.ToString("yyyy-MM-dd");
                    Guid personPK = Guid.NewGuid();
                    
                    string sql = $@"
INSERT INTO Persons (PersonPK, Id, Name, LastName, Sex, BirthDate, Province, Canton, OtherSigns)
VALUES ('{personPK}', '{employee.Personid}', '{employee.Name}', '{employee.Lastname}', '{employee.Sex}', '{formattedBirthdate}', '{employee.Province}', '{employee.Canton}', '{employee.Othersigns}');

INSERT INTO Users (Email, PasswordHash, Active, Role, PersonPK)
VALUES ('{employee.Email}','{hashedPassword}', 1, '{employee.Role}', '{personPK}');

INSERT INTO PersonPhoneNumbers (PersonPK, Number)
VALUES ('{personPK}', '{employee.Phonenumber}');

INSERT INTO Employees (PersonPK, WorksFor, JobPosition, ContractType, WorkHours, ExtraHours, StartDate, BankAccount, BruteSalary, PayCycleType)
VALUES ('{personPK}', '{companyPK}', '{employee.Jobposition}', '{employee.Contract}', 0, 0, '{formattedStartdate}', '{employee.Bankaccount}', '{employee.Brutesalary}', '{employee.Paycicle}');
";
                    if (employee.Role == "Administrador")
                    {
                        sql += $@"
INSERT INTO Admins (AdminPK, CompanyPK)
VALUES ('{personPK}', '{companyPK}');
";
                    }

                    using SqlCommand cmd = new SqlCommand(sql, conn);
                    await cmd.ExecuteNonQueryAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error inserting employee data", ex);
                }
            }
        }
    }
}
