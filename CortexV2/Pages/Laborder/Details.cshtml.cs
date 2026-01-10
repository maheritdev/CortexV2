using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Laborder
{
    public class DetailsModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public DetailsModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public LabOrder LabOrder { get; set; } = default!;

        // Additional properties for better data access
        public string PatientFullName { get; set; } = string.Empty;
        public string DoctorFullName { get; set; } = string.Empty;
        public string? PatientContactInfo { get; set; }
        public string? DoctorContactInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Use Include to load related data (eager loading)
            var laborder = await _context.LabOrders
                .Include(l => l.Patient)          // Include Patient navigation property
                .Include(l => l.Doctor)           // Include Doctor navigation property
                .Include(l => l.LabOrderDetails)  // Include LabOrderDetails if needed
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (laborder == null)
            {
                return NotFound();
            }

            LabOrder = laborder;

            // Set additional properties for easier access in the view
            if (LabOrder.Patient != null)
            {
                PatientFullName = $"{LabOrder.Patient.FirstName} {LabOrder.Patient.LastName}";
                PatientContactInfo = FormatPatientContactInfo(LabOrder.Patient);
            }

            if (LabOrder.Doctor != null)
            {
                DoctorFullName = $"Dr. {LabOrder.Doctor.FirstName} {LabOrder.Doctor.LastName}";
                DoctorContactInfo = FormatDoctorContactInfo(LabOrder.Doctor);
            }

            return Page();
        }

        // Helper method to format patient contact information
        private string? FormatPatientContactInfo(Patient patient)
        {
            var contactInfo = new List<string>();

            if (!string.IsNullOrEmpty(patient.ContactNumber))
                contactInfo.Add($"📞 {patient.ContactNumber}");

            if (!string.IsNullOrEmpty(patient.Email))
                contactInfo.Add($"✉️ {patient.Email}");

            if (!string.IsNullOrEmpty(patient.Address))
                contactInfo.Add($"🏠 {patient.Address}");

            return contactInfo.Any() ? string.Join(" | ", contactInfo) : null;
        }

        // Helper method to format doctor contact information
        private string? FormatDoctorContactInfo(Cortex.Entities.Staff doctor)
        {
            var contactInfo = new List<string>();

            if (!string.IsNullOrEmpty(doctor.ContactNumber))
                contactInfo.Add($"📞 {doctor.ContactNumber}");

            if (!string.IsNullOrEmpty(doctor.Email))
                contactInfo.Add($"✉️ {doctor.Email}");

            if (!string.IsNullOrEmpty(doctor.Department?.Description))
                contactInfo.Add($"🏢 {doctor.Department}");

            return contactInfo.Any() ? string.Join(" | ", contactInfo) : null;
        }

        // Helper method to calculate age
        public int CalculateAge(DateOnly? birthDate)
        {
            if (!birthDate.HasValue)
                return 0;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - birthDate.Value.Year;

            // Check if birthday hasn't occurred yet this year
            if (birthDate.Value > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        // Helper method to get order urgency based on status and age
        public string GetOrderUrgency()
        {
            if (LabOrder.OrderDate.HasValue)
            {
                var daysSinceOrder = (DateTime.Today - LabOrder.OrderDate.Value).TotalDays;

                if (daysSinceOrder > 7 && LabOrder.Status != "Completed")
                    return "High";
                else if (daysSinceOrder > 3 && LabOrder.Status != "Completed")
                    return "Medium";
            }

            return "Normal";
        }

        // Helper method to get status badge color
        public string GetStatusBadgeClass()
        {
            return LabOrder.Status?.ToLower() switch
            {
                "completed" => "bg-success",
                "in progress" => "bg-primary",
                "sample collected" => "bg-info",
                "cancelled" => "bg-danger",
                "on hold" => "bg-warning",
                _ => "bg-secondary"
            };
        }
    }
}