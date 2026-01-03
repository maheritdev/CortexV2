using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Billing
{
    public class DetailsModel : PageModel
    {
        private readonly HospitalManagementSystemContext _context;

        public DetailsModel(HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public Cortex.Entities.Billing Billing { get; set; } = default!;

        // Display properties
        public string PatientName { get; set; } = "Unknown Patient";
        public string PatientAge { get; set; } = "N/A";
        public string PatientGender { get; set; } = "N/A";
        public string PatientPhone { get; set; } = "N/A";
        public string PatientEmail { get; set; } = "N/A";

        public string CreatedByName { get; set; } = "Unknown";

        public List<BillingDetail> BillingDetails { get; set; } = new List<BillingDetail>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load billing with all related data
            Billing = await _context.Billings
                .Include(b => b.Patient)
                .Include(b => b.CreatedByNavigation)
                .Include(b => b.BillingDetails)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(m => m.BillId == id);

            if (Billing == null)
            {
                return NotFound();
            }

            // Set patient information
            if (Billing.Patient != null)
            {
                PatientName = $"{Billing.Patient.FirstName} {Billing.Patient.LastName}";
                PatientAge = CalculateAge(Billing.Patient.DateOfBirth);
                PatientGender = Billing.Patient.Gender ?? "N/A";
                PatientPhone = Billing.Patient.EmergencyContactName ?? "N/A";
                PatientEmail = Billing.Patient.Email ?? "N/A";
            }

            // Set creator information
            if (Billing.CreatedByNavigation != null)
            {
                CreatedByName = $"{Billing.CreatedByNavigation.FirstName} {Billing.CreatedByNavigation.LastName}";
            }

            // Load billing details
            BillingDetails = await _context.BillingDetails
                .Where(bd => bd.BillId == id)
                .ToListAsync();

            // Ensure amounts are calculated
            if (!Billing.Balance.HasValue)
            {
                Billing.Balance = Billing.TotalAmount - (Billing.PaidAmount ?? 0);
            }

            return Page();
        }

        private string CalculateAge(DateOnly? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
                return "N/A";

            var today = DateOnly.FromDateTime(DateTime.Today);
            var birthDate = dateOfBirth.Value;

            var age = today.Year - birthDate.Year;

            // Adjust if birthday hasn't occurred this year
            if (birthDate > today.AddYears(-age))
                age--;

            // Calculate months if less than 1 year old
            if (age == 0)
            {
                var months = today.Month - birthDate.Month;
                if (months < 0)
                {
                    months += 12;
                }

                // Adjust for days within the month
                if (today.Day < birthDate.Day)
                {
                    months--;
                    if (months < 0) months = 11;
                }

                if (months == 0)
                {
                    var days = today.Day - birthDate.Day;
                    if (days < 0)
                    {
                        var previousMonth = today.AddMonths(-1);
                        days = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month) - birthDate.Day + today.Day;
                        months = 11; // This happens when crossing month boundaries with < 1 month age
                    }
                    return days == 1 ? "1 day" : $"{days} days";
                }

                return months == 1 ? "1 month" : $"{months} months";
            }

            // For ages 1 year or more
            return age == 1 ? "1 year" : $"{age} years";
        }
    }
}