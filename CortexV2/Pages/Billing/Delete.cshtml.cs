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
    public class DeleteModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public DeleteModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cortex.Entities.Billing Billing { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billing = await _context.Billings.FirstOrDefaultAsync(m => m.BillId == id);

            if (billing == null)
            {
                return NotFound();
            }
            else
            {
                Billing = billing;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billing = await _context.Billings.FindAsync(id);
            if (billing != null)
            {
                Billing = billing;
                _context.Billings.Remove(Billing);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
