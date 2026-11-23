using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class MultimediaFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Kategori")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(200)]
    [Display(Name = "Başlık")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kısa özet zorunludur.")]
    [StringLength(1000)]
    [Display(Name = "Kısa Özet")]
    public string Summary { get; set; } = string.Empty;

    [Display(Name = "İçerik")]
    public string Content { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Kaynak Adı")]
    public string? SourceName { get; set; }

    [Url(ErrorMessage = "Geçerli bir kaynak bağlantısı yazmalısınız.")]
    [StringLength(500)]
    [Display(Name = "Kaynak Bağlantısı")]
    public string? SourceUrl { get; set; }

    [Display(Name = "Yayın Tarihi")]
    public DateTime PublishedAt { get; set; } = DateTime.Now;

    [Display(Name = "Yayında")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Foto Galeri Öğesi")]
    public bool IsGallery { get; set; }

    [Display(Name = "Kapak Görseli")]
    public IFormFile? ImageFile { get; set; }

    public string? ExistingImageUrl { get; set; }
}
