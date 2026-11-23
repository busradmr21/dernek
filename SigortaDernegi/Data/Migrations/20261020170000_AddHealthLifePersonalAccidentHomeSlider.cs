using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261020170000_AddHealthLifePersonalAccidentHomeSlider")]
    public partial class AddHealthLifePersonalAccidentHomeSlider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF NOT EXISTS (" +
                "SELECT 1 FROM [Sliders] " +
                "WHERE [ImageUrl] = N'~/images/sliders/saglik-hayat-ferdi-kaza.png') " +
                "BEGIN " +
                "INSERT INTO [Sliders] " +
                "([Title], [Description], [ImageUrl], [ButtonText], [ButtonUrl], [IsActive], [IsDeleted], [Order]) " +
                "VALUES (" +
                "N'Sağlık, Hayat ve Ferdi Kaza Risklerinde Güvenilir Değerlendirme', " +
                "N'Sağlık hizmetleri, yaşam güvencesi ve ferdi kaza süreçlerini; tıbbi veriler, risk analizi ve etik sigortacılık ilkeleriyle bütüncül olarak değerlendiriyoruz.', " +
                "N'~/images/sliders/saglik-hayat-ferdi-kaza.png', " +
                "NULL, NULL, 1, 0, 2); " +
                "END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM [Sliders] " +
                "WHERE [ImageUrl] = N'~/images/sliders/saglik-hayat-ferdi-kaza.png';");
        }
    }
}
