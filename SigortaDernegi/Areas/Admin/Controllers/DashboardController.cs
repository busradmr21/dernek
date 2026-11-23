using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Areas.Admin.Authorization;
using SigortaDernegi.Areas.Admin.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = IdentitySeedData.ManagementRoles)]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRolePermissionService _permissionService;

        public DashboardController(
            ApplicationDbContext context,
            IRolePermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
        }

        public async Task<IActionResult> Index()
        {
            var currentDate = DateTime.Now;
            var canViewMemberManagement = await _permissionService.HasPermissionAsync(
                User, ManagementArea.MemberManagement, PermissionActionType.View);
            var canViewApplications = await _permissionService.HasPermissionAsync(
                User, ManagementArea.ApplicationsAndCommunication, PermissionActionType.View);
            var canViewSiteContent = await _permissionService.HasPermissionAsync(
                User, ManagementArea.SiteContent, PermissionActionType.View);
            var canEditSiteContent = await _permissionService.HasPermissionAsync(
                User, ManagementArea.SiteContent, PermissionActionType.Edit);
            var canViewDuesAndPayments = await _permissionService.HasPermissionAsync(
                User, ManagementArea.DuesAndPayments, PermissionActionType.View);
            var canViewDonations = await _permissionService.HasPermissionAsync(
                User, ManagementArea.DonationManagement, PermissionActionType.View);

            var model = new AdminDashboardViewModel
            {
                CanViewMemberManagement = canViewMemberManagement,
                CanViewApplications = canViewApplications,
                CanViewSiteContent = canViewSiteContent,
                CanEditSiteContent = canEditSiteContent,
                CanViewDuesAndPayments = canViewDuesAndPayments,
                CanViewDonations = canViewDonations,

                TotalMemberCount =
                    canViewMemberManagement
                        ? await _context.MemberProfiles.CountAsync()
                        : 0,

                PendingMembershipApplicationCount =
                    canViewApplications
                        ? await _context.MembershipApplications
                            .CountAsync(application =>
                                application.ApplicationStatus ==
                                MembershipApplicationStatuses.Pending)
                        : 0,

                NewContactMessageCount =
                    canViewApplications
                        ? await _context.ContactMessages
                            .CountAsync(message =>
                                message.Status == "Yeni")
                        : 0,

                UpcomingEventCount =
                    canViewSiteContent
                        ? await _context.AssociationEvents
                            .CountAsync(eventItem =>
                                eventItem.IsActive &&
                                eventItem.StartDate >= currentDate)
                        : 0,

                ActiveNewsCount =
                    canViewSiteContent
                        ? await _context.News
                            .CountAsync(news =>
                                news.IsActive &&
                                !news.IsAnnouncement)
                        : 0,

                ActiveAnnouncementCount =
                    canViewSiteContent
                        ? await _context.News
                            .CountAsync(news =>
                                news.IsActive &&
                                news.IsAnnouncement)
                        : 0,

                VolunteerApplicationCount =
                    canViewApplications
                        ? await _context.Volunteers.CountAsync()
                        : 0,

                TotalDonationAmount =
                    canViewDonations
                        ? await _context.Donations
                            .SumAsync(donation =>
                                (decimal?)donation.Amount) ?? 0
                        : 0,

                RecentMembershipApplications =
                    canViewApplications
                        ? await _context.MembershipApplications
                            .AsNoTracking()
                            .OrderByDescending(application =>
                                application.CreatedAt)
                            .Take(5)
                            .ToListAsync()
                        : new(),

                RecentContactMessages =
                    canViewApplications
                        ? await _context.ContactMessages
                            .AsNoTracking()
                            .OrderByDescending(message =>
                                message.CreatedAt)
                            .Take(5)
                            .ToListAsync()
                        : new()
            };

            return View(model);
        }
    }
}
