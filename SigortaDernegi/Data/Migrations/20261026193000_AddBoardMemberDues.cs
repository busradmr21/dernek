using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261026193000_AddBoardMemberDues")]
    public partial class AddBoardMemberDues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberDues_DuesPeriodId_MemberProfileId",
                table: "MemberDues");

            migrationBuilder.AlterColumn<int>(
                name: "MemberProfileId",
                table: "MemberDues",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BoardMemberId",
                table: "MemberDues",
                type: "int",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_MemberDues_ExactlyOnePayer",
                table: "MemberDues",
                sql: "([MemberProfileId] IS NOT NULL AND [BoardMemberId] IS NULL) OR ([MemberProfileId] IS NULL AND [BoardMemberId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_MemberDues_BoardMemberId",
                table: "MemberDues",
                column: "BoardMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberDues_DuesPeriodId_BoardMemberId",
                table: "MemberDues",
                columns: new[] { "DuesPeriodId", "BoardMemberId" },
                unique: true,
                filter: "[BoardMemberId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MemberDues_DuesPeriodId_MemberProfileId",
                table: "MemberDues",
                columns: new[] { "DuesPeriodId", "MemberProfileId" },
                unique: true,
                filter: "[MemberProfileId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberDues_BoardMembers_BoardMemberId",
                table: "MemberDues",
                column: "BoardMemberId",
                principalTable: "BoardMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_MemberDues_ExactlyOnePayer",
                table: "MemberDues");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberDues_BoardMembers_BoardMemberId",
                table: "MemberDues");

            migrationBuilder.DropIndex(
                name: "IX_MemberDues_DuesPeriodId_BoardMemberId",
                table: "MemberDues");

            migrationBuilder.DropIndex(
                name: "IX_MemberDues_DuesPeriodId_MemberProfileId",
                table: "MemberDues");

            migrationBuilder.Sql(
                "DELETE FROM [MemberDues] WHERE [BoardMemberId] IS NOT NULL;");

            migrationBuilder.DropColumn(
                name: "BoardMemberId",
                table: "MemberDues");

            migrationBuilder.AlterColumn<int>(
                name: "MemberProfileId",
                table: "MemberDues",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberDues_DuesPeriodId_MemberProfileId",
                table: "MemberDues",
                columns: new[] { "DuesPeriodId", "MemberProfileId" },
                unique: true);
        }
    }
}
