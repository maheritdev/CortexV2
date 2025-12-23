using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cortex.Entities;

namespace Cortex.Pages.Appointment
{
    public class CreateModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public CreateModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CreatedBy"] = new SelectList(_context.Staff, "StaffID", "StaffID");
        ViewData["DoctorId"] = new SelectList(_context.Staff, "StaffID", "StaffID");
        ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId");
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
