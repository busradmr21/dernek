using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;

namespace SigortaDernegi.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private static readonly string[] EventTypes =
        {
            "Seminer",
            "Webinar",
            "Eğitim",
            "Söyleşi"
        };

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Etkinlikleri listeler ve kategoriye göre filtreler
        [HttpGet]
        public async Task<IActionResult> Index(string? type)
        {
            var selectedEventType = EventTypes.FirstOrDefault(
                eventType => string.Equals(
                    eventType,
                    type,
                    StringComparison.OrdinalIgnoreCase));

            var query = _context.AssociationEvents
                .AsNoTracking()
                .Where(associationEvent =>
                    associationEvent.IsActive);

            if (selectedEventType != null)
            {
                query = query.Where(associationEvent =>
                    associationEvent.EventType == selectedEventType);
            }

            var events = await query
                .OrderByDescending(associationEvent =>
                    associationEvent.IsFeatured)
                .ThenBy(associationEvent =>
                    associationEvent.StartDate)
                .ToListAsync();

            ViewBag.EventTypes = EventTypes;
            ViewBag.SelectedEventType = selectedEventType;

            return View(events);
        }

        // Seçilen etkinliğin ayrıntılarını gösterir
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var associationEvent = await _context.AssociationEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(eventItem =>
                    eventItem.Id == id &&
                    eventItem.IsActive);

            if (associationEvent == null)
            {
                return NotFound();
            }

            return View(associationEvent);
        }
    }
}