using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class NewsFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(200)]
    [Display(Name = "Başlık")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kısa özet zorunludur.")]
    [StringLength(1000)]
    [Display(Name = "Kısa Özet")]
    public string Summary { get; set; } = string.Empty;

    [Required(ErrorMessage = "İçerik zorunludur.")]
    [Display(Name = "İçerik")]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "Yayın Tarihi")]
    public DateTime PublishedAt { get; set; } = DateTime.Now;

    [Display(Name = "Duyuru")]
    public bool IsAnnouncement { get; set; }

    [Display(Name = "Yayında")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Kapak Görseli")]
    public IFormFile? ImageFile { get; set; }

    public string? ExistingImageUrl { get; set; }
}
