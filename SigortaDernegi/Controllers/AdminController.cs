using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;

namespace SigortaDernegi.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> Volunteers()
    {
        var volunteers = await _context.Volunteers
            .AsNoTracking()
            .OrderByDescending(volunteer => volunteer.CreatedDate)
            .ToListAsync();

        return View(volunteers);
    }

    public async Task<IActionResult> Donations()
    {
        var donations = await _context.Donations
            .AsNoTracking()
            .OrderByDescending(donation => donation.DonationDate)
            .ToListAsync();

        return View(donations);
    }
}
