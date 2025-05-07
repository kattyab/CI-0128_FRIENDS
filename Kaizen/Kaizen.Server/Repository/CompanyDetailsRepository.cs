using Kaizen.Server.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kaizen.Server.Repository
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
            SELECT 
                c.CompanyPK,
                c.CompanyID,
                CONCAT(p.Name, ' ', p.LastName) AS OwnerName,
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
                c.OtherSigns,
                c.Logo
            FROM Companies c
            INNER JOIN Persons p ON c.OwnerPK = p.PersonPK
            INNER JOIN Users u ON u.PersonPK = p.PersonPK
            WHERE u.Email = @Email;", connection);

            command.Parameters.AddWithValue("@Email", email);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows) return null;

            await reader.ReadAsync();
            return new CompanyDetailsDto
            {
                CompanyPK = reader["CompanyPK"].ToString(),
                CompanyID = reader["CompanyID"]?.ToString(),
                OwnerName = reader["OwnerName"]?.ToString(),
                CompanyName = reader["CompanyName"]?.ToString(),
                BrandName = reader["BrandName"]?.ToString(),
                Type = reader["Type"]?.ToString(),
                FoundationDate = reader["FoundationDate"] != DBNull.Value ? Convert.ToDateTime(reader["FoundationDate"]) : (DateTime?)null,
                MaxBenefits = Convert.ToInt32(reader["MaxBenefits"]),
                WebPage = reader["WebPage"]?.ToString(),
                Description = reader["Description"]?.ToString(),
                PO = reader["PO"]?.ToString(),
                Province = reader["Province"]?.ToString(),
                Canton = reader["Canton"]?.ToString(),
                OtherSigns = reader["OtherSigns"]?.ToString(),
                Logo = reader["Logo"]?.ToString()
            };
        }
    }
}
