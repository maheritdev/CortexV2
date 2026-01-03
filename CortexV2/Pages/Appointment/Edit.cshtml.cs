using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Appointment
{
    public class EditModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public EditModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cortex.Entities.Appointment Appointment { get; set; } = default!;

        public string CreatedByName { get; set; } = "Unknown";
        public string PatientName { get; set; } = "Unknown";
        public string DoctorName { get; set; } = "Unknown";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load appointment with related data
            Appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (Appointment == null)
            {
                return NotFound();
            }

            // Set display names
            if (Appointment.CreatedByNavigation != null)
            {
                CreatedByName = $"{Appointment.CreatedByNavigation.FirstName} {Appointment.CreatedByNavigation.LastName}";
            }

            if (Appointment.Patient != null)
            {
                PatientName = $"{Appointment.Patient.FirstName} {Appointment.Patient.LastName}";
            }

            if (Appointment.Doctor != null)
            {
                DoctorName = $"{Appointment.Doctor.FirstName} {Appointment.Doctor.LastName}";
            }

            // Populate ViewBag for dropdowns with full names
            ViewData["PatientId"] = new SelectList(_context.Patients
                .Select(p => new
                {
                    Id = p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .OrderBy(p => p.FullName), "Id", "FullName", Appointment.PatientId);

            ViewData["DoctorId"] = new SelectList(_context.Staff
                .Where(s => s.Role == "Doctor" || s.Role.Contains("Doctor"))
                .Select(s => new
                {
                    Id = s.StaffID,
                    FullName = s.FirstName + " " + s.LastName + " (" + s.Specialization + ")"
                })
                .OrderBy(s => s.FullName), "Id", "FullName", Appointment.DoctorId);

            ViewData["CreatedBy"] = new SelectList(_context.Staff
                .Select(s => new
                {
                    Id = s.StaffID,
                    FullName = s.FirstName + " " + s.LastName
                })
                .OrderBy(s => s.FullName), "Id", "FullName", Appointment.CreatedBy);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns if validation fails
                await PopulateDropdownsAsync();
                return Page();
            }

            // Get existing appointment to preserve original creator and date
            var existingAppointment = await _context.Appointments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AppointmentId == Appointment.AppointmentId);

            if (existingAppointment != null)
            {
                // Preserve original creator and creation date
                Appointment.CreatedBy = existingAppointment.CreatedBy;
                Appointment.CreatedDate = existingAppointment.CreatedDate;

                // Set updated date (if you have that field)
                // Appointment.UpdatedDate = DateTime.Now;
                // Appointment.UpdatedBy = GetCurrentUserId(); // Implement this method
            }

            _context.Attach(Appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(Appointment.AppointmentId))
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

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }

        private async Task PopulateDropdownsAsync()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients
                .Select(p => new
                {
                    Id = p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .OrderBy(p => p.FullName), "Id", "FullName", Appointment.PatientId);

            ViewData["DoctorId"] = new SelectList(_context.Staff
                .Where(s => s.Role == "Doctor" || s.Role.Contains("Doctor"))
                .Select(s => new
                {
                    Id = s.StaffID,
                    FullName = s.FirstName + " " + s.LastName + " (" + s.Specialization + ")"
                })
                .OrderBy(s => s.FullName), "Id", "FullName", Appointment.DoctorId);

            ViewData["CreatedBy"] = new SelectList(_context.Staff
                .Select(s => new
                {
                    Id = s.StaffID,
                    FullName = s.FirstName + " " + s.LastName
                })
                .OrderBy(s => s.FullName), "Id", "FullName", Appointment.CreatedBy);
        }

        // Optional: Method to get current user ID
        private int? GetCurrentUserId()
        {
            // Implement based on your authentication system
            // Example:
            // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // if (int.TryParse(userIdClaim, out int userId)) return userId;
            return null;
        }
    }
}