using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class RelatedInstitutionFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kurum kısaltması zorunludur.")]
    [StringLength(20, ErrorMessage = "Kısaltma en fazla 20 karakter olabilir.")]
    [Display(Name = "Kurum Kısaltması")]
    public string Abbreviation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kurum adı zorunludur.")]
    [StringLength(180, ErrorMessage = "Kurum adı en fazla 180 karakter olabilir.")]
    [Display(Name = "Kurumun Tam Adı")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Resmî internet adresi zorunludur.")]
    [StringLength(500, ErrorMessage = "İnternet adresi en fazla 500 karakter olabilir.")]
    [Url(ErrorMessage = "Geçerli bir internet adresi giriniz.")]
    [Display(Name = "Resmî İnternet Adresi")]
    public string WebsiteUrl { get; set; } = string.Empty;

    [Range(0, 999, ErrorMessage = "Gösterim sırası 0 ile 999 arasında olmalıdır.")]
    [Display(Name = "Gösterim Sırası")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Ana sayfada yayınlansın")]
    public bool IsActive { get; set; } = true;
}
