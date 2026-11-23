using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261022160000_AddConfidentialReports")]
    public partial class AddConfidentialReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfidentialReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsuranceBranch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReferenceInformation = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    WantsContact = table.Column<bool>(type: "bit", nullable: false),
                    ReporterName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ReporterEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ReporterPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AdminNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfidentialReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfidentialReportAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfidentialReportId = table.Column<int>(type: "int", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfidentialReportAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfidentialReportAttachments_ConfidentialReports_ConfidentialReportId",
                        column: x => x.ConfidentialReportId,
                        principalTable: "ConfidentialReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfidentialReportAttachments_ConfidentialReportId",
                table: "ConfidentialReportAttachments",
                column: "ConfidentialReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfidentialReports_CreatedAtUtc",
                table: "ConfidentialReports",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ConfidentialReports_Status",
                table: "ConfidentialReports",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ConfidentialReportAttachments");
            migrationBuilder.DropTable(name: "ConfidentialReports");
        }
    }
}
