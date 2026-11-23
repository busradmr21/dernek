using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261020191000_ShortenTransportHomeSliderTitle")]
    public partial class ShortenTransportHomeSliderTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE [Sliders] " +
                "SET [Title] = N'Denizcilik, Havacılık ve Raylı Sistemlerde Risk Yönetimi' " +
                "WHERE [ImageUrl] = N'~/images/sliders/nakliye-denizcilik-havacilik-rayli-sistemler.png';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE [Sliders] " +
                "SET [Title] = N'Nakliye, Denizcilik, Havacılık ve Raylı Sistemlerde Bütüncül Risk Yönetimi' " +
                "WHERE [ImageUrl] = N'~/images/sliders/nakliye-denizcilik-havacilik-rayli-sistemler.png';");
        }
    }
}
