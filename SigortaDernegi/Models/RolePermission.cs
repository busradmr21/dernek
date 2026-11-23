using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models;

public enum ManagementArea
{
    [Display(Name = "Üye Yönetimi")]
    MemberManagement = 1,

    [Display(Name = "Aidat ve Ödemeler")]
    DuesAndPayments = 2,

    [Display(Name = "Site İçerikleri")]
    SiteContent = 4,

    [Display(Name = "Başvuru ve İletişim")]
    ApplicationsAndCommunication = 5,

    [Display(Name = "Yönetim Kurulu")]
    BoardManagement = 6,

    [Display(Name = "Gizli İhbarlar")]
    ConfidentialReports = 7,

    [Display(Name = "Ana Sayfa Etkinlik ve Haberleri")]
    HomeUpdates = 8,

    [Display(Name = "Bağış Yönetimi")]
    DonationManagement = 9
}

[Index(
    nameof(RoleName),
    nameof(Area),
    IsUnique = true)]
public class RolePermission
{
    public int Id { get; set; }

    [Required]
    [StringLength(64)]
    public string RoleName { get; set; } = string.Empty;

    [Required]
    public ManagementArea Area { get; set; }

    [Display(Name = "Görüntüle")]
    public bool CanView { get; set; }

    [Display(Name = "Ekle")]
    public bool CanAdd { get; set; }

    [Display(Name = "Düzenle")]
    public bool CanEdit { get; set; }

    [Display(Name = "Onayla")]
    public bool CanApprove { get; set; }

    [Display(Name = "Sil")]
    public bool CanDelete { get; set; }

    public DateTime UpdatedAtUtc { get; set; } =
        DateTime.UtcNow;

    [StringLength(256)]
    public string? UpdatedBy { get; set; }
}
