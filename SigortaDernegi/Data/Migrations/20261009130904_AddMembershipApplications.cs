using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMembershipApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MembershipApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MembershipType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HasAcceptedBylaws = table.Column<bool>(type: "bit", nullable: false),
                    HasReadKvkkNotice = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipApplications", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembershipApplications");
        }
    }
}
