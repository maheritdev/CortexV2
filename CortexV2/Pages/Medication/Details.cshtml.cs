using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Medication
{
    public class DetailsModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public DetailsModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public Cortex.Entities.Medication Medication { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medications.FirstOrDefaultAsync(m => m.MedicationId == id);
            if (medication == null)
            {
                return NotFound();
            }
            else
            {
                Medication = medication;
            }
            return Page();
        }
    }
}
