using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261006103000_AddDonationAndVolunteer")]
public class AddDonationAndVolunteer : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Donations",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DonorName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DonationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Donations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Volunteers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ExpertiseArea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Volunteers", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Donations");
        migrationBuilder.DropTable(name: "Volunteers");
    }
}
