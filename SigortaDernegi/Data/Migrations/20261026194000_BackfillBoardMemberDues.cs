using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261026194000_BackfillBoardMemberDues")]
    public partial class BackfillBoardMemberDues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [MemberDues]
                    ([DuesPeriodId], [MemberProfileId], [BoardMemberId], [Amount], [Currency], [Status], [AssignedAtUtc], [PaidAtUtc], [AdminNote])
                SELECT
                    period.[Id], NULL, boardMember.[Id], period.[Amount], period.[Currency], N'Ödenmedi', SYSUTCDATETIME(), NULL, N'Yönetim kurulu aidat entegrasyonu kapsamında otomatik tahakkuk.'
                FROM [DuesPeriods] AS period
                CROSS JOIN [BoardMembers] AS boardMember
                WHERE period.[IsActive] = 1
                  AND boardMember.[IsActive] = 1
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM [MemberDues] AS existingDue
                      WHERE existingDue.[DuesPeriodId] = period.[Id]
                        AND existingDue.[BoardMemberId] = boardMember.[Id]
                  );
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM [MemberDues]
                WHERE [BoardMemberId] IS NOT NULL
                  AND [AdminNote] = N'Yönetim kurulu aidat entegrasyonu kapsamında otomatik tahakkuk.'
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM [DuesPayments]
                      WHERE [DuesPayments].[MemberDueId] = [MemberDues].[Id]
                  );
                """);
        }
    }
}
