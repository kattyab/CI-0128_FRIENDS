
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public class RegisterCompanyRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<RegisterCompanyRepository> _logger;

        public RegisterCompanyRepository(IConfiguration configuration, ILogger<RegisterCompanyRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException(
                    "La cadena de conexion 'KaizenDb' no está definida en appsettings.json");
            _logger = logger;
        }

        public async Task<bool> CreateCompany(RegisterCompanyDto company)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        Guid personPK = Guid.NewGuid();
                        Guid userPK = Guid.NewGuid();
                        Guid companyPK = Guid.NewGuid();

                        var hasher = new PasswordHasher<string>();
                        string hashedPassword = hasher.HashPassword(company.user.Email, company.user.PasswordHash);

                        string insertSql = @"
                            INSERT INTO Persons (PersonPK, Id, Name, LastName, Sex, BirthDate, Province, Canton, OtherSigns)
                            VALUES (@PersonPK, @Id, @Name, @LastName, @Sex, @BirthDate, @OwnerProvince, @OwnerCanton, @OwnerOtherSigns);

                            INSERT INTO Users (UserPK, Email, PasswordHash, Active, Role, PersonPK)
                            VALUES (@UserPK, @Email, @PasswordHash, @Active, @Role, @PersonPK);

                            INSERT INTO Companies (CompanyPK, CompanyID, OwnerPK, CompanyName, BrandName, Type, FoundationDate, MaxBenefits, WebPage, Logo, Description, PO, Province, Canton, Distrito, OtherSigns)
                            VALUES (@CompanyPK, @CompanyID, @OwnerPK, @CompanyName, @BrandName, @Type, @FoundationDate, @MaxBenefits, @WebPage, @Logo, @Description, @PO, @Province, @Canton, @District, @OtherSigns);
                            ";

                        using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                        {
                            // Owner (Persona)
                            cmd.Parameters.Add("@PersonPK", System.Data.SqlDbType.UniqueIdentifier).Value = personPK;
                            cmd.Parameters.Add("@Id", System.Data.SqlDbType.NVarChar, 50).Value = company.owner.Id;
                            cmd.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 100).Value = company.owner.Name;
                            cmd.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar, 100).Value = company.owner.LastName;
                            cmd.Parameters.Add("@Sex", System.Data.SqlDbType.NVarChar, 10).Value = company.owner.Sex;
                            cmd.Parameters.Add("@BirthDate", System.Data.SqlDbType.DateTime).Value = company.owner.BirthDate;
                            cmd.Parameters.Add("@OwnerProvince", System.Data.SqlDbType.NVarChar, 50).Value = company.owner.Province;
                            cmd.Parameters.Add("@OwnerCanton", System.Data.SqlDbType.NVarChar, 50).Value = company.owner.Canton;
                            cmd.Parameters.Add("@OwnerOtherSigns", System.Data.SqlDbType.NVarChar, 200).Value = (object?)company.owner.OtherSigns ?? DBNull.Value;

                            // User
                            cmd.Parameters.Add("@UserPK", System.Data.SqlDbType.UniqueIdentifier).Value = userPK;
                            cmd.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 100).Value = company.user.Email;
                            cmd.Parameters.Add("@PasswordHash", System.Data.SqlDbType.NVarChar, 200).Value = hashedPassword;
                            cmd.Parameters.Add("@Active", System.Data.SqlDbType.Bit).Value = company.user.Active;
                            cmd.Parameters.Add("@Role", System.Data.SqlDbType.NVarChar, 50).Value = company.user.Role;

                            // Company
                            cmd.Parameters.Add("@CompanyPK", System.Data.SqlDbType.UniqueIdentifier).Value = companyPK;
                            cmd.Parameters.Add("@CompanyID", System.Data.SqlDbType.NVarChar, 50).Value = company.CompanyID;
                            cmd.Parameters.Add("@OwnerPK", System.Data.SqlDbType.UniqueIdentifier).Value = personPK;
                            cmd.Parameters.Add("@CompanyName", System.Data.SqlDbType.NVarChar, 100).Value = company.CompanyName;
                            cmd.Parameters.Add("@BrandName", System.Data.SqlDbType.NVarChar, 100).Value = company.BrandName;
                            cmd.Parameters.Add("@Type", System.Data.SqlDbType.NVarChar, 50).Value = company.Type;
                            cmd.Parameters.Add("@FoundationDate", System.Data.SqlDbType.DateTime).Value = (object?)company.FoundationDate ?? DBNull.Value;
                            cmd.Parameters.Add("@MaxBenefits", System.Data.SqlDbType.Int).Value = company.MaxBenefits;
                            cmd.Parameters.Add("@WebPage", System.Data.SqlDbType.NVarChar, 200).Value = (object?)company.WebPage ?? DBNull.Value;
                            cmd.Parameters.Add("@Logo", System.Data.SqlDbType.NVarChar, 200).Value = (object?)company.Logo ?? DBNull.Value;
                            cmd.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar, 500).Value = company.Description;
                            cmd.Parameters.Add("@PO", System.Data.SqlDbType.NVarChar, 100).Value = (object?)company.PO ?? DBNull.Value;
                            cmd.Parameters.Add("@Province", System.Data.SqlDbType.NVarChar, 50).Value = company.Province;
                            cmd.Parameters.Add("@Canton", System.Data.SqlDbType.NVarChar, 50).Value = company.Canton;
                            cmd.Parameters.Add("@District", System.Data.SqlDbType.NVarChar, 50).Value = company.District;
                            cmd.Parameters.Add("@OtherSigns", System.Data.SqlDbType.NVarChar, 200).Value = company.OtherSigns;

                            await cmd.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            _logger.LogError(rollbackEx, "Error al hacer rollback de la transacción en RegisterCompanyRepository");
                        }
                        _logger.LogError(ex, "Error creando la compañía en RegisterCompanyRepository");
                        throw new Exception("Error creating company", ex);
                    }
                }
            }
        }
    }
}