using System;
using System.Collections.Generic;
using System.Data;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.Employers;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public class DeleteEmployerRepository : IDeleteEmployerRepository
    {
        private readonly string _connectionString;

        public DeleteEmployerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException("Connection string 'KaizenDb' not found.");
        }

        public IEnumerable<DeleteEmployerDto> GetEmployersWithCompanyAndPersonData()
        {
            const string query = @"
            SELECT
                o.OwnerPK,
                o.IsDeleted AS OwnerIsDeleted,
                c.CompanyPK,
                c.CompanyName,
                c.IsDeleted AS CompanyIsDeleted,
                p.ID,
                p.Name,
                p.LastName,
                gp.PaidBy
            FROM Owners o
            LEFT JOIN Companies c ON o.OwnerPK = c.OwnerPK
            LEFT JOIN Persons p ON o.OwnerPK = p.PersonPK
            OUTER APPLY (
                SELECT TOP 1 PaidBy
                FROM GeneralPayrolls
                WHERE PaidBy = c.CompanyPK
                ORDER BY ExecutedOn DESC
            ) gp
            WHERE o.IsDeleted = 0;
            ";

            var result = new List<DeleteEmployerDto>();

            using SqlDataReader reader = SqlHelper.ExecuteReader(_connectionString, query, CommandType.Text);

            while (reader.Read())
            {
                var dto = new DeleteEmployerDto
                {
                    OwnerPK = reader.GetGuid(reader.GetOrdinal("OwnerPK")),
                    OwnerIsDeleted = reader.GetBoolean(reader.GetOrdinal("OwnerIsDeleted")),
                    CompanyPK = reader.IsDBNull(reader.GetOrdinal("CompanyPK")) ? null : reader.GetGuid(reader.GetOrdinal("CompanyPK")),
                    CompanyIsDeleted = reader.IsDBNull(reader.GetOrdinal("CompanyIsDeleted")) ? null : reader.GetBoolean(reader.GetOrdinal("CompanyIsDeleted")),
                    CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? "" : reader.GetString(reader.GetOrdinal("CompanyName")),
                    ID = reader.IsDBNull(reader.GetOrdinal("ID")) ? "" : reader.GetString(reader.GetOrdinal("ID")),
                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name")),
                    LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? "" : reader.GetString(reader.GetOrdinal("LastName")),
                    PaidBy = reader.IsDBNull(reader.GetOrdinal("PaidBy")) ? null : reader.GetGuid(reader.GetOrdinal("PaidBy"))
                };

                result.Add(dto);
            }

            return result;
        }

        public bool SoftDeleteEmployer(Guid ownerPK)
        {
            const string query = @"
        UPDATE Owners
        SET IsDeleted = 1
        WHERE OwnerPK = @OwnerPK;
    ";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction(); //Read commited por default
            try
            {
                using var command = new SqlCommand(query, connection, transaction);
                command.Parameters.Add(new SqlParameter("@OwnerPK", SqlDbType.UniqueIdentifier) { Value = ownerPK });

                int rowsAffected = command.ExecuteNonQuery();
                transaction.Commit();

                return rowsAffected > 0;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public bool HardDeleteEmployer(Guid ownerPK)
        {
            const string query = @"
        DELETE FROM Owners
        WHERE OwnerPK = @OwnerPK;
    ";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction(); // Read commited por default
            try
            {
                using var command = new SqlCommand(query, connection, transaction);
                command.Parameters.Add(new SqlParameter("@OwnerPK", SqlDbType.UniqueIdentifier) { Value = ownerPK });

                int rowsAffected = command.ExecuteNonQuery();
                transaction.Commit();

                return rowsAffected > 0;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

    }
}
