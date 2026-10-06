using System;
using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models
{
    public class Volunteer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur."), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon alanı zorunludur.")]
        public string Phone { get; set; } = string.Empty;

        public string ExpertiseArea { get; set; } = string.Empty; // Örn: Saha İncelemesi, Hukuk, Dijital Analiz
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
