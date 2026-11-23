using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261020180000_AddDigitalInvestigationCyberRiskHomeSlider")]
    public partial class AddDigitalInvestigationCyberRiskHomeSlider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF NOT EXISTS (" +
                "SELECT 1 FROM [Sliders] " +
                "WHERE [ImageUrl] = N'~/images/sliders/dijital-inceleme-veri-analizi-siber-risk.png') " +
                "BEGIN " +
                "INSERT INTO [Sliders] " +
                "([Title], [Description], [ImageUrl], [ButtonText], [ButtonUrl], [IsActive], [IsDeleted], [Order]) " +
                "VALUES (" +
                "N'Dijital İnceleme, Veri Analizi ve Siber Risklerde Güvenli Yaklaşım', " +
                "N'Dijital delilleri, veri örüntülerini ve siber olayları; teknik inceleme, analitik yöntemler ve güvenli risk yönetimiyle değerlendiriyoruz.', " +
                "N'~/images/sliders/dijital-inceleme-veri-analizi-siber-risk.png', " +
                "NULL, NULL, 1, 0, 3); " +
                "END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM [Sliders] " +
                "WHERE [ImageUrl] = N'~/images/sliders/dijital-inceleme-veri-analizi-siber-risk.png';");
        }
    }
}
