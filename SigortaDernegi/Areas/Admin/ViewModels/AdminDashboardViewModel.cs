using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.ViewModels
{
    public class AdminDashboardViewModel
    {
        public bool CanViewMemberManagement { get; set; }
        public bool CanViewApplications { get; set; }
        public bool CanViewSiteContent { get; set; }
        public bool CanEditSiteContent { get; set; }
        public bool CanViewDuesAndPayments { get; set; }
        public bool CanViewDonations { get; set; }

        public int TotalMemberCount { get; set; }

        public int PendingMembershipApplicationCount { get; set; }

        public int NewContactMessageCount { get; set; }

        public int UpcomingEventCount { get; set; }

        public int ActiveNewsCount { get; set; }

        public int ActiveAnnouncementCount { get; set; }

        public int VolunteerApplicationCount { get; set; }

        public decimal TotalDonationAmount { get; set; }

        public List<MembershipApplication> RecentMembershipApplications
        { get; set; } = new List<MembershipApplication>();

        public List<ContactMessage> RecentContactMessages
        { get; set; } = new List<ContactMessage>();
    }
}
