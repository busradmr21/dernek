using Microsoft.AspNetCore.Mvc;

namespace SigortaDernegi.Controllers
{
    public class MembershipController : Controller
    {
        // Üyelik Koşulları
        public IActionResult Conditions()
        {
            return View();
        }

        // Online Üyelik Başvuru Formu (GET)
        public IActionResult Apply()
        {
            return View();
        }

        // Online Üyelik Başvuru Formu (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(IFormCollection collection)
        {
            // Başvuru alındı mesajı göstermek için TempData kullanalım
            TempData["SuccessMessage"] = "Üyelik başvurunuz başarıyla alındı. Yönetim Kurulu incelemesinin ardından tarafınıza bilgilendirme yapılacaktır.";
            return RedirectToAction(nameof(Apply));
        }

        // Üye / Uzman Rehberi
        public IActionResult Directory()
        {
            return View();
        }
    }
}