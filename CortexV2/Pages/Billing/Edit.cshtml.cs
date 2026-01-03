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
    public class EditModel : PageModel
    {
        private readonly HospitalManagementSystemContext _context;

        public EditModel(HospitalManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cortex.Entities.Billing Billing { get; set; } = default!;

        public string PatientName { get; set; } = "Unknown Patient";
        public string CreatedByName { get; set; } = "Unknown";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load billing with related data
            Billing = await _context.Billings
                .Include(b => b.Patient)
                .Include(b => b.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.BillId == id);

            if (Billing == null)
            {
                return NotFound();
            }

            // Set display names
            if (Billing.Patient != null)
            {
                PatientName = $"{Billing.Patient.FirstName} {Billing.Patient.LastName}";
            }

            if (Billing.CreatedByNavigation != null)
            {
                CreatedByName = $"{Billing.CreatedByNavigation.FirstName} {Billing.CreatedByNavigation.LastName}";
            }

            // Populate dropdowns with full names
            await PopulateDropdownsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return Page();
            }

            // Get existing billing record to preserve some fields
            var existingBilling = await _context.Billings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BillId == Billing.BillId);

            if (existingBilling == null)
            {
                return NotFound();
            }

            // Preserve original creator and creation date
            Billing.CreatedBy = existingBilling.CreatedBy;
            Billing.BillDate = existingBilling.BillDate;

            // Auto-calculate balance if not set or if amounts changed
            CalculateAndUpdateBalance();

            // Auto-update payment status
            UpdatePaymentStatus();

            // Set updated date
            Billing.DueDate = DateTime.Now;

            _context.Attach(Billing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillingExists(Billing.BillId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BillingExists(int id)
        {
            return _context.Billings.Any(e => e.BillId == id);
        }

        private async Task PopulateDropdownsAsync()
        {
            // Get patients with full names
            var patients = await _context.Patients
                .Select(p => new
                {
                    Id = p.PatientId,
                    FirstName = p.FirstName,
                    LastName = p.LastName
                })
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();

            var patientList = patients
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.FirstName} {p.LastName}",
                    Selected = p.Id == Billing.PatientId
                })
                .ToList();

            ViewData["PatientId"] = new SelectList(patientList, "Value", "Text", Billing.PatientId);

            // Get staff with full names for CreatedBy
            var staff = await _context.Staff
                .Select(s => new
                {
                    Id = s.StaffID,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Role = s.Role
                })
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();

            var staffList = staff
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FirstName} {s.LastName} - {s.Role}",
                    Selected = s.Id == Billing.CreatedBy
                })
                .ToList();

            ViewData["CreatedBy"] = new SelectList(staffList, "Value", "Text", Billing.CreatedBy);
        }

        private void CalculateAndUpdateBalance()
        {
            // Ensure PaidAmount has a value
            Billing.PaidAmount ??= 0;

            // Calculate balance: TotalAmount - PaidAmount
            Billing.Balance = Billing.TotalAmount - Billing.PaidAmount.Value;

            // Ensure balance is not negative (adjust PaidAmount if needed)
            if (Billing.Balance < 0)
            {
                Billing.Balance = 0;
                Billing.PaidAmount = Billing.TotalAmount;
            }

            // Ensure balance is not greater than total amount
            if (Billing.Balance > Billing.TotalAmount)
            {
                Billing.Balance = Billing.TotalAmount;
                Billing.PaidAmount = 0;
            }
        }

        private void UpdatePaymentStatus()
        {
            // Update isPaid based on balance
            if (Billing.Balance == 0 && Billing.TotalAmount > 0)
            {
                Billing.isPaid = true;
                // Auto-set paid date if not already set
                if (!Billing.PaidDate.HasValue)
                {
                    Billing.PaidDate = DateTime.Now;
                }
            }
            else if (Billing.PaidAmount > 0)
            {
                Billing.isPaid = false; // Partial payment
            }
            else
            {
                Billing.isPaid = false; // No payment
            }

            // Update status based on payment and dates
            UpdateInvoiceStatus();
        }

        private void UpdateInvoiceStatus()
        {
            if (Billing.isPaid == true)
            {
                Billing.Status = "Paid";
            }
            else if (Billing.PaidAmount > 0 && Billing.PaidAmount < Billing.TotalAmount)
            {
                Billing.Status = "Partial";
            }
            else if (Billing.DueDate < DateTime.Now)
            {
                Billing.Status = "Overdue";
            }
            else
            {
                Billing.Status = "Pending";
            }

            // Handle special cases
            if (Billing.Status == "Cancelled" || Billing.Status == "Refunded")
            {
                // Preserve these statuses if manually set
            }
        }
    }
}