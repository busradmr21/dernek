using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models;

public enum BoardPosition
{
    [Display(Name = "Dernek Başkanı")]
    President = 1,

    [Display(Name = "Başkan Yardımcısı")]
    VicePresident = 2,

    [Display(Name = "Sayman")]
    Treasurer = 3,

    [Display(Name = "Genel Sekreter")]
    GeneralSecretary = 4
}

public class BoardMember
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Yönetim Görevi")]
    public BoardPosition Position { get; set; }

    [StringLength(150)]
    [Display(Name = "Mesleki Ünvan")]
    public string? ProfessionalTitle { get; set; }

    [StringLength(1000)]
    [Display(Name = "Kısa Tanıtım")]
    public string? Biography { get; set; }

    [StringLength(180)]
    [Display(Name = "Ana Sayfa Mesaj Başlığı")]
    public string? HomepageMessageTitle { get; set; }

    [StringLength(900)]
    [Display(Name = "Ana Sayfa Mesaj Özeti")]
    public string? HomepageMessageSummary { get; set; }

    [StringLength(4000)]
    [Display(Name = "Başkanın Tam Mesajı")]
    public string? PresidentMessageContent { get; set; }

    [StringLength(500)]
    [Display(Name = "Fotoğraf")]
    public string? PhotoUrl { get; set; }

    [Display(Name = "Görev Başlangıç Tarihi")]
    [DataType(DataType.Date)]
    public DateTime TermStart { get; set; } = DateTime.Today;

    [Display(Name = "Görev Bitiş Tarihi")]
    [DataType(DataType.Date)]
    public DateTime? TermEnd { get; set; }

    [StringLength(450)]
    public string? IdentityUserId { get; set; }

    public IdentityUser? IdentityUser { get; set; }

    [Display(Name = "Aktif Görevde")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Ana Sayfada Göster")]
    public bool IsPublished { get; set; } = true;

    [Display(Name = "Gösterim Sırası")]
    public int DisplayOrder { get; set; }
}
