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
    public class IndexModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public IndexModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public IList<Cortex.Entities.Billing> Billing { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Billing = await _context.Billings
                .Include(b => b.CreatedByNavigation)
                .Include(b => b.Patient).ToListAsync();
        }
    }
}
