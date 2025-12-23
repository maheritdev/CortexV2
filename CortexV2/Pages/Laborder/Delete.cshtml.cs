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
    public class DeleteModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public DeleteModel(Cortex.Entities.HospitalManagementSystemContext context)
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

            var laborder = await _context.LabOrders.FirstOrDefaultAsync(m => m.OrderId == id);

            if (laborder == null)
            {
                return NotFound();
            }
            else
            {
                LabOrder = laborder;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laborder = await _context.LabOrders.FindAsync(id);
            if (laborder != null)
            {
                LabOrder = laborder;
                _context.LabOrders.Remove(LabOrder);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
