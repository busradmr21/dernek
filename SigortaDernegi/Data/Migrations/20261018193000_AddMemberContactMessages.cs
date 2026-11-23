using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261018193000_AddMemberContactMessages")]
    public partial class AddMemberContactMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecipientDisplayName",
                table: "ContactMessages",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecipientMemberProfileId",
                table: "ContactMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_RecipientMemberProfileId",
                table: "ContactMessages",
                column: "RecipientMemberProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_MemberProfiles_RecipientMemberProfileId",
                table: "ContactMessages",
                column: "RecipientMemberProfileId",
                principalTable: "MemberProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_MemberProfiles_RecipientMemberProfileId",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_RecipientMemberProfileId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "RecipientDisplayName",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "RecipientMemberProfileId",
                table: "ContactMessages");
        }
    }
}
