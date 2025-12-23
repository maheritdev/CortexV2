using Cortex.Services;
using Cortex.Viewmodel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cortex.UI.Areas.Web.Pages.Account
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,Roles = "Staff,Admin,Doctor,Nurse,Pharmacist,Lab Technician,Receptionist,IT Support")]
    public class DashboardModel : PageModel
    {
        private readonly IApiClient _api;
        public DashboardModel(IApiClient api, PatientsService patientsService, AppointmentService appointmentService, RoomService roomService, BillingService billingService)
        {
            _api = api;
            _patientsService = patientsService;
            _appointmentService = appointmentService;
            _roomService = roomService;
            _billingService = billingService;
        }

        private readonly PatientsService _patientsService;
        private readonly AppointmentService _appointmentService;
        private readonly RoomService _roomService;
        private readonly BillingService _billingService;
        public string DisplayName { get; set; } = "";
        public string UserType { get; set; } = "";
        public string Token { get; set; } = "";
        public string TokenExpiry { get; set; } = "";
        public DashboardViewModel Dashboard { get; set; } = new();

        public async Task OnGetAsync()
        {
            var name = User.Identity?.Name ?? "Unknown";
            DisplayName = name;
            UserType = User.FindFirst("user_type")?.Value ?? "n/a";
            Token = User.FindFirst("access_token")?.Value ?? "";


            var exp = User.FindFirst("exp")?.Value; // jwt exp if present
            TokenExpiry = exp ?? "(unknown)";

            // Read claims from the authenticated user
            DisplayName = User.FindFirstValue(ClaimTypes.Name) ?? "User";
            UserType = User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

            // Read expiry from token
            var expiryClaim = User.FindFirstValue(JwtRegisteredClaimNames.Exp);
            if (expiryClaim != null && long.TryParse(expiryClaim, out var expiryUnix))
            {
                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryUnix).DateTime;
                TokenExpiry = expiryDate.ToString("g");
            }
            else
            {
                TokenExpiry = "Unknown";
            }

            // For demonstration only - be careful with exposing tokens!
            Token = Request.Cookies["authToken"];

            Dashboard. RecentAppointments  = await _appointmentService.GetRecentAppointmentsAsync();

            Dashboard.TotalPatients = await _patientsService.GetPatientsCountAsync();

            // TODO: Wire up services for Appointments, Rooms, Billing
            Dashboard.TodaysAppointments = await _appointmentService.GetAppointmentCountAsync(); // placeholder
            Dashboard.AvailableRooms = await _roomService.GetRoomCountAsync(); // placeholder
            Dashboard.PendingBills = await _billingService.GetPendingBillingCountAsync();     // placeholder
        }

        // Add this to your IndexModel class
        public IActionResult OnPostLogout()
        {
            Response.Cookies.Delete("authToken");
            return RedirectToPage("/Web/Account/Login");
        }
    }
}