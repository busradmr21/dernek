using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261020160000_AddFireDisasterEngineeringHomeSlider")]
    public partial class AddFireDisasterEngineeringHomeSlider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF NOT EXISTS (" +
                "SELECT 1 FROM [Sliders] " +
                "WHERE [ImageUrl] = N'~/images/sliders/yangin-dogal-afet-muhendislik-hasarlari.png') " +
                "BEGIN " +
                "INSERT INTO [Sliders] " +
                "([Title], [Description], [ImageUrl], [ButtonText], [ButtonUrl], [IsActive], [IsDeleted], [Order]) " +
                "VALUES (" +
                "N'Yangın, Doğal Afet ve Mühendislik Hasarlarında Teknik Uzmanlık', " +
                "N'Yangın, deprem, sel ve mühendislik kaynaklı hasarları; saha incelemesi, yapısal analiz ve bilimsel değerlendirme yöntemleriyle ele alıyoruz.', " +
                "N'~/images/sliders/yangin-dogal-afet-muhendislik-hasarlari.png', " +
                "NULL, NULL, 1, 0, 1); " +
                "END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM [Sliders] " +
                "WHERE [ImageUrl] = N'~/images/sliders/yangin-dogal-afet-muhendislik-hasarlari.png';");
        }
    }
}
