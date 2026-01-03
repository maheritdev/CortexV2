using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cortex.Pages.Appointment
{
    public class CreateModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public CreateModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }
        public SelectList Patients { get; set; }
        public SelectList Doctors { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
        ViewData["CreatedBy"] = new SelectList(_context.Staff, "StaffID", "StaffID");
        ViewData["DoctorId"] = new SelectList(_context.Staff, "StaffID", "StaffID");
        ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId");
            // Get patients with full name
            var patients = await _context.Patients
                .Select(p => new
                {
                    Id = p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .OrderBy(p => p.FullName)
                .ToListAsync();

            Patients = new SelectList(patients, "Id", "FullName");

            // Get doctors with full name
            var doctors = await _context.Staff
                .Where(s => s.Role == "Doctor")
                .Select(s => new
                {
                    Id = s.StaffID,
                    FullName = s.FirstName + " " + s.LastName
                })
                .OrderBy(s => s.FullName)
                .ToListAsync();

            Doctors = new SelectList(doctors, "Id", "FullName");

            // ... rest of your code
            return Page();

        }

        [BindProperty]
        public Cortex.Entities.Appointment Appointment { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Appointments.Add(Appointment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
