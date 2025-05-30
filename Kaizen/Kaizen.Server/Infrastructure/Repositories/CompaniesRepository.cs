﻿using System.Data;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories;

public class CompaniesRepository(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("KaizenDb")
        ?? throw new InvalidOperationException(
               "The connection string 'KaizenDb' is not defined in appsettings.json.");

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
                    OtherSigns
                FROM
                    Companies";

        List<CompanyDto> companies = [];

        using SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, commandText, CommandType.Text);

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
                    OtherSigns
                FROM
                    Companies
                WHERE
                    CompanyPK = @CompanyPK";

        SqlParameter[] parameters =
        [
            new SqlParameter("@CompanyPK", companyPK),
        ];

        using SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, commandText, CommandType.Text, parameters);
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

            using SqlDataReader ownerReader = SqlHelper.ExecuteReader(this._connectionString, ownerNameCommandText, CommandType.Text, parameters);
            if (ownerReader.Read())
            {
                company.OwnerName = ownerReader.GetString(ownerReader.GetOrdinal("Name")) + " " +
                                    ownerReader.GetString(ownerReader.GetOrdinal("LastName"));
            }
        }

        return company;
    }
}
