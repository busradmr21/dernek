using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var sliders = await _context.Sliders
            .Where(slider => slider.IsActive)
            .OrderBy(slider => slider.Order)
            .ToListAsync();

        var latestNews = await _context.News
            .Where(news => news.IsActive)
            .OrderByDescending(news => news.CreatedAt)
            .Take(6)
            .ToListAsync();

        ViewBag.Sliders = sliders;
        return View(latestNews);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Volunteer()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Volunteer(Volunteer volunteer)
    {
        if (!ModelState.IsValid)
        {
            return View(volunteer);
        }

        volunteer.CreatedDate = DateTime.Now;
        _context.Volunteers.Add(volunteer);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Gönüllü başvurunuz başarıyla alınmıştır. Teşekkür ederiz!";
        return RedirectToAction(nameof(Volunteer));
    }

    [HttpGet]
    public IActionResult Donate()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Donate(Donation donation)
    {
        if (!ModelState.IsValid)
        {
            return View(donation);
        }

        donation.DonationDate = DateTime.Now;
        _context.Donations.Add(donation);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Bağışınız başarıyla kaydedilmiştir. Destekleriniz için teşekkür ederiz!";
        return RedirectToAction(nameof(Donate));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
