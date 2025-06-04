using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public class CompanyDetailsRepository
    {
        private readonly string _connectionString;

        public CompanyDetailsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb");
        }

        public async Task<CompanyDetailsDto> GetCompanyDetailsByUserEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(@"
SELECT DISTINCT
    c.CompanyPK,
    c.CompanyID,
    CONCAT(po.Name, ' ', po.LastName) AS OwnerName,
    c.CompanyName,
    c.BrandName,
    c.Type,
    c.FoundationDate,
    c.MaxBenefits,
    c.WebPage,
    c.Description,
    c.PO,
    c.Province,
    c.Canton,
    c.Distrito,
    c.OtherSigns,
    c.Logo
FROM Companies c
INNER JOIN Persons po ON c.OwnerPK = po.PersonPK

INNER JOIN Users u ON u.Email = @Email
INNER JOIN Persons pu ON u.PersonPK = pu.PersonPK

LEFT JOIN Companies co ON c.CompanyPK = co.CompanyPK AND c.OwnerPK = pu.PersonPK

LEFT JOIN Admins a ON a.AdminPK = pu.PersonPK AND a.CompanyPK = c.CompanyPK

WHERE co.CompanyPK IS NOT NULL OR a.CompanyPK IS NOT NULL;
", connection);


            command.Parameters.AddWithValue("@Email", email);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows) return null;

            await reader.ReadAsync();
            CompanyDetailsDto companyDetailsDto = new()
            {
                CompanyPK = reader["CompanyPK"].ToString(),
                CompanyID = reader["CompanyID"]?.ToString(),
                OwnerName = reader["OwnerName"]?.ToString(),
                CompanyName = reader["CompanyName"]?.ToString(),
                BrandName = reader["BrandName"]?.ToString(),
                Type = reader["Type"]?.ToString(),
                FoundationDate = reader["FoundationDate"] != DBNull.Value ? Convert.ToDateTime(reader["FoundationDate"]) : null,
                MaxBenefits = Convert.ToInt32(reader["MaxBenefits"]),
                WebPage = reader["WebPage"]?.ToString(),
                Description = reader["Description"]?.ToString(),
                PO = reader["PO"]?.ToString(),
                Province = reader["Province"]?.ToString(),
                Canton = reader["Canton"]?.ToString(),
                Distrito = reader["Distrito"]?.ToString(),
                OtherSigns = reader["OtherSigns"]?.ToString(),
                Logo = reader["Logo"]?.ToString()
            };

            const string phoneNumbersCommandText = @"
                SELECT
                    Number
                FROM
                    CompaniesPhoneNumbers
                WHERE
                    CompanyPK = @CompanyPK";

            SqlParameter[] parameters =
            [
                new SqlParameter("@CompanyPK", companyDetailsDto.CompanyPK),
            ];

            using SqlDataReader phoneNumbersReader = SqlHelper.ExecuteReader(this._connectionString, phoneNumbersCommandText, CommandType.Text, parameters);
            List<string> phoneNumbers = [];
            while (phoneNumbersReader.Read())
            {
                phoneNumbers.Add(phoneNumbersReader.GetString(phoneNumbersReader.GetOrdinal("Number")));
            }

            companyDetailsDto.PhoneNumbers = string.Join(", ", phoneNumbers);

            const string emailsCommandText = @"
                SELECT
                    CompanyEmail
                FROM
                    CompaniesEmails
                WHERE
                    CompanyPK = @CompanyPK";

            parameters =
            [
                new SqlParameter("@CompanyPK", companyDetailsDto.CompanyPK),
            ];
            using SqlDataReader emailsReader = SqlHelper.ExecuteReader(this._connectionString, emailsCommandText, CommandType.Text, parameters);
            List<string> emails = [];
            while (emailsReader.Read())
            {
                emails.Add(emailsReader.GetString(emailsReader.GetOrdinal("CompanyEmail")));
            }
            companyDetailsDto.Emails = string.Join(", ", emails);

            return companyDetailsDto;
        }
    }
}
