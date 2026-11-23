using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCorporateContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporatePages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MenuTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HeroEyebrow = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HeroTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HeroDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporatePages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CorporateContentSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorporatePageId = table.Column<int>(type: "int", nullable: false),
                    SectionKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Eyebrow = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContentHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateContentSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateContentSections_CorporatePages_CorporatePageId",
                        column: x => x.CorporatePageId,
                        principalTable: "CorporatePages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporateContentSections_CorporatePageId",
                table: "CorporateContentSections",
                column: "CorporatePageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporateContentSections");

            migrationBuilder.DropTable(
                name: "CorporatePages");
        }
    }
}
