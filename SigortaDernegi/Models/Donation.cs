using System;

namespace SigortaDernegi.Models
{
    public class Donation
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Bağışçı adı zorunludur.")]
        [System.ComponentModel.DataAnnotations.StringLength(150)]
        public string DonorName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Range(1, 100000000, ErrorMessage = "Bağış tutarı sıfırdan büyük olmalıdır.")]
        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Ödeme yöntemi zorunludur.")]
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; } = DateTime.Now;
        public string? Description { get; set; }
    }
}
