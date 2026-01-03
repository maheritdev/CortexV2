using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Appointment
{
    public class DetailsModel : PageModel
    {
        private readonly HospitalManagementSystemContext _context;

        public DetailsModel(HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public Cortex.Entities.Appointment? Appointment { get; set; } = default!;

        // Display properties
        public string PatientName { get; set; } = "Unknown Patient";
        public string PatientAge { get; set; } = "N/A";
        public string PatientGender { get; set; } = "N/A";
        public string PatientPhone { get; set; } = "N/A";

        public string DoctorName { get; set; } = "Unknown Doctor";
        public string DoctorSpecialization { get; set; } = "N/A";
        public string DoctorDepartment { get; set; } = "N/A";
        public string DoctorPhone { get; set; } = "N/A";

        public string CreatedByName { get; set; } = "Unknown";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load appointment with all related data
            Appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (Appointment == null)
            {
                return NotFound();
            }

            // Set patient information
            if (Appointment.Patient != null)
            {
                PatientName = $"{Appointment.Patient.FirstName} {Appointment.Patient.LastName}";
                PatientAge = CalculateAge(Appointment.Patient.DateOfBirth);
                PatientGender = Appointment.Patient.Gender ?? "N/A";
                PatientPhone = Appointment.Patient.EmergencyContactNumber ?? "N/A";
            }

            // Set doctor information
            if (Appointment.Doctor != null)
            {
                DoctorName = $"{Appointment.Doctor.FirstName} {Appointment.Doctor.LastName}";
                DoctorSpecialization = Appointment.Doctor.Specialization ?? "N/A";
                DoctorDepartment = Appointment?.Doctor?.Department?.DepartmentName ?? "N/A";
                DoctorPhone = Appointment.Doctor.ContactNumber ?? "N/A";
            }

            // Set creator information
            if (Appointment.CreatedByNavigation != null)
            {
                CreatedByName = $"{Appointment.CreatedByNavigation.FirstName} {Appointment.CreatedByNavigation.LastName}";
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