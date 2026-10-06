using Microsoft.AspNetCore.Mvc;

namespace SigortaDernegi.Controllers
{
    public class ContactController : Controller
    {
        // İletişim Sayfası
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // İletişim Formu Gönderildiğinde
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string name, string email, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(subject) ||
                string.IsNullOrWhiteSpace(message))
            {
                ModelState.AddModelError(string.Empty, "Lütfen tüm alanları doldurun.");
                return View();
            }

            ViewBag.SuccessMessage = "Mesajınız dernek yönetimine başarıyla iletilmiştir. En kısa sürede dönüş yapılacaktır.";
            return View();
        }
    }
}
