using Microsoft.AspNetCore.Mvc;

namespace SigortaDernegi.Controllers
{
    public class AboutController : Controller
    {
        // Hakkımızda (Index)
        public IActionResult Index()
        {
            return View();
        }

        // Tüzük ve Etik İlkeler
        public IActionResult Bylaws()
        {
            return View();
        }

        // Yönetim ve Denetim Kurulu
        public IActionResult Board()
        {
            return View();
        }

        // Komisyonlar
        public IActionResult Committees()
        {
            return View();
        }
    }
}