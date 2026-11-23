using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SigortaDernegi.Data;

#nullable disable

namespace SigortaDernegi.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261025173000_AddDonationAcknowledgements")]
public partial class AddDonationAcknowledgements : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "HasReadKvkkNotice",
            table: "Donations",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "HasReadDonationInformation",
            table: "Donations",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "HasReadKvkkNotice",
            table: "Donations");

        migrationBuilder.DropColumn(
            name: "HasReadDonationInformation",
            table: "Donations");
    }
}
