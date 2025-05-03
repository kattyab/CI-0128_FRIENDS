using System.Configuration;
using System.Data;
using System.Transactions;
using Microsoft.Data.SqlClient;


namespace Kaizen.Server.Repository
{
    public class RegisterEmployeeForm
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Personid { get; set; }
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
            _connectionString = configuration.GetConnectionString("RegisterEmployee")
                ?? throw new InvalidOperationException(
                    "La cadena de conexion 'EmployeeDetails' no está definida en appsettings.json");
        }

        public async Task<bool> CreateEmployee(RegisterEmployeeForm employee)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Insert into Persons
                    var insertPerson = new SqlCommand(@"
            INSERT INTO Persons (ID, Name, LastName, Sex, BirthDate, Province, Canton, OtherSigns)
            VALUES (@ID, @Name, @LastName, @Sex, @BirthDate, @Province, @Canton, @OtherSigns)", conn, transaction);

                    insertPerson.Parameters.AddWithValue("@ID", employee.Personid);
                    insertPerson.Parameters.AddWithValue("@Name", employee.Name);
                    insertPerson.Parameters.AddWithValue("@LastName", employee.Lastname);
                    insertPerson.Parameters.AddWithValue("@Sex", "Hombre"); // TODO: Replace with actual input
                    insertPerson.Parameters.AddWithValue("@BirthDate", DateTime.Now.AddYears(-25)); // TODO: Replace
                    insertPerson.Parameters.AddWithValue("@Province", "San José"); // TODO
                    insertPerson.Parameters.AddWithValue("@Canton", "Central"); // TODO
                    insertPerson.Parameters.AddWithValue("@OtherSigns", DBNull.Value);

                    await insertPerson.ExecuteNonQueryAsync();

                    // Insert into Users
                    var insertUser = new SqlCommand(@"
            INSERT INTO Users (Email, Password, Active, Role, PersonID)
            VALUES (@Email, @Password, @Active, @Role, @PersonID)", conn, transaction);

                    insertUser.Parameters.AddWithValue("@Email", employee.Email);
                    insertUser.Parameters.AddWithValue("@Password", "hashed-password-placeholder"); // TODO: hash & replace
                    insertUser.Parameters.AddWithValue("@Active", true);
                    insertUser.Parameters.AddWithValue("@Role", /*employee.Role*/ "Empleado");
                    insertUser.Parameters.AddWithValue("@PersonID", employee.Personid);

                    await insertUser.ExecuteNonQueryAsync();

                    // Insert into Employees
                    var insertEmployee = new SqlCommand(@"
            INSERT INTO Employees (EmployeeID, EmployeeNumber, JobPosition, ContractType, WorkedHours, ExtraHours, StartDate, BankAccount, BruteSalary, PayCycle)
            VALUES (@EmployeeID, @EmployeeNumber, @JobPosition, @ContractType, @WorkedHours, @ExtraHours, @StartDate, @BankAccount, @BruteSalary, @PayCycle)", conn, transaction);

                    insertEmployee.Parameters.AddWithValue("@EmployeeID", employee.Personid);
                    insertEmployee.Parameters.AddWithValue("@EmployeeNumber", "12345"); // TODO: auto-generate
                    insertEmployee.Parameters.AddWithValue("@JobPosition", employee.Jobposition);
                    insertEmployee.Parameters.AddWithValue("@ContractType", /*employee.Contract*/ "Tiempo Completo");
                    insertEmployee.Parameters.AddWithValue("@WorkedHours", 0);
                    insertEmployee.Parameters.AddWithValue("@ExtraHours", 0);
                    insertEmployee.Parameters.Add("@StartDate", SqlDbType.Date).Value = DateTime.Parse(/*employee.Startdate*/ "2020-01-01");

                    insertEmployee.Parameters.AddWithValue("@BankAccount", employee.Bankaccount);
                    insertEmployee.Parameters.AddWithValue("@BruteSalary", int.Parse(/*employee.Brutesalary.Replace(",", ""))*/ "1000000"));
                    insertEmployee.Parameters.AddWithValue("@PayCycle", /*employee.Paycicle*/ "Mensual");
                    insertEmployee.Parameters.AddWithValue("@WorksFor", "Empleador");

                    await insertEmployee.ExecuteNonQueryAsync();

                    // Commit transaction
                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log error
                    throw new Exception("Error inserting employee data", ex);
                }
            }
        }
    }
}
