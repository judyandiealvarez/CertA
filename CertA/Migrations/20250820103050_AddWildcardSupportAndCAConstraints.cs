using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertA.Migrations
{
    /// <inheritdoc />
    public partial class AddWildcardSupportAndCAConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Certificates_ExpiryDate",
                table: "Certificates",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_Status",
                table: "Certificates",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateAuthorities_IsActive",
                table: "CertificateAuthorities",
                column: "IsActive",
                unique: true,
                filter: "\"IsActive\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Certificates_ExpiryDate",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_Status",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_CertificateAuthorities_IsActive",
                table: "CertificateAuthorities");
        }
    }
}
