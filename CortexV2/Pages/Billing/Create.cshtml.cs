using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Billing
{
    public class CreateModel : PageModel
    {
        private readonly HospitalManagementSystemContext _context;

        public CreateModel(HospitalManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cortex.Entities.Billing Billing { get; set; } = default!;

        public string CurrentUserName { get; set; } = "System User";

        public async Task<IActionResult> OnGet()
        {
            // Get patients and format names in memory (after ToListAsync)
            var patients = await _context.Patients
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Select(p => new
                {
                    p.PatientId,
                    p.FirstName,
                    p.LastName
                })
                .ToListAsync();

            var patientList = patients
                .Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(),
                    Text = $"{p.FirstName} {p.LastName}"
                })
                .ToList();

            ViewData["PatientId"] = new SelectList(patientList, "Value", "Text");

            // Get staff for CreatedBy dropdown
            var staff = await _context.Staff
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .Select(s => new
                {
                    s.StaffID,
                    s.FirstName,
                    s.LastName,
                    s.Role
                })
                .ToListAsync();

            var staffList = staff
                .Select(s => new SelectListItem
                {
                    Value = s.StaffID.ToString(),
                    Text = $"{s.FirstName} {s.LastName} - {s.Role}"
                })
                .ToList();

            ViewData["CreatedBy"] = new SelectList(staffList, "Value", "Text");

            // Initialize Billing with defaults
            Billing = new Cortex.Entities.Billing
            {
                BillDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                isPaid = false,
                Status = "Pending",
                PaidAmount = 0,
                Balance = 0,
                InsuranceClaimAmount = 0
            };

            // Try to set current user
            var currentUser = await GetCurrentUserAsync();
            if (currentUser != null)
            {
                CurrentUserName = $"{currentUser.FirstName} {currentUser.LastName}";
                Billing.CreatedBy = currentUser.StaffID;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns
                await RebindViewData();
                return Page();
            }

            // Auto-calculate balance
            Billing.PaidAmount ??= 0;
            Billing.Balance = Billing.TotalAmount - Billing.PaidAmount.Value;

            // Auto-set status
            if (Billing.isPaid == true)
            {
                Billing.Status = "Paid";
                Billing.PaidAmount = Billing.TotalAmount;
                Billing.Balance = 0;
                Billing.PaidDate ??= DateTime.Now;
            }
            else if (Billing.PaidAmount > 0)
            {
                Billing.Status = Billing.PaidAmount == Billing.TotalAmount ? "Paid" : "Partial";
                if (Billing.PaidAmount == Billing.TotalAmount)
                {
                    Billing.isPaid = true;
                    Billing.PaidDate ??= DateTime.Now;
                }
            }
            else
            {
                Billing.Status = "Pending";
            }

            // Ensure dates are set
            Billing.BillDate ??= DateTime.Now;
            Billing.DueDate ??= DateTime.Now.AddDays(30);

            // Set current user as creator
            var currentUser = await GetCurrentUserAsync();
            if (currentUser != null)
            {
                Billing.CreatedBy = currentUser.StaffID;
            }

            _context.Billings.Add(Billing);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task RebindViewData()
        {
            // Re-populate patients
            var patients = await _context.Patients
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Select(p => new
                {
                    p.PatientId,
                    p.FirstName,
                    p.LastName
                })
                .ToListAsync();

            var patientList = patients
                .Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(),
                    Text = $"{p.FirstName} {p.LastName} (ID: {p.PatientId})",
                    Selected = p.PatientId == Billing.PatientId
                })
                .ToList();

            ViewData["PatientId"] = new SelectList(patientList, "Value", "Text", Billing.PatientId);

            // Re-populate staff
            var staff = await _context.Staff
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .Select(s => new
                {
                    s.StaffID,
                    s.FirstName,
                    s.LastName,
                    s.Role
                })
                .ToListAsync();

            var staffList = staff
                .Select(s => new SelectListItem
                {
                    Value = s.StaffID.ToString(),
                    Text = $"{s.FirstName} {s.LastName} - {s.Role}",
                    Selected = s.StaffID == Billing.CreatedBy
                })
                .ToList();

            ViewData["CreatedBy"] = new SelectList(staffList, "Value", "Text", Billing.CreatedBy);
        }

        private async Task<Cortex.Entities.Staff> GetCurrentUserAsync()
        {
            if (!string.IsNullOrEmpty(User.Identity?.Name))
            {
                return await _context.Staff
                    .FirstOrDefaultAsync(s =>
                        s.Email == User.Identity.Name ||
                        s.Username == User.Identity.Name);
            }
            return null;
        }
    }
}