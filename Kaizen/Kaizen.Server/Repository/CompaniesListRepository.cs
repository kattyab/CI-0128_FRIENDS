using System.Data;
using System.Reflection.PortableExecutable;
using Kaizen.Server.Models;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Repository;

// This class handles data access related to the list of companies
public class CompaniesListRepository(IConfiguration configuration)
{
    // Retrieves the connection string from appsettings.json
    private readonly string _connectionString =
        configuration.GetConnectionString("KaizenDb")
        ?? throw new InvalidOperationException(
               "The connection string 'KaizenDb' is not defined in appsettings.json.");

    // Retrieves a list of companies with their owner and employee count
    public List<CompaniesListDto> GetCompaniesList()
    {
        // SQL query to retrieve company details, owner name, and number of employees
        const string commandText = @"
        SELECT
            c.CompanyPK,
            c.CompanyID,
            c.OwnerPK,
            c.CompanyName,
            CONCAT(ISNULL(p.Name, ''), ' ', ISNULL(p.LastName, '')) AS OwnerName,
            COUNT(e.EmpID) AS EmployeesCount
        FROM
            Companies c
        INNER JOIN
            Persons p ON c.OwnerPK = p.PersonPK
        LEFT JOIN
            Employees e ON e.WorksFor = c.CompanyPK
        GROUP BY
            c.CompanyPK,
            c.CompanyID,
            c.OwnerPK,
            c.CompanyName,
            p.Name,
            p.LastName";

        // Initialize the list that will hold the result
        List<CompaniesListDto> companies = [];

        // Execute the SQL query using a helper and obtain a data reader
        using SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, commandText, CommandType.Text);

        // Loop through each row in the result set
        while (reader.Read())
        {
            // Create a DTO object and populate it with data from the current row
            CompaniesListDto dto = new()
            {
                CompanyPK = reader.GetGuid(reader.GetOrdinal("CompanyPK")),
                CompanyID = reader.GetString(reader.GetOrdinal("CompanyID")),
                OwnerPK = reader.GetGuid(reader.GetOrdinal("OwnerPK")),
                CompanyName = reader.GetString(reader.GetOrdinal("CompanyName")),
                OwnerName = reader.GetString(reader.GetOrdinal("OwnerName")),
                EmployeesCount = reader.GetInt32(reader.GetOrdinal("EmployeesCount"))
            };

            // Add the populated DTO to the list
            companies.Add(dto);
        }


        return companies;
    }
}
