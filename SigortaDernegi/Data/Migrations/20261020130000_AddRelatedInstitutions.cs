using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261020130000_AddRelatedInstitutions")]
    public partial class AddRelatedInstitutions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelatedInstitutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Abbreviation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedInstitutions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelatedInstitutions_DisplayOrder",
                table: "RelatedInstitutions",
                column: "DisplayOrder");

            migrationBuilder.InsertData(
                table: "RelatedInstitutions",
                columns: new[]
                {
                    "Id", "Abbreviation", "Name", "WebsiteUrl",
                    "DisplayOrder", "IsActive", "CreatedAt", "UpdatedAt"
                },
                columnTypes: new[]
                {
                    "int", "nvarchar(20)", "nvarchar(180)", "nvarchar(500)",
                    "int", "bit", "datetime2", "datetime2"
                },
                values: new object[,]
                {
                    { 1, "TSB", "Türkiye Sigorta Birliği", "https://www.tsb.org.tr/", 1, true, new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, "SBM", "Sigorta Bilgi ve Gözetim Merkezi", "https://www.sbm.org.tr/", 2, true, new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, "TOBB SEİK", "Sigorta Eksperleri İcra Komitesi", "https://www.tobb.org.tr/SigortacilikMudurlugu/SigortaEksperleri/Sayfalar/SEIcraKomitesi.php", 3, true, new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, "SEGEM", "Sigortacılık Eğitim Merkezi", "https://segem.org.tr/", 4, true, new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, "STİGM", "Sivil Toplumla İlişkiler Genel Müdürlüğü", "https://www.siviltoplum.gov.tr/", 5, true, new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, "DERBİS", "Dernekler Bilgi Sistemi", "https://www.siviltoplum.gov.tr/dernekler-bilgi-sistemi-derbis", 6, true, new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, "SEDDK", "Sigortacılık ve Özel Emeklilik Düzenleme ve Denetleme Kurumu", "https://seddk.gov.tr/", 7, true, new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, "SGK", "Sosyal Güvenlik Kurumu", "https://www.sgk.gov.tr/", 8, true, new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RelatedInstitutions");
        }
    }
}
