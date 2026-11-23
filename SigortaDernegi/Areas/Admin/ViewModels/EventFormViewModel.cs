using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class EventFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Etkinlik başlığı zorunludur.")]
    [StringLength(200)]
    [Display(Name = "Etkinlik Başlığı")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kısa açıklama zorunludur.")]
    [StringLength(500)]
    [Display(Name = "Kısa Açıklama")]
    public string Summary { get; set; } = string.Empty;

    [StringLength(4000)]
    [Display(Name = "Etkinlik Detayı")]
    public string? Description { get; set; }

    [Display(Name = "Başlangıç Tarihi")]
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(1);

    [Display(Name = "Bitiş Tarihi")]
    public DateTime? EndDate { get; set; }

    [StringLength(100)]
    [Display(Name = "Etkinlik Türü")]
    public string? EventType { get; set; }

    [StringLength(200)]
    [Display(Name = "Konum")]
    public string? Location { get; set; }

    [Url(ErrorMessage = "Geçerli bir kayıt bağlantısı yazmalısınız.")]
    [StringLength(500)]
    [Display(Name = "Kayıt Bağlantısı")]
    public string? RegistrationUrl { get; set; }

    [Display(Name = "Çevrim İçi")]
    public bool IsOnline { get; set; }

    [Display(Name = "Öne Çıkan")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Yayında")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Etkinlik Görseli")]
    public IFormFile? ImageFile { get; set; }

    public string? ExistingImageUrl { get; set; }
}
