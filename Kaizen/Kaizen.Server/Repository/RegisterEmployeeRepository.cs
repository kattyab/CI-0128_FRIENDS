using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;


namespace Kaizen.Server.Repository
{
    public class RegisterEmployeeForm
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
        public required string Province{ get; set; }
        public required string Canton { get; set; }
        public required string Othersigns{ get; set; }
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

    public class RegisterEmployeeRepository
    {
        private readonly string _connectionString;

        public RegisterEmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException(
                    "La cadena de conexion 'KaizenDb' no está definida en appsettings.json");
        }

        public async Task<bool> CreateEmployee(RegisterEmployeeForm employee)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    string getCompanySql = employee.Adminrole == "Administrador" ? $@"
SELECT A.CompanyPK
FROM Admins A
JOIN Users U ON A.AdminPK = U.PersonPK
WHERE U.Email = '{employee.Adminemail}';"
            : employee.Adminrole == "Dueño" ? $@"
SELECT C.CompanyPK
FROM Companies C
JOIN Owners O ON C.OwnerPK = O.OwnerPK
JOIN Users U ON O.OwnerPK = U.PersonPK
WHERE U.Email = '{employee.Adminemail}';"
                    : throw new Exception("Invalid role for retrieving company.");
                    string companyPK;

                    using (SqlCommand companyCmd = new SqlCommand(getCompanySql, conn))
                    {
                        var result = await companyCmd.ExecuteScalarAsync() ?? throw new Exception("Admin email not found or not associated with a company.");
                        companyPK = result.ToString();
                    }


                    var hasher = new PasswordHasher<string>();
                    string hashedPassword = hasher.HashPassword(employee.Email, employee.Password); // Provisional way: boss sets it
                    
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
VALUES ('{personPK}', '{companyPK}', '{employee.Jobposition}', '{employee.Contract}', 0, 0, '{formattedStartdate}', '{employee.Bankaccount}', '{employee.Brutesalary}', '{employee.Paycycle}');
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
