using System.Data;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Repository
{
  public class EmployeeDetails
  {
    private readonly SqlConnection _connection;
    private readonly string _connectionPath;

    public EmployeeDetails(IConfiguration configuration)
    {
      _connectionPath = configuration.GetConnectionString("KaizenDb") ??
        throw new ArgumentNullException(nameof(configuration), "Connection string 'KaizenDb' cannot be null.");
      _connection = new SqlConnection(_connectionPath);
    }

    public DataTable CreateConsultTable(string query, SqlParameter[]? parameters = null)
    {
      using var connection = new SqlConnection(_connection.ConnectionString);
      using var command = new SqlCommand(query, connection);

      if (parameters != null)
      {
        command.Parameters.AddRange(parameters);
      }

      var dataAdapter = new SqlDataAdapter(command);
      var resultTable = new DataTable();
      dataAdapter.Fill(resultTable);
      return resultTable;
    }

    public object? ObtainEmployeeData(string email)
    {
      var employeeDataTable = FetchEmployeeDataTable(email);

      if (employeeDataTable.Rows.Count == 0)
        return null;

      var row = employeeDataTable.Rows[0];

      var employeeData = new
      {
        Id = GetString(row, "id"),
        FirstName = GetString(row, "first_name"),
        LastName = GetString(row, "last_name"),
        Gender = GetString(row, "gender"),
        BirthDate = Convert.ToDateTime(row["birth_date"]),
        Province = GetString(row, "province"),
        Canton = GetString(row, "canton"),
        OtherSigns = GetString(row, "other_signs"),
        PhoneNumbers = ExtractPhoneNumbers(employeeDataTable),
        GrossSalary = GetNullableDecimal(row, "gross_salary"),
        ContractType = GetString(row, "contract_type"),
        StartDate = GetNullableDateTime(row, "start_date"),
        PayCycle = GetString(row, "pay_cycle"),
        JobPosition = GetString(row, "job_position"),
        Role = GetString(row, "role"),
        Status = Convert.ToBoolean(row["status"]) ? "Activo" : "Inactivo",
        Benefits = ExtractBenefits(employeeDataTable),
        Email = GetString(row, "email")
      };

      return employeeData;
    }

    private DataTable FetchEmployeeDataTable(string email)
    {
      string query = @"SELECT 
          p.Id AS id,
          p.Name AS first_name,
          p.LastName AS last_name,
          p.Sex AS gender,
          p.BirthDate AS birth_date,
          p.Province AS province,
          p.Canton AS canton,
          p.OtherSigns AS other_signs,
          ph.Number AS phone_number,
          e.BruteSalary AS gross_salary,
          e.ContractType AS contract_type,
          e.StartDate AS start_date,
          e.PayCycleType AS pay_cycle,
          e.JobPosition AS job_position,
          u.Role AS role,
          u.Active AS status,
          b.Name AS benefit,
          u.Email AS email
        FROM Persons p
        INNER JOIN Users u ON u.PersonPK = p.PersonPK
        LEFT JOIN Employees e ON e.PersonPK = p.PersonPK
        LEFT JOIN PersonPhoneNumbers ph ON ph.PersonPK = p.PersonPK
        LEFT JOIN ChosenBenefits cb ON cb.EmployeeID = e.EmpID
        LEFT JOIN Benefits b ON b.ID = cb.BenefitID
        WHERE u.Email = @Email;";

      SqlParameter[] parameters = { new("@Email", email) };
      return CreateConsultTable(query, parameters);
    }

    private static string? GetString(DataRow row, string column)
    {
      return row.IsNull(column) ? null : Convert.ToString(row[column]);
    }

    private static decimal? GetNullableDecimal(DataRow row, string column)
    {
      return row.IsNull(column) ? (decimal?)null : Convert.ToDecimal(row[column]);
    }

    private static DateTime? GetNullableDateTime(DataRow row, string column)
    {
      return row.IsNull(column) ? (DateTime?)null : Convert.ToDateTime(row[column]);
    }

    private static List<string> ExtractPhoneNumbers(DataTable table)
    {
      return [.. table.AsEnumerable()
                  .Where(r => !r.IsNull("phone_number"))
                  .Select(r => Convert.ToString(r["phone_number"])!)
                  .Distinct()];
    }

    private static List<string> ExtractBenefits(DataTable table)
    {
      return [.. table.AsEnumerable()
                  .Where(r => !r.IsNull("benefit"))
                  .Select(r => Convert.ToString(r["benefit"])!)
                  .Distinct()];
    }

  }
}
