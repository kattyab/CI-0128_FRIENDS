using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.Companies;
using Kaizen.Server.Application.Interfaces.Companies;
using Kaizen.Server.Infrastructure.Contexts;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Transactions;

namespace Kaizen.Server.Infrastructure.Repositories;

public class CompaniesRepository : ICompaniesRepository
{
    private readonly string _connectionString;

    public CompaniesRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("KaizenDb")
            ?? throw new InvalidOperationException(
                   "The connection string 'KaizenDb' is not defined in appsettings.json.");
    }

    private static async Task EnsureOpenAsync(SqlConnection connection)
    {
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();
    }

    public List<CompanyDto> GetCompanies()
    {
        const string commandText = @"
                SELECT
                    CompanyPK,
                    CompanyID,
                    OwnerPK,
                    CompanyName,
                    BrandName,
                    Type,
                    FoundationDate,
                    MaxBenefits,
                    WebPage,
                    Logo,
                    Description,
                    PO,
                    Province,
                    Canton,
                    Distrito,
                    OtherSigns
                FROM
                    Companies";

        List<CompanyDto> companies = [];

        using SqlDataReader reader = SqlHelper.ExecuteReader(_connectionString, commandText, CommandType.Text);

        while (reader.Read())
        {
            CompanyDto company = new()
            {
                CompanyPK = reader.GetGuid(reader.GetOrdinal("CompanyPK")),
                CompanyID = reader.GetString(reader.GetOrdinal("CompanyID")),
                OwnerPK = reader.GetGuid(reader.GetOrdinal("OwnerPK")),
                CompanyName = reader.GetString(reader.GetOrdinal("CompanyName")),
                BrandName = reader.GetString(reader.GetOrdinal("BrandName")),
                Type = reader.GetString(reader.GetOrdinal("Type")),
                FoundationDate = reader.IsDBNull(reader.GetOrdinal("FoundationDate"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("FoundationDate")),
                MaxBenefits = reader.GetInt32(reader.GetOrdinal("MaxBenefits")),
                WebPage = reader.IsDBNull(reader.GetOrdinal("WebPage"))
                ? null
                : reader.GetString(reader.GetOrdinal("WebPage")),
                Logo = reader.IsDBNull(reader.GetOrdinal("Logo"))
                ? null
                : reader.GetString(reader.GetOrdinal("Logo")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null
                : reader.GetString(reader.GetOrdinal("Description")),
                PO = reader.IsDBNull(reader.GetOrdinal("PO"))
                ? null
                : reader.GetString(reader.GetOrdinal("PO")),
                Province = reader.IsDBNull(reader.GetOrdinal("Province"))
                ? null
                : reader.GetString(reader.GetOrdinal("Province")),
                Canton = reader.IsDBNull(reader.GetOrdinal("Canton"))
                ? null
                : reader.GetString(reader.GetOrdinal("Canton")),
                Distrito = reader.IsDBNull(reader.GetOrdinal("Distrito"))
                ? null
                : reader.GetString(reader.GetOrdinal("Distrito")),
                OtherSigns = reader.IsDBNull(reader.GetOrdinal("OtherSigns"))
                ? null
                : reader.GetString(reader.GetOrdinal("OtherSigns"))
            };

            companies.Add(company);
        }
        return companies;
    }

    public CompanyDto? GetCompany(Guid companyPK)
    {
        CompanyDto? company = null;

        const string commandText = @"
                SELECT
                TOP 1
                    CompanyPK,
                    CompanyID,
                    OwnerPK,
                    CompanyName,
                    BrandName,
                    Type,
                    FoundationDate,
                    MaxBenefits,
                    WebPage,
                    Logo,
                    Description,
                    PO,
                    Province,
                    Canton,
                    Distrito,
                    OtherSigns
                FROM
                    Companies
                WHERE
                    CompanyPK = @CompanyPK";

        SqlParameter[] parameters =
        [
            new SqlParameter("@CompanyPK", companyPK),
        ];

        using SqlDataReader reader = SqlHelper.ExecuteReader(_connectionString, commandText, CommandType.Text, parameters);
        if (reader.Read())
        {
            company = new()
            {
                CompanyPK = reader.GetGuid(reader.GetOrdinal("CompanyPK")),
                CompanyID = reader.GetString(reader.GetOrdinal("CompanyID")),
                OwnerPK = reader.GetGuid(reader.GetOrdinal("OwnerPK")),
                CompanyName = reader.GetString(reader.GetOrdinal("CompanyName")),
                BrandName = reader.GetString(reader.GetOrdinal("BrandName")),
                Type = reader.GetString(reader.GetOrdinal("Type")),
                FoundationDate = reader.IsDBNull(reader.GetOrdinal("FoundationDate"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("FoundationDate")),
                MaxBenefits = reader.GetInt32(reader.GetOrdinal("MaxBenefits")),
                WebPage = reader.IsDBNull(reader.GetOrdinal("WebPage"))
                ? null
                : reader.GetString(reader.GetOrdinal("WebPage")),
                Logo = reader.IsDBNull(reader.GetOrdinal("Logo"))
                ? null
                : reader.GetString(reader.GetOrdinal("Logo")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null
                : reader.GetString(reader.GetOrdinal("Description")),
                PO = reader.IsDBNull(reader.GetOrdinal("PO"))
                ? null
                : reader.GetString(reader.GetOrdinal("PO")),
                Province = reader.IsDBNull(reader.GetOrdinal("Province"))
                ? null
                : reader.GetString(reader.GetOrdinal("Province")),
                Canton = reader.IsDBNull(reader.GetOrdinal("Canton"))
                ? null
                : reader.GetString(reader.GetOrdinal("Canton")),
                Distrito = reader.IsDBNull(reader.GetOrdinal("Distrito"))
                ? null
                : reader.GetString(reader.GetOrdinal("Distrito")),
                OtherSigns = reader.IsDBNull(reader.GetOrdinal("OtherSigns"))
                ? null
                : reader.GetString(reader.GetOrdinal("OtherSigns"))
            };
        }

        if (company != null)
        {
            const string ownerNameCommandText = @"
                SELECT
                TOP 1
                    Name,
                    LastName
                FROM
                    Persons
                WHERE
                    PersonPK = @OwnerPK";

            parameters =
            [
                new SqlParameter("@OwnerPK", company.OwnerPK),
            ];

            using SqlDataReader ownerReader = SqlHelper.ExecuteReader(_connectionString, ownerNameCommandText, CommandType.Text, parameters);
            if (ownerReader.Read())
            {
                company.OwnerName = ownerReader.GetString(ownerReader.GetOrdinal("Name")) + " " +
                                    ownerReader.GetString(ownerReader.GetOrdinal("LastName"));
            }

            const string phoneNumbersCommandText = @"
                SELECT
                    Number
                FROM
                    CompaniesPhoneNumbers
                WHERE
                    CompanyPK = @CompanyPK";

            parameters =
            [
                new SqlParameter("@CompanyPK", company.CompanyPK),
            ];

            using SqlDataReader phoneNumbersReader = SqlHelper.ExecuteReader(_connectionString, phoneNumbersCommandText, CommandType.Text, parameters);
            List<string> phoneNumbers = [];
            while (phoneNumbersReader.Read())
            {
                phoneNumbers.Add(phoneNumbersReader.GetString(phoneNumbersReader.GetOrdinal("Number")));
            }

            company.PhoneNumbers = string.Join(", ", phoneNumbers);

            const string emailsCommandText = @"
                SELECT
                    CompanyEmail
                FROM
                    CompaniesEmails
                WHERE
                    CompanyPK = @CompanyPK";

            parameters =
            [
                new SqlParameter("@CompanyPK", company.CompanyPK),
            ];
            using SqlDataReader emailsReader = SqlHelper.ExecuteReader(_connectionString, emailsCommandText, CommandType.Text, parameters);
            List<string> emails = [];
            while (emailsReader.Read())
            {
                emails.Add(emailsReader.GetString(emailsReader.GetOrdinal("CompanyEmail")));
            }

            company.Emails = string.Join(", ", emails);

        }

        return company;
    }

    public void UpdateCompany(Guid companyPK, CompanyEditDto companyEditDto)
    {
        const string updateCompanyCommandText = @"
            UPDATE
                Companies
            SET
                CompanyName = @CompanyName,
                BrandName = @BrandName,
                MaxBenefits = @MaxBenefits,
                WebPage = @WebPage,
                Logo = @Logo,
                Description = @Description,
                PO = @PO,
                Province = @Province,
                Canton = @Canton,
                Distrito = @Distrito,
                OtherSigns = @OtherSigns
            WHERE
                CompanyPK = @CompanyPK
                AND IsDeleted = 0;";

        SqlParameter[] updateCompanyParameters = [
            new SqlParameter("@CompanyPK", companyPK),

            new SqlParameter("@CompanyName", companyEditDto.CompanyName),
            new SqlParameter("@BrandName", companyEditDto.BrandName),
            new SqlParameter("@MaxBenefits", companyEditDto.MaxBenefits),
            new SqlParameter("@WebPage", companyEditDto.WebPage),
            new SqlParameter("@Logo", (object?)companyEditDto.Logo ?? DBNull.Value),
            new SqlParameter("@Description", companyEditDto.Description),
            new SqlParameter("@PO", companyEditDto.PO),
            new SqlParameter("@Province", companyEditDto.Province),
            new SqlParameter("@Canton", companyEditDto.Canton),
            new SqlParameter("@Distrito", companyEditDto.Distrito),
            new SqlParameter("@OtherSigns", companyEditDto.OtherSigns),
        ];

        SqlHelper.ExecuteNonQuery(_connectionString,
            updateCompanyCommandText,
            CommandType.Text,
            updateCompanyParameters);

        string phoneNumbers = string.Join(",", companyEditDto.PhoneNumbers.Split(',')
            .Where(p => !string.IsNullOrWhiteSpace(p)));

        const string deletePhoneNumbersCommandText = @"
    IF (SELECT IsDeleted FROM Companies WHERE CompanyPK = @CompanyPK) = 0
    BEGIN
            DELETE FROM
                CompaniesPhoneNumbers
            WHERE
                CompanyPK = @CompanyPK AND
                Number NOT IN (SELECT value FROM STRING_SPLIT(@PhoneNumbers, ','))
    END;";

        SqlParameter[] deletePhoneNumbersParameters = [
            new SqlParameter("@CompanyPK", companyPK),
            new SqlParameter("@PhoneNumbers", phoneNumbers)
        ];

        SqlHelper.ExecuteNonQuery(_connectionString,
            deletePhoneNumbersCommandText,
            CommandType.Text,
            deletePhoneNumbersParameters);

        const string insertPhoneNumbersCommandText = @"
            IF (SELECT IsDeleted FROM Companies WHERE CompanyPK = @CompanyPK) = 0
            BEGIN
            INSERT INTO
                CompaniesPhoneNumbers (CompanyPK, Number)
            SELECT
                @CompanyPK, value
            FROM
                STRING_SPLIT(@PhoneNumbers, ',')
            WHERE
                value NOT IN (SELECT Number FROM CompaniesPhoneNumbers WHERE CompanyPK = @CompanyPK)
            END;";

        SqlParameter[] insertPhoneNumbersParameters = [
            new SqlParameter("@CompanyPK", companyPK),
            new SqlParameter("@PhoneNumbers", phoneNumbers)
        ];

        SqlHelper.ExecuteNonQuery(_connectionString,
            insertPhoneNumbersCommandText,
            CommandType.Text,
            insertPhoneNumbersParameters);

        string emails = string.Join(",", companyEditDto.Emails.Split(',')
            .Where(e => !string.IsNullOrWhiteSpace(e)));
        const string deleteEmailsCommandText = @"
            IF (SELECT IsDeleted FROM Companies WHERE CompanyPK = @CompanyPK) = 0
            BEGIN
            DELETE FROM
                CompaniesEmails
            WHERE
                CompanyPK = @CompanyPK AND
                CompanyEmail NOT IN (SELECT value FROM STRING_SPLIT(@Emails, ','))
            END;";

        SqlParameter[] deleteEmailsParameters = [
            new SqlParameter("@CompanyPK", companyPK),
            new SqlParameter("@Emails", emails)
        ];
        SqlHelper.ExecuteNonQuery(_connectionString,
            deleteEmailsCommandText,
            CommandType.Text,
            deleteEmailsParameters);

        const string insertEmailsCommandText = @"
            IF (SELECT IsDeleted FROM Companies WHERE CompanyPK = @CompanyPK) = 0
            BEGIN
            INSERT INTO
                CompaniesEmails (CompanyPK, CompanyEmail)
            SELECT
                @CompanyPK, value
            FROM
                STRING_SPLIT(@Emails, ',')
            WHERE
                value NOT IN (SELECT CompanyEmail FROM CompaniesEmails WHERE CompanyPK = @CompanyPK)
            END;";

        SqlParameter[] insertEmailsParameters = [
            new SqlParameter("@CompanyPK", companyPK),
            new SqlParameter("@Emails", emails)
        ];
        SqlHelper.ExecuteNonQuery(_connectionString,
            insertEmailsCommandText,
            CommandType.Text,
            insertEmailsParameters);
    }

    public async Task<bool> IsTherePayrollAsync(Guid companyPK, TransactionContext context)
    {
        SqlParameter[] checkParameters = [
            new SqlParameter("@CompanyPK", companyPK),
            new SqlParameter("@IsTherePayroll", SqlDbType.Bit) { Direction = ParameterDirection.Output }
        ];

        using SqlCommand checkCommand = new("sp_IsTherePayroll", context.Connection, context.Transaction);
        checkCommand.CommandType = CommandType.StoredProcedure;
        checkCommand.Parameters.AddRange(checkParameters);

        await checkCommand.ExecuteNonQueryAsync();

        return Convert.ToBoolean(checkParameters[1].Value);
    }

    public async Task SoftDeleteCompanyAsync(Guid companyPK, TransactionContext context)
    {
        SqlParameter[] deleteParameters = [
            new SqlParameter("@CompanyPK", companyPK)
        ];

        using SqlCommand deleteCommand = new("sp_SoftDeleteCompany", context.Connection, context.Transaction);
        deleteCommand.CommandType = CommandType.StoredProcedure;
        deleteCommand.Parameters.AddRange(deleteParameters);

        deleteCommand.ExecuteNonQuery();
    }

    public async Task FullDeleteCompanyAsync(Guid companyPK, TransactionContext context)
    {
        SqlParameter[] deleteParameters = [
            new SqlParameter("@CompanyPK", companyPK)
        ];

        using SqlCommand deleteCommand = new("sp_FullDeleteCompany", context.Connection, context.Transaction);
        deleteCommand.CommandType = CommandType.StoredProcedure;
        deleteCommand.Parameters.AddRange(deleteParameters);

        deleteCommand.ExecuteNonQuery();
    }
}