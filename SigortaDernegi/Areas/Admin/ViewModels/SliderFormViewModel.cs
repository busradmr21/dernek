using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SigortaDernegi.Areas.Admin.ViewModels
{
	public class SliderFormViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Slider başlığı zorunludur.")]
		[StringLength(
			100,
			ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
		[Display(Name = "Başlık")]
		public string Title { get; set; } = string.Empty;

		[StringLength(
			250,
			ErrorMessage = "Açıklama en fazla 250 karakter olabilir.")]
		[Display(Name = "Açıklama")]
		public string? Description { get; set; }

		[StringLength(
			60,
			ErrorMessage = "Buton yazısı en fazla 60 karakter olabilir.")]
		[Display(Name = "Buton Yazısı")]
		public string? ButtonText { get; set; }

		[StringLength(
			500,
			ErrorMessage = "Buton bağlantısı en fazla 500 karakter olabilir.")]
		[Display(Name = "Buton Bağlantısı")]
		public string? ButtonUrl { get; set; }

		[Range(
			0,
			999,
			ErrorMessage = "Gösterim sırası 0 ile 999 arasında olmalıdır.")]
		[Display(Name = "Gösterim Sırası")]
		public int Order { get; set; }

		[Display(Name = "Yayında")]
		public bool IsActive { get; set; } = true;

		[Display(Name = "Slider Görseli")]
		public IFormFile? ImageFile { get; set; }

		public string? ExistingImageUrl { get; set; }
	}
}