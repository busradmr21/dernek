using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SigortaDernegi.Data;

#nullable disable

namespace SigortaDernegi.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261026120000_AddVolunteerKvkkAcknowledgement")]
public partial class AddVolunteerKvkkAcknowledgement : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "HasReadKvkkNotice",
            table: "Volunteers",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "HasReadKvkkNotice",
            table: "Volunteers");
    }
}
