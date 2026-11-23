using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models;

public class ContactSettings
{
    [Required, StringLength(100)]
    [Display(Name = "Üst Başlık")]
    public string HeroEyebrow { get; set; } = "Bize Ulaşın";

    [Required, StringLength(200)]
    [Display(Name = "Sayfa Başlığı")]
    public string HeroTitle { get; set; } = "İletişim";

    [Required, StringLength(1000)]
    [Display(Name = "Sayfa Açıklaması")]
    public string HeroDescription { get; set; } = "Dernek faaliyetleri, üyelik süreçleri, etkinlikler ve kurumsal çalışmalar hakkında bizimle iletişime geçebilirsiniz.";

    [Required, StringLength(200)]
    [Display(Name = "Kurum Adı")]
    public string OrganizationName { get; set; } = "ELSADER Dernek Merkezi";

    [Required, StringLength(300)]
    [Display(Name = "Adres Birinci Satır")]
    public string AddressLine1 { get; set; } = "Barbaros Bulvarı No: 123 Kat: 4";

    [StringLength(200)]
    [Display(Name = "Adres İkinci Satır")]
    public string AddressLine2 { get; set; } = "Beşiktaş / İstanbul";

    [Required, StringLength(50)]
    [Display(Name = "Görünen Telefon")]
    public string PhoneDisplay { get; set; } = "+90 (212) 300 00 00";

    [Required, StringLength(30)]
    [Display(Name = "Telefon Bağlantısı")]
    public string PhoneLink { get; set; } = "+902123000000";

    [Required, EmailAddress, StringLength(150)]
    [Display(Name = "Genel E-posta")]
    public string Email { get; set; } = "info@hisigorta.org.tr";

    [Required, StringLength(100)]
    [Display(Name = "Hafta İçi Çalışma Saati")]
    public string WeekdayHours { get; set; } = "09.00–17.30";

    [Required, StringLength(100)]
    [Display(Name = "Cumartesi")]
    public string SaturdayHours { get; set; } = "Kapalı";

    [Required, StringLength(100)]
    [Display(Name = "Pazar")]
    public string SundayHours { get; set; } = "Kapalı";

    [Url, StringLength(1000)]
    [Display(Name = "Harita Gömme Bağlantısı")]
    public string? MapEmbedUrl { get; set; }

    [Url, StringLength(1000)]
    [Display(Name = "Haritada Aç Bağlantısı")]
    public string? MapLink { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "Instagram")]
    public string? InstagramUrl { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "LinkedIn")]
    public string? LinkedInUrl { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "YouTube")]
    public string? YouTubeUrl { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "X / Twitter")]
    public string? XUrl { get; set; }

    [EmailAddress, StringLength(150)]
    [Display(Name = "Üyelik Birimi E-postası")]
    public string? MembershipEmail { get; set; }

    [EmailAddress, StringLength(150)]
    [Display(Name = "Basın Birimi E-postası")]
    public string? PressEmail { get; set; }

    [EmailAddress, StringLength(150)]
    [Display(Name = "Muhasebe Birimi E-postası")]
    public string? FinanceEmail { get; set; }

    [Required, StringLength(1000)]
    [Display(Name = "Form Konuları")]
    public string SubjectOptions { get; set; } = "Genel Bilgi\nÜyelik İşlemleri\nEtkinlikler\nBasın ve Kurumsal İletişim\nDiğer";

    [Required, StringLength(1000)]
    [Display(Name = "Başarılı Gönderim Mesajı")]
    public string SuccessMessage { get; set; } = "Mesajınız dernek yönetimine başarıyla iletildi. En kısa sürede sizinle iletişime geçeceğiz.";

    [Required, StringLength(2000)]
    [Display(Name = "KVKK Bilgilendirmesi")]
    public string KvkkText { get; set; } = "KVKK Aydınlatma Metni’ni okudum ve kişisel verilerimin talebimin değerlendirilmesi amacıyla işlenmesi hakkında bilgi edindim.";

    [Required, StringLength(200)]
    [Display(Name = "Güvenlik Notu Başlığı")]
    public string SecurityNoteTitle { get; set; } = "Güvenli iletişim";

    [Required, StringLength(1000)]
    [Display(Name = "Güvenlik Notu")]
    public string SecurityNoteDescription { get; set; } = "İletişim bilgileriniz yalnızca talebinizi değerlendirmek ve size dönüş yapmak amacıyla kullanılır.";

    [Display(Name = "İletişim Formu Açık")]
    public bool FormEnabled { get; set; } = true;
}
