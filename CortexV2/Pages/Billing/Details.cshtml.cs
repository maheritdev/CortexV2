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
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public DetailsModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

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
    }
}
