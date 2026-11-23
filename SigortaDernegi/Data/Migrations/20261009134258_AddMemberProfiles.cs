using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RegistryNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Specialty = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PublicEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HasPublicationConsent = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberProfiles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberProfiles");
        }
    }
}
