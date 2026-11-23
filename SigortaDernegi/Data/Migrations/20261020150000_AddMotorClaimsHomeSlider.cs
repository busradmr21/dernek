using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261020150000_AddMotorClaimsHomeSlider")]
    public partial class AddMotorClaimsHomeSlider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF NOT EXISTS (" +
                "SELECT 1 FROM [Sliders] " +
                "WHERE [ImageUrl] = N'~/images/sliders/motorlu-araclar-trafik-hasarlari.png') " +
                "BEGIN " +
                "INSERT INTO [Sliders] " +
                "([Title], [Description], [ImageUrl], [ButtonText], [ButtonUrl], [IsActive], [IsDeleted], [Order]) " +
                "VALUES (" +
                "N'Motorlu Araçlar ve Trafik Hasarlarında Uzman Yaklaşım', " +
                "N'Kaza dinamikleri, araç hasarları ve kusur değerlendirmelerini bilimsel yöntemler, mesleki deneyim ve etik ilkelerle ele alıyoruz.', " +
                "N'~/images/sliders/motorlu-araclar-trafik-hasarlari.png', " +
                "NULL, NULL, 1, 0, 0); " +
                "END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM [Sliders] " +
                "WHERE [ImageUrl] = N'~/images/sliders/motorlu-araclar-trafik-hasarlari.png';");
        }
    }
}
