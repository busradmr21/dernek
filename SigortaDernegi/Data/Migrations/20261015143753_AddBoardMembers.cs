using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    ProfessionalTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Biography = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TermStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TermEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardMembers_AspNetUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardMembers_IdentityUserId",
                table: "BoardMembers",
                column: "IdentityUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardMembers");
        }
    }
}
