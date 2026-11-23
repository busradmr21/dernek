using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261018175521_AddMembershipApprovalWorkflow")]
    public partial class AddMembershipApprovalWorkflow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "MemberProfiles",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MembershipApprovedAtUtc",
                table: "MemberProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MembershipApplicationId",
                table: "MemberProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "MemberProfiles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAtUtc",
                table: "MembershipApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                table: "MembershipApplications",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberProfiles_IdentityUserId",
                table: "MemberProfiles",
                column: "IdentityUserId",
                unique: true,
                filter: "[IdentityUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MemberProfiles_MembershipApplicationId",
                table: "MemberProfiles",
                column: "MembershipApplicationId",
                unique: true,
                filter: "[MembershipApplicationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberProfiles_AspNetUsers_IdentityUserId",
                table: "MemberProfiles",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberProfiles_MembershipApplications_MembershipApplicationId",
                table: "MemberProfiles",
                column: "MembershipApplicationId",
                principalTable: "MembershipApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberProfiles_AspNetUsers_IdentityUserId",
                table: "MemberProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberProfiles_MembershipApplications_MembershipApplicationId",
                table: "MemberProfiles");

            migrationBuilder.DropIndex(
                name: "IX_MemberProfiles_IdentityUserId",
                table: "MemberProfiles");

            migrationBuilder.DropIndex(
                name: "IX_MemberProfiles_MembershipApplicationId",
                table: "MemberProfiles");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "MemberProfiles");

            migrationBuilder.DropColumn(
                name: "MembershipApprovedAtUtc",
                table: "MemberProfiles");

            migrationBuilder.DropColumn(
                name: "MembershipApplicationId",
                table: "MemberProfiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "MemberProfiles");

            migrationBuilder.DropColumn(
                name: "ReviewedAtUtc",
                table: "MembershipApplications");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "MembershipApplications");
        }
    }
}
