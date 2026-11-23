using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261020140000_RenameCivilSocietyInstitutionAbbreviation")]
    public partial class RenameCivilSocietyInstitutionAbbreviation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE [RelatedInstitutions] " +
                "SET [Abbreviation] = N'T.C. STGM' " +
                "WHERE [Id] = 5 AND [Abbreviation] = N'STİGM';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE [RelatedInstitutions] " +
                "SET [Abbreviation] = N'STİGM' " +
                "WHERE [Id] = 5 AND [Abbreviation] = N'T.C. STGM';");
        }
    }
}
