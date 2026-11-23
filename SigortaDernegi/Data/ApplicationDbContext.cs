using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Models;

namespace SigortaDernegi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MemberProfile>()
                .HasIndex(member => member.IdentityUserId)
                .IsUnique()
                .HasFilter("[IdentityUserId] IS NOT NULL");

            builder.Entity<MemberProfile>()
                .HasIndex(member => member.MembershipApplicationId)
                .IsUnique()
                .HasFilter("[MembershipApplicationId] IS NOT NULL");

            builder.Entity<MemberProfile>()
                .HasOne(member => member.IdentityUser)
                .WithOne()
                .HasForeignKey<MemberProfile>(member => member.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MemberProfile>()
                .HasOne(member => member.MembershipApplication)
                .WithOne(application => application.MemberProfile)
                .HasForeignKey<MemberProfile>(member => member.MembershipApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ContactMessage>()
                .HasOne(message => message.RecipientMemberProfile)
                .WithMany()
                .HasForeignKey(message => message.RecipientMemberProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ConfidentialReport>()
                .HasMany(report => report.Attachments)
                .WithOne(attachment => attachment.ConfidentialReport)
                .HasForeignKey(attachment => attachment.ConfidentialReportId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ConfidentialReport>()
                .HasIndex(report => report.CreatedAtUtc);

            builder.Entity<ConfidentialReport>()
                .HasIndex(report => report.Status);

            builder.Entity<DuesPeriod>()
                .Property(period => period.Amount)
                .HasPrecision(18, 2);

            builder.Entity<MemberDue>()
                .Property(due => due.Amount)
                .HasPrecision(18, 2);

            builder.Entity<DuesPayment>()
                .Property(payment => payment.Amount)
                .HasPrecision(18, 2);

            builder.Entity<DuesPayment>()
                .Property(payment => payment.RefundedAmount)
                .HasPrecision(18, 2);

            builder.Entity<MemberDue>()
                .HasIndex(due => new
                {
                    due.DuesPeriodId,
                    due.MemberProfileId
                })
                .IsUnique()
                .HasFilter("[MemberProfileId] IS NOT NULL");

            builder.Entity<MemberDue>()
                .HasIndex(due => new
                {
                    due.DuesPeriodId,
                    due.BoardMemberId
                })
                .IsUnique()
                .HasFilter("[BoardMemberId] IS NOT NULL");

            builder.Entity<MemberDue>()
                .ToTable(table => table.HasCheckConstraint(
                    "CK_MemberDues_ExactlyOnePayer",
                    "([MemberProfileId] IS NOT NULL AND [BoardMemberId] IS NULL) OR ([MemberProfileId] IS NULL AND [BoardMemberId] IS NOT NULL)"));

            builder.Entity<MemberDue>()
                .HasOne(due => due.DuesPeriod)
                .WithMany(period => period.MemberDues)
                .HasForeignKey(due => due.DuesPeriodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MemberDue>()
                .HasOne(due => due.MemberProfile)
                .WithMany()
                .HasForeignKey(due => due.MemberProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MemberDue>()
                .HasOne(due => due.BoardMember)
                .WithMany()
                .HasForeignKey(due => due.BoardMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DuesPayment>()
                .HasOne(payment => payment.MemberDue)
                .WithMany(due => due.Payments)
                .HasForeignKey(payment => payment.MemberDueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RelatedInstitution>()
                .HasIndex(institution => institution.DisplayOrder);

            builder.Entity<RelatedInstitution>().HasData(
                new RelatedInstitution
                {
                    Id = 1,
                    Abbreviation = "TSB",
                    Name = "Türkiye Sigorta Birliği",
                    WebsiteUrl = "https://www.tsb.org.tr/",
                    DisplayOrder = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc)
                },
                new RelatedInstitution
                {
                    Id = 2,
                    Abbreviation = "SBM",
                    Name = "Sigorta Bilgi ve Gözetim Merkezi",
                    WebsiteUrl = "https://www.sbm.org.tr/",
                    DisplayOrder = 2,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc)
                },
                new RelatedInstitution
                {
                    Id = 3,
                    Abbreviation = "TOBB SEİK",
                    Name = "Sigorta Eksperleri İcra Komitesi",
                    WebsiteUrl = "https://www.tobb.org.tr/SigortacilikMudurlugu/SigortaEksperleri/Sayfalar/SEIcraKomitesi.php",
                    DisplayOrder = 3,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc)
                },
                new RelatedInstitution
                {
                    Id = 4,
                    Abbreviation = "SEGEM",
                    Name = "Sigortacılık Eğitim Merkezi",
                    WebsiteUrl = "https://segem.org.tr/",
                    DisplayOrder = 4,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc)
                },
                new RelatedInstitution
                {
                    Id = 5,
                    Abbreviation = "T.C. STGM",
                    Name = "Sivil Toplumla İlişkiler Genel Müdürlüğü",
                    WebsiteUrl = "https://www.siviltoplum.gov.tr/",
                    DisplayOrder = 5,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc)
                },
                new RelatedInstitution
                {
                    Id = 6,
                    Abbreviation = "DERBİS",
                    Name = "Dernekler Bilgi Sistemi",
                    WebsiteUrl = "https://www.siviltoplum.gov.tr/dernekler-bilgi-sistemi-derbis",
                    DisplayOrder = 6,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc)
                },
                new RelatedInstitution
                {
                    Id = 7,
                    Abbreviation = "SEDDK",
                    Name = "Sigortacılık ve Özel Emeklilik Düzenleme ve Denetleme Kurumu",
                    WebsiteUrl = "https://seddk.gov.tr/",
                    DisplayOrder = 7,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc)
                },
                new RelatedInstitution
                {
                    Id = 8,
                    Abbreviation = "SGK",
                    Name = "Sosyal Güvenlik Kurumu",
                    WebsiteUrl = "https://www.sgk.gov.tr/",
                    DisplayOrder = 8,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc)
                });
        }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SliderRevision> SliderRevisions { get; set; }

        public DbSet<CorporatePage> CorporatePages { get; set; }
        public DbSet<CorporateContentSection> CorporateContentSections { get; set; }

        public DbSet<News> News { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Donation> Donations { get; set; }

        public DbSet<MembershipApplication> MembershipApplications { get; set; }
        public DbSet<MemberProfile> MemberProfiles { get; set; }

        public DbSet<AssociationEvent> AssociationEvents { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<ConfidentialReport> ConfidentialReports { get; set; }
        public DbSet<ConfidentialReportAttachment> ConfidentialReportAttachments { get; set; }

        public DbSet<DuesPeriod> DuesPeriods { get; set; }
        public DbSet<MemberDue> MemberDues { get; set; }
        public DbSet<DuesPayment> DuesPayments { get; set; }

        public DbSet<BoardMember> BoardMembers { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<RelatedInstitution> RelatedInstitutions { get; set; }
    }
}
