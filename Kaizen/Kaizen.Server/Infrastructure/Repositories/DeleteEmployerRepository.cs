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
            p.ID,
            p.Name,
            p.LastName,
            u.UserPK,
            u.Email,
            gp.InCharge
        FROM Owners o
        LEFT JOIN Persons p ON o.OwnerPK = p.PersonPK
        LEFT JOIN Users u ON p.PersonPK = u.PersonPK
        OUTER APPLY (
            SELECT TOP 1 InCharge
            FROM GeneralPayrolls
            WHERE InCharge = u.Email
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
                    ID = reader.IsDBNull(reader.GetOrdinal("ID")) ? "" : reader.GetString(reader.GetOrdinal("ID")),
                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name")),
                    LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? "" : reader.GetString(reader.GetOrdinal("LastName")),
                    UserPK = reader.IsDBNull(reader.GetOrdinal("UserPK")) ? null : reader.GetGuid(reader.GetOrdinal("UserPK")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email")),
                    InCharge = reader.IsDBNull(reader.GetOrdinal("InCharge")) ? null : reader.GetString(reader.GetOrdinal("InCharge"))
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

        UPDATE Users
        SET Active = 0
        WHERE PersonPK = @OwnerPK;
    ";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
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
        DELETE FROM Users
        WHERE PersonPK = @OwnerPK;

        DELETE FROM Owners
        WHERE OwnerPK = @OwnerPK;

    ";

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
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
