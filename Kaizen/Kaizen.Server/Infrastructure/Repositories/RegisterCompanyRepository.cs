using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public class RegisterCompanyRepository
    {
        private readonly string _connectionString;

        public RegisterCompanyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException(
                    "La cadena de conexion 'KaizenDb' no está definida en appsettings.json");
        }

        public async Task<bool> CreateCompany(RegisterCompanyDto company)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using var transaction = conn.BeginTransaction();

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

                    using SqlCommand cmd = new SqlCommand(insertSql, conn, transaction);
                    // Owner (Persona)
                    cmd.Parameters.AddWithValue("@PersonPK", personPK);
                    cmd.Parameters.AddWithValue("@Id", company.owner.Id);
                    cmd.Parameters.AddWithValue("@Name", company.owner.Name);
                    cmd.Parameters.AddWithValue("@LastName", company.owner.LastName);
                    cmd.Parameters.AddWithValue("@Sex", company.owner.Sex);
                    cmd.Parameters.AddWithValue("@BirthDate", company.owner.BirthDate);
                    cmd.Parameters.AddWithValue("@OwnerProvince", company.owner.Province);
                    cmd.Parameters.AddWithValue("@OwnerCanton", company.owner.Canton);
                    cmd.Parameters.AddWithValue("@OwnerOtherSigns", company.owner.OtherSigns);

                    // Usuario
                    cmd.Parameters.AddWithValue("@UserPK", userPK);
                    cmd.Parameters.AddWithValue("@Email", company.user.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    cmd.Parameters.AddWithValue("@Active", company.user.Active);
                    cmd.Parameters.AddWithValue("@Role", company.user.Role);

                    // Empresa
                    cmd.Parameters.AddWithValue("@CompanyPK", companyPK);
                    cmd.Parameters.AddWithValue("@CompanyID", company.CompanyID);
                    cmd.Parameters.AddWithValue("@OwnerPK", personPK);
                    cmd.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                    cmd.Parameters.AddWithValue("@BrandName", company.BrandName);
                    cmd.Parameters.AddWithValue("@Type", company.Type);
                    cmd.Parameters.AddWithValue("@FoundationDate", (object?)company.FoundationDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaxBenefits", company.MaxBenefits);
                    cmd.Parameters.AddWithValue("@WebPage", (object?)company.WebPage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Logo", (object?)company.Logo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", company.Description);
                    cmd.Parameters.AddWithValue("@PO", (object?)company.PO ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Province", company.Province);
                    cmd.Parameters.AddWithValue("@Canton", company.Canton);
                    cmd.Parameters.AddWithValue("@District", company.District);
                    cmd.Parameters.AddWithValue("@OtherSigns", company.OtherSigns);

                    await cmd.ExecuteNonQueryAsync();

                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"[ERROR REPO] {ex.Message} - {ex.InnerException?.Message}");
                    throw new Exception("Error creating company", ex);
                }
            }
        }
    }
}