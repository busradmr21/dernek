using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261019150000_AddDuesManagement")]
    public partial class AddDuesManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DuesPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PeriodType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuesPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberDues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DuesPeriodId = table.Column<int>(type: "int", nullable: false),
                    MemberProfileId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberDues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberDues_DuesPeriods_DuesPeriodId",
                        column: x => x.DuesPeriodId,
                        principalTable: "DuesPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberDues_MemberProfiles_MemberProfileId",
                        column: x => x.MemberProfileId,
                        principalTable: "MemberProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DuesPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberDueId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProviderPaymentId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    PaidAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    AdminNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RefundedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RefundedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuesPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuesPayments_MemberDues_MemberDueId",
                        column: x => x.MemberDueId,
                        principalTable: "MemberDues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DuesPayments_MemberDueId",
                table: "DuesPayments",
                column: "MemberDueId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberDues_DuesPeriodId_MemberProfileId",
                table: "MemberDues",
                columns: new[] { "DuesPeriodId", "MemberProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberDues_MemberProfileId",
                table: "MemberDues",
                column: "MemberProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DuesPayments");
            migrationBuilder.DropTable(name: "MemberDues");
            migrationBuilder.DropTable(name: "DuesPeriods");
        }
    }
}
