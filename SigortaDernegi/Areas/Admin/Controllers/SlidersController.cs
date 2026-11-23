using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Areas.Admin.Authorization;
using SigortaDernegi.Areas.Admin.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeedData.ManagementRoles)]
[RequirePermission(ManagementArea.SiteContent, PermissionActionType.View)]
public class SlidersController : Controller
{
    private const long MaximumImageSize = 5 * 1024 * 1024;

    private static readonly string[] AllowedImageExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public SlidersController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var sliders = await _context.Sliders
            .AsNoTracking()
            .Where(slider => !slider.IsDeleted)
            .OrderBy(slider => slider.Order)
            .ThenBy(slider => slider.Id)
            .ToListAsync();

        return View(sliders);
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Add)]
    public async Task<IActionResult> Create()
    {
        var sliderQuery = _context.Sliders
            .Where(slider => !slider.IsDeleted);

        var nextOrder = await sliderQuery.AnyAsync()
            ? await sliderQuery.MaxAsync(slider => slider.Order) + 1
            : 0;

        return View(new SliderFormViewModel
        {
            Order = nextOrder,
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Add)]
    public async Task<IActionResult> Create(SliderFormViewModel model)
    {
        ValidateImage(model.ImageFile, imageIsRequired: true);
        ValidateButton(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var imageUrl = await SaveImageAsync(model.ImageFile!);

        var slider = new Slider
        {
            Title = model.Title.Trim(),
            Description = NormalizeOptionalValue(model.Description),
            ImageUrl = imageUrl,
            ButtonText = NormalizeOptionalValue(model.ButtonText),
            ButtonUrl = NormalizeOptionalValue(model.ButtonUrl),
            Order = model.Order,
            IsActive = model.IsActive
        };

        _context.Sliders.Add(slider);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            DeleteManagedImage(imageUrl);
            throw;
        }

        TempData["SuccessMessage"] = "Slider başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var slider = await _context.Sliders
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id && !item.IsDeleted);

        if (slider == null)
        {
            return NotFound();
        }

        return View(new SliderFormViewModel
        {
            Id = slider.Id,
            Title = slider.Title,
            Description = slider.Description,
            ButtonText = slider.ButtonText,
            ButtonUrl = slider.ButtonUrl,
            Order = slider.Order,
            IsActive = slider.IsActive,
            ExistingImageUrl = slider.ImageUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id, SliderFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var slider = await _context.Sliders
            .SingleOrDefaultAsync(item => item.Id == id && !item.IsDeleted);

        if (slider == null)
        {
            return NotFound();
        }

        ValidateImage(model.ImageFile, imageIsRequired: false);
        ValidateButton(model);

        if (!ModelState.IsValid)
        {
            model.ExistingImageUrl = slider.ImageUrl;
            return View(model);
        }

        var revision = CreateRevision(
            slider,
            "Düzenleme Öncesi");

        _context.SliderRevisions.Add(revision);

        string? newImageUrl = null;

        if (model.ImageFile is { Length: > 0 })
        {
            newImageUrl = await SaveImageAsync(model.ImageFile);
            slider.ImageUrl = newImageUrl;
        }

        slider.Title = model.Title.Trim();
        slider.Description = NormalizeOptionalValue(model.Description);
        slider.ButtonText = NormalizeOptionalValue(model.ButtonText);
        slider.ButtonUrl = NormalizeOptionalValue(model.ButtonUrl);
        slider.Order = model.Order;
        slider.IsActive = model.IsActive;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            if (newImageUrl != null)
            {
                DeleteManagedImage(newImageUrl);
            }

            throw;
        }

        TempData["SuccessMessage"] = "Slider başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var slider = await _context.Sliders
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id && !item.IsDeleted);

        if (slider == null)
        {
            return NotFound();
        }

        return View(slider);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Delete)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var slider = await _context.Sliders
            .SingleOrDefaultAsync(item => item.Id == id && !item.IsDeleted);

        if (slider == null)
        {
            return NotFound();
        }

        var revision = CreateRevision(
            slider,
            "Silinme Öncesi");

        _context.SliderRevisions.Add(revision);

        slider.IsDeleted = true;
        slider.IsActive = false;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Slider başarıyla silindi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Approve)]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var slider = await _context.Sliders
            .SingleOrDefaultAsync(item => item.Id == id && !item.IsDeleted);

        if (slider == null)
        {
            return NotFound();
        }

        var changeType = slider.IsActive
            ? "Yayından Kaldırma Öncesi"
            : "Yayına Alma Öncesi";

        var revision = CreateRevision(
            slider,
            changeType);

        _context.SliderRevisions.Add(revision);

        slider.IsActive = !slider.IsActive;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = slider.IsActive
            ? "Slider yayına alındı."
            : "Slider yayından kaldırıldı.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> History(int? sliderId)
    {
        IQueryable<SliderRevision> revisionQuery = _context.SliderRevisions
            .AsNoTracking();

        if (sliderId.HasValue)
        {
            revisionQuery = revisionQuery.Where(
                revision => revision.SliderId == sliderId.Value);
        }

        var revisions = await revisionQuery
            .OrderByDescending(revision => revision.ChangedAt)
            .ThenByDescending(revision => revision.Id)
            .ToListAsync();

        ViewBag.FilteredSliderId = sliderId;
        return View(revisions);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Restore(int id)
    {
        var revision = await _context.SliderRevisions
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id);

        if (revision == null)
        {
            return NotFound();
        }

        var slider = await _context.Sliders
            .SingleOrDefaultAsync(item => item.Id == revision.SliderId);

        if (slider == null)
        {
            slider = new Slider
            {
                Title = revision.Title,
                Description = revision.Description,
                ImageUrl = revision.ImageUrl,
                ButtonText = revision.ButtonText,
                ButtonUrl = revision.ButtonUrl,
                IsActive = revision.IsActive,
                IsDeleted = false,
                Order = revision.Order
            };

            _context.Sliders.Add(slider);
        }
        else
        {
            var currentRevision = CreateRevision(
                slider,
                "Geri Yükleme Öncesi");

            _context.SliderRevisions.Add(currentRevision);

            slider.Title = revision.Title;
            slider.Description = revision.Description;
            slider.ImageUrl = revision.ImageUrl;
            slider.ButtonText = revision.ButtonText;
            slider.ButtonUrl = revision.ButtonUrl;
            slider.IsActive = revision.IsActive;
            slider.IsDeleted = false;
            slider.Order = revision.Order;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Seçilen slider sürümü başarıyla geri yüklendi.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Delete)]
    public async Task<IActionResult> DeleteRevision(int id, int? sliderId)
    {
        var revision = await _context.SliderRevisions
            .SingleOrDefaultAsync(item => item.Id == id);

        if (revision == null)
        {
            return NotFound();
        }

        var imageUrl = revision.ImageUrl;

        _context.SliderRevisions.Remove(revision);
        await _context.SaveChangesAsync();

        await DeleteImageIfUnreferencedAsync(imageUrl);

        TempData["SuccessMessage"] = "Geçmiş sürümü başarıyla silindi.";

        if (sliderId.HasValue)
        {
            return RedirectToAction(nameof(History), new { sliderId });
        }

        return RedirectToAction(nameof(History));
    }

    private SliderRevision CreateRevision(
        Slider slider,
        string changeType)
    {
        return new SliderRevision
        {
            SliderId = slider.Id,
            Title = slider.Title,
            Description = slider.Description,
            ImageUrl = slider.ImageUrl,
            ButtonText = slider.ButtonText,
            ButtonUrl = slider.ButtonUrl,
            IsActive = slider.IsActive,
            Order = slider.Order,
            ChangeType = changeType,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = User.Identity?.Name
        };
    }

    private void ValidateImage(IFormFile? imageFile, bool imageIsRequired)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            if (imageIsRequired)
            {
                ModelState.AddModelError(
                    nameof(SliderFormViewModel.ImageFile),
                    "Slider görseli seçmelisiniz.");
            }

            return;
        }

        if (imageFile.Length > MaximumImageSize)
        {
            ModelState.AddModelError(
                nameof(SliderFormViewModel.ImageFile),
                "Görsel dosyası en fazla 5 MB olabilir.");
        }

        var extension = Path.GetExtension(imageFile.FileName);

        if (!AllowedImageExtensions.Contains(
                extension,
                StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                nameof(SliderFormViewModel.ImageFile),
                "Yalnızca JPG, JPEG, PNG veya WEBP görsel yükleyebilirsiniz.");
        }

        if (string.IsNullOrWhiteSpace(imageFile.ContentType) ||
            !imageFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                nameof(SliderFormViewModel.ImageFile),
                "Seçilen dosya geçerli bir görsel olmalıdır.");
        }
    }

    private void ValidateButton(SliderFormViewModel model)
    {
        var hasButtonText = !string.IsNullOrWhiteSpace(model.ButtonText);
        var hasButtonUrl = !string.IsNullOrWhiteSpace(model.ButtonUrl);

        if (hasButtonText != hasButtonUrl)
        {
            ModelState.AddModelError(
                nameof(SliderFormViewModel.ButtonText),
                "Buton kullanmak için hem buton yazısını hem bağlantısını girin.");

            ModelState.AddModelError(
                nameof(SliderFormViewModel.ButtonUrl),
                "Buton kullanmak için hem buton yazısını hem bağlantısını girin.");
        }

        if (hasButtonUrl && !IsSafeButtonUrl(model.ButtonUrl!))
        {
            ModelState.AddModelError(
                nameof(SliderFormViewModel.ButtonUrl),
                "Bağlantı / ile başlayan bir site adresi veya http/https adresi olmalıdır.");
        }
    }

    private static bool IsSafeButtonUrl(string url)
    {
        if (url.StartsWith("/", StringComparison.Ordinal) &&
            !url.StartsWith("//", StringComparison.Ordinal))
        {
            return true;
        }

        return Uri.TryCreate(url, UriKind.Absolute, out var parsedUrl) &&
               (parsedUrl.Scheme == Uri.UriSchemeHttp ||
                parsedUrl.Scheme == Uri.UriSchemeHttps);
    }

    private async Task<string> SaveImageAsync(IFormFile imageFile)
    {
        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var uploadFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "sliders");

        Directory.CreateDirectory(uploadFolder);

        var filePath = Path.Combine(uploadFolder, fileName);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await imageFile.CopyToAsync(fileStream);
        return $"/uploads/sliders/{fileName}";
    }

    private void DeleteManagedImage(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl) ||
            !imageUrl.StartsWith("/uploads/sliders/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var fileName = Path.GetFileName(imageUrl);
        var filePath = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "sliders",
            fileName);

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }

    private async Task DeleteImageIfUnreferencedAsync(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return;
        }

        var isUsedBySlider = await _context.Sliders
            .AnyAsync(slider => slider.ImageUrl == imageUrl);

        var isUsedByRevision = await _context.SliderRevisions
            .AnyAsync(revision => revision.ImageUrl == imageUrl);

        if (!isUsedBySlider && !isUsedByRevision)
        {
            DeleteManagedImage(imageUrl);
        }
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
