using Microsoft.AspNetCore.Mvc;

namespace SigortaDernegi.Controllers
{
    public class ReportController : Controller
    {
        // İhbar ve Bildirim Sayfası (Form)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Form Gönderildiğinde Çalışacak Action (Şimdilik Başarılı Mesajına Yönlendirir)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit()
        {
            ViewBag.SuccessMessage = "Bildiriminiz gizlilik ilkelerine uygun olarak başarıyla alınmıştır. İnceleme komisyonumuz tarafından değerlendirilecektir.";
            return View("Index");
        }
    }
}
