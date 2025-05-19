using Microsoft.Data.SqlClient;
using Kaizen.Server.Application.Dtos;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public class BenefitCreationRepository
    {
        private readonly string _connectionString;

        public BenefitCreationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException(
                    "La cadena de conexion 'KaizenDb' no está definida en appsettings.json");
        }

        public async Task<bool> CreateBenefit(BenefitCreationDto benefit)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    Console.WriteLine(benefit.AdminRole);
                    string getCompanySql = benefit.AdminRole switch
                    {
                        "Administrador" => @"
                    SELECT A.CompanyPK
                    FROM Admins A
                    JOIN Users U ON A.AdminPK = U.PersonPK
                    WHERE U.Email = @AdminEmail;",
                        "Dueño" => @"
                    SELECT C.CompanyPK
                    FROM Companies C
                    JOIN Persons P ON C.OwnerPK = P.PersonPK
                    JOIN Users U ON P.PersonPK= U.PersonPK
                    WHERE U.Email = @AdminEmail;",
                        _ => throw new Exception("Invalid role for retrieving company.")
                    };

                    string companyPK;
                    using (SqlCommand companyCmd = new SqlCommand(getCompanySql, conn))
                    {
                        companyCmd.Parameters.AddWithValue("@AdminEmail", benefit.AdminEmail);
                        var result = await companyCmd.ExecuteScalarAsync()
                            ?? throw new Exception("Admin email not found or not associated with a company.");
                        companyPK = result.ToString();
                    }

                    // Insert into the new Benefits table
                    string insertSql = @"
INSERT INTO Benefits (Name, MinWorkDurationMonths, OfferedBy, IsFixed, FixedValue, IsPercetange, PercentageValue, 
    IsAPI, Path, NumParameters, IsFullTime, IsPartTime, IsByHours, IsByService
)
VALUES (@Name, @MinWorkDurationMonths, @OfferedBy, @IsFixed, @FixedValue, @IsPercentage, @PercentageValue, 
    @IsAPI, @Path, @NumParameters, @IsFullTime, @IsPartTime, @IsByHours, @IsByService
);";

                    using SqlCommand cmd = new SqlCommand(insertSql, conn);
                    cmd.Parameters.AddWithValue("@Name", benefit.Name);
                    cmd.Parameters.AddWithValue("@MinWorkDurationMonths", benefit.MinWorkDurationMonths);
                    cmd.Parameters.AddWithValue("@OfferedBy", companyPK);
                    cmd.Parameters.AddWithValue("@IsFixed", benefit.IsFixed);
                    cmd.Parameters.AddWithValue("@FixedValue", benefit.FixedValue ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPercentage", benefit.IsPercentage);
                    cmd.Parameters.AddWithValue("@PercentageValue", benefit.PercentageValue ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsAPI", benefit.IsAPI);
                    cmd.Parameters.AddWithValue("@Path", string.IsNullOrEmpty(benefit.ApiPath) ? DBNull.Value : benefit.ApiPath);
                    cmd.Parameters.AddWithValue("@NumParameters", benefit.NumParameters ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsFullTime", benefit.IsFullTime);
                    cmd.Parameters.AddWithValue("@IsPartTime", benefit.IsPartTime);
                    cmd.Parameters.AddWithValue("@IsByHours", benefit.IsByHours);
                    cmd.Parameters.AddWithValue("@IsByService", benefit.IsByService);

                    await cmd.ExecuteNonQueryAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error inserting benefit data", ex);
                }
            }
        }
    }
}