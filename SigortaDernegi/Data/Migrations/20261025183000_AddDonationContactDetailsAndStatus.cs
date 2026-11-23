using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SigortaDernegi.Data;

#nullable disable

namespace SigortaDernegi.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261025183000_AddDonationContactDetailsAndStatus")]
public partial class AddDonationContactDetailsAndStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "Donations",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Phone",
            table: "Donations",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "PaymentStatus",
            table: "Donations",
            type: "nvarchar(30)",
            maxLength: 30,
            nullable: false,
            defaultValue: "Geçmiş kayıt");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Email", table: "Donations");
        migrationBuilder.DropColumn(name: "Phone", table: "Donations");
        migrationBuilder.DropColumn(name: "PaymentStatus", table: "Donations");
    }
}
