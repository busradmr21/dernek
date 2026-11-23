using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SigortaDernegi.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261022100000_AddPresidentHomepageMessage")]
    public partial class AddPresidentHomepageMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HomepageMessageSummary",
                table: "BoardMembers",
                type: "nvarchar(900)",
                maxLength: 900,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomepageMessageTitle",
                table: "BoardMembers",
                type: "nvarchar(180)",
                maxLength: 180,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresidentMessageContent",
                table: "BoardMembers",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE [BoardMembers] SET " +
                "[HomepageMessageTitle] = N'Güvenilir Bir Gelecek İçin Birlikte Çalışıyoruz', " +
                "[HomepageMessageSummary] = N'Sigorta araştırmalarında bilgi, etik ve uzmanlığı ortak bir zeminde buluşturuyor; mesleki standartları geliştirirken bağımsız değerlendirme kültürünü güçlendiriyoruz. Güvene dayalı, adil ve sürdürülebilir bir sigortacılık ekosistemi için sektörümüzün tüm paydaşlarıyla birlikte ilerliyoruz.', " +
                "[PresidentMessageContent] = N'Değerli üyelerimiz ve sektör paydaşlarımız,\n\nHİSAD olarak sigorta araştırmalarında bilgi, etik ve uzmanlığı ortak bir zeminde buluşturmayı temel sorumluluğumuz kabul ediyoruz. Mesleki standartların gelişmesine katkı sunarken bağımsız değerlendirme kültürünü, dayanışmayı ve güveni güçlendirmek için çalışıyoruz.\n\nGüvene dayalı, adil ve sürdürülebilir bir sigortacılık ekosistemi için sektörümüzün tüm paydaşlarıyla birlikte ilerlemeye devam edeceğiz.' " +
                "WHERE [Position] = 1;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomepageMessageSummary",
                table: "BoardMembers");

            migrationBuilder.DropColumn(
                name: "HomepageMessageTitle",
                table: "BoardMembers");

            migrationBuilder.DropColumn(
                name: "PresidentMessageContent",
                table: "BoardMembers");
        }
    }
}
