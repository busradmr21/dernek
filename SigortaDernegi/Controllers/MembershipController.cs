using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.RateLimiting;
using SigortaDernegi.Data;
using SigortaDernegi.Models;
using SigortaDernegi.ViewModels;

namespace SigortaDernegi.Controllers
{
    public class MembershipController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _captchaProtector;

        public MembershipController(
            ApplicationDbContext context,
            IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _captchaProtector = dataProtectionProvider.CreateProtector(
                "SigortaDernegi.MemberContactCaptcha.v1");
        }

        // Üyelik koşulları ve tipleri
        public IActionResult Conditions()
        {
            return View();
        }

        // Üyelik başvuru formunu gösterir
        [HttpGet]
        public IActionResult Apply()
        {
            return View(new MembershipApplication());
        }

        // Üyelik başvurusunu kaydeder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(
            [Bind(
                "MembershipType," +
                "ApplicantName," +
                "Email," +
                "Phone," +
                "Specialty," +
                "City," +
                "Message," +
                "HasAcceptedBylaws," +
                "HasReadKvkkNotice"
            )]
            MembershipApplication application)
        {
            if (!application.HasAcceptedBylaws)
            {
                ModelState.AddModelError(
                    nameof(application.HasAcceptedBylaws),
                    "Tüzük ve etik ilkeleri kabul etmelisiniz.");
            }

            if (!application.HasReadKvkkNotice)
            {
                ModelState.AddModelError(
                    nameof(application.HasReadKvkkNotice),
                    "KVKK Aydınlatma Metni’ni okuyup bilgi edindiğinizi belirtmelisiniz.");
            }

            if (!ModelState.IsValid)
            {
                return View(application);
            }

            application.ApplicationStatus =
                MembershipApplicationStatuses.Pending;
            application.CreatedAt = DateTime.UtcNow;

            await _context.MembershipApplications.AddAsync(application);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Üyelik ön başvurunuz başarıyla alındı. " +
                "Değerlendirme sonrasında sizinle iletişime geçilecektir.";

            return RedirectToAction(nameof(Apply));
        }

        // Üye ve uzman rehberi
        public async Task<IActionResult> Directory(
            string? search,
            string? specialty,
            string? city)
        {
            var query = _context.MemberProfiles
                .AsNoTracking()
                .Where(member =>
                    member.IsActive &&
                    member.IsPublished &&
                    member.HasPublicationConsent);

            var specialties = await query
                .Select(member => member.Specialty)
                .Distinct()
                .OrderBy(value => value)
                .ToListAsync();

            var cities = await query
                .Select(member => member.City)
                .Distinct()
                .OrderBy(value => value)
                .ToListAsync();

            search = search?.Trim();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(member =>
                    member.DisplayName.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(specialty))
            {
                query = query.Where(member =>
                    member.Specialty == specialty);
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(member =>
                    member.City == city);
            }

            ViewBag.Search = search;
            ViewBag.SelectedSpecialty = specialty;
            ViewBag.SelectedCity = city;
            ViewBag.Specialties = specialties;
            ViewBag.Cities = cities;

            var members = await query
                .OrderBy(member => member.DisplayName)
                .ToListAsync();

            return View(members);
        }

        [HttpGet]
        public async Task<IActionResult> ContactMember(int id)
        {
            var member = await FindPublishedMemberAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            var model = CreateMemberContactModel(member);
            CreateCaptcha(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("member-contact")]
        public async Task<IActionResult> ContactMember(MemberContactViewModel model)
        {
            var member = await FindPublishedMemberAsync(model.MemberId);
            if (member == null)
            {
                return NotFound();
            }

            ApplyMemberDetails(model, member);

            if (!model.HasReadKvkkNotice)
            {
                ModelState.AddModelError(
                    nameof(model.HasReadKvkkNotice),
                    "KVKK Aydınlatma Metni’ni okuyup bilgi edindiğinizi belirtmelisiniz.");
            }

            if (!IsCaptchaValid(model.CaptchaToken, model.CaptchaAnswer))
            {
                ModelState.AddModelError(
                    nameof(model.CaptchaAnswer),
                    "Güvenlik sorusunun yanıtı hatalı veya süresi dolmuş.");
            }

            // Bot alanı doluysa başarılı yanıt verilir, mesaj kaydı oluşturulmaz.
            if (!string.IsNullOrWhiteSpace(model.Website))
            {
                TempData["SuccessMessage"] =
                    "Mesajınız üyeye ve dernek yönetimine iletildi.";
                return RedirectToAction(nameof(ContactMember), new { id = member.Id });
            }

            if (!ModelState.IsValid)
            {
                RefreshCaptchaAfterValidationFailure(model);
                return View(model);
            }

            var message = new ContactMessage
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim(),
                Subject = model.Subject.Trim(),
                Message = model.Message.Trim(),
                HasReadKvkkNotice = true,
                Status = "Yeni",
                RecipientMemberProfileId = member.Id,
                RecipientDisplayName = member.DisplayName,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ContactMessages.AddAsync(message);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Mesajınız üyeye ve dernek yönetimine iletildi. " +
                "Üye, güvenli panelinden mesajınızı görüntüleyebilir.";

            return RedirectToAction(nameof(ContactMember), new { id = member.Id });
        }

        private async Task<MemberProfile?> FindPublishedMemberAsync(int id) =>
            await _context.MemberProfiles
                .AsNoTracking()
                .SingleOrDefaultAsync(member =>
                    member.Id == id &&
                    member.IsActive &&
                    member.IsPublished &&
                    member.HasPublicationConsent);

        private static MemberContactViewModel CreateMemberContactModel(
            MemberProfile member)
        {
            var model = new MemberContactViewModel();
            ApplyMemberDetails(model, member);
            return model;
        }

        private static void ApplyMemberDetails(
            MemberContactViewModel model,
            MemberProfile member)
        {
            model.MemberId = member.Id;
            model.MemberName = member.DisplayName;
            model.MemberType = member.MemberType;
            model.Specialty = member.Specialty;
            model.City = member.City;
        }

        private void CreateCaptcha(MemberContactViewModel model)
        {
            var firstNumber = RandomNumberGenerator.GetInt32(1, 10);
            var secondNumber = RandomNumberGenerator.GetInt32(1, 10);
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();

            model.CaptchaQuestion = $"{firstNumber} + {secondNumber} kaç eder?";
            model.CaptchaToken = _captchaProtector.Protect(
                $"{firstNumber + secondNumber}:{expiresAt}");
            model.CaptchaAnswer = null;
        }

        private void RefreshCaptchaAfterValidationFailure(
            MemberContactViewModel model)
        {
            var captchaErrors = ModelState[nameof(model.CaptchaAnswer)]?.Errors
                .Select(error => error.ErrorMessage)
                .Where(error => !string.IsNullOrWhiteSpace(error))
                .ToList() ?? new List<string>();

            ModelState.Remove(nameof(model.CaptchaToken));
            ModelState.Remove(nameof(model.CaptchaAnswer));
            CreateCaptcha(model);

            foreach (var error in captchaErrors)
            {
                ModelState.AddModelError(nameof(model.CaptchaAnswer), error);
            }
        }

        private bool IsCaptchaValid(string? token, int? answer)
        {
            if (string.IsNullOrWhiteSpace(token) || !answer.HasValue)
            {
                return false;
            }

            try
            {
                var values = _captchaProtector.Unprotect(token).Split(':');
                return values.Length == 2 &&
                       int.TryParse(values[0], out var expectedAnswer) &&
                       long.TryParse(values[1], out var expiresAt) &&
                       DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= expiresAt &&
                       answer.Value == expectedAnswer;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }
    }
}
