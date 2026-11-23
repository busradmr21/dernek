using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261020190000_AddTransportMarineAviationRailHomeSlider")]
    public partial class AddTransportMarineAviationRailHomeSlider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF NOT EXISTS (" +
                "SELECT 1 FROM [Sliders] " +
                "WHERE [ImageUrl] = N'~/images/sliders/nakliye-denizcilik-havacilik-rayli-sistemler.png') " +
                "BEGIN " +
                "INSERT INTO [Sliders] " +
                "([Title], [Description], [ImageUrl], [ButtonText], [ButtonUrl], [IsActive], [IsDeleted], [Order]) " +
                "VALUES (" +
                "N'Nakliye, Denizcilik, Havacılık ve Raylı Sistemlerde Bütüncül Risk Yönetimi', " +
                "N'Kara, deniz, hava ve demir yolu taşımacılığındaki yük, operasyon ve sorumluluk risklerini; uzman inceleme ve bağlantılı lojistik analiziyle değerlendiriyoruz.', " +
                "N'~/images/sliders/nakliye-denizcilik-havacilik-rayli-sistemler.png', " +
                "NULL, NULL, 1, 0, 4); " +
                "END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM [Sliders] " +
                "WHERE [ImageUrl] = N'~/images/sliders/nakliye-denizcilik-havacilik-rayli-sistemler.png';");
        }
    }
}
