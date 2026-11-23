using Microsoft.AspNetCore.Http;
using SigortaDernegi.Models;
using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class BoardMemberFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad soyad alanı zorunludur.")]
    [StringLength(150)]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yönetim görevi seçilmelidir.")]
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

    [Display(Name = "Profil Fotoğrafı")]
    public IFormFile? PhotoFile { get; set; }

    public string? ExistingPhotoUrl { get; set; }

    [Required(ErrorMessage = "Görev başlangıç tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Görev Başlangıç Tarihi")]
    public DateTime TermStart { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    [Display(Name = "Görev Bitiş Tarihi")]
    public DateTime? TermEnd { get; set; }

    [Display(Name = "Aktif Görevde")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Sitede Göster")]
    public bool IsPublished { get; set; } = true;

    [Display(Name = "Gösterim Sırası")]
    [Range(0, 100)]
    public int DisplayOrder { get; set; }
}
