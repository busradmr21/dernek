using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models.Account
{
	public class Giris
	{
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta Adresi")]
        public string KullaniciMail { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string KullaniciSifre { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool BeniHatirla { get; set; }
        public string ReturnUrl { get; set; }

    }
}
