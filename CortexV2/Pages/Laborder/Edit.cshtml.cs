using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Laborder
{
    public class EditModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public EditModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LabOrder LabOrder { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laborder =  await _context.LabOrders.FirstOrDefaultAsync(m => m.OrderId == id);
            if (laborder == null)
            {
                return NotFound();
            }
            LabOrder = laborder;
           ViewData["DoctorId"] = new SelectList(_context.Staff, "StaffID", "StaffID");
           ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(LabOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LabOrderExists(LabOrder.OrderId))
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

        private bool LabOrderExists(int id)
        {
            return _context.LabOrders.Any(e => e.OrderId == id);
        }
    }
}
