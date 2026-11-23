using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class FaqFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori zorunludur."), StringLength(100)]
    [Display(Name = "Kategori")]
    public string Category { get; set; } = "Genel";

    [Required(ErrorMessage = "Soru zorunludur."), StringLength(200)]
    [Display(Name = "Soru")]
    public string Question { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yanıt zorunludur."), StringLength(5000)]
    [Display(Name = "Yanıt")]
    public string Answer { get; set; } = string.Empty;

    [Range(0, 999)]
    [Display(Name = "Gösterim Sırası")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Yayında")]
    public bool IsActive { get; set; } = true;
}
