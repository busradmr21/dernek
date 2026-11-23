using SigortaDernegi.Models;
using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class BoardMemberAccountViewModel
{
    public int BoardMemberId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public BoardPosition Position { get; set; }

    public string RoleDisplayName { get; set; } =
        string.Empty;

    public bool HasAccount { get; set; }

    public bool IsPresident { get; set; }

    public string? ExistingEmail { get; set; }

    public string? CurrentAdminEmail { get; set; }

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(
        ErrorMessage = "Geçerli bir e-posta adresi yazınız.")]
    [Display(Name = "Giriş E-postası")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Geçici şifre zorunludur.")]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage =
            "Şifre en az 8 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Geçici Şifre")]
    public string TemporaryPassword { get; set; } =
        string.Empty;

    [Required(
        ErrorMessage = "Şifre tekrarı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(TemporaryPassword),
        ErrorMessage = "Yazılan şifreler aynı değil.")]
    [Display(Name = "Geçici Şifre Tekrarı")]
    public string ConfirmPassword { get; set; } =
        string.Empty;
}