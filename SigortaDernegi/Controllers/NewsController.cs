using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;

namespace SigortaDernegi.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Haberler
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var news = await _context.News
                .AsNoTracking()
                .Where(item =>
                    item.IsActive &&
                    !item.IsAnnouncement)
                .OrderByDescending(item => item.CreatedAt)
                .ToListAsync();

            return View(news);
        }

        // Duyurular
        [HttpGet]
        public async Task<IActionResult> Announcements()
        {
            var announcements = await _context.News
                .AsNoTracking()
                .Where(item =>
                    item.IsActive &&
                    item.IsAnnouncement)
                .OrderByDescending(item => item.CreatedAt)
                .ToListAsync();

            return View(announcements);
        }

        // Haber veya duyuru detay sayfası
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var item = await _context.News
                .AsNoTracking()
                .FirstOrDefaultAsync(news =>
                    news.Id == id &&
                    news.IsActive);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }
    }
}