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
    public class IndexModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public IndexModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public IList<LabOrder> LabOrder { get;set; } = default!;

        public async Task OnGetAsync()
        {
            LabOrder = await _context.LabOrders
                .Include(l => l.Doctor)
                .Include(l => l.Patient).ToListAsync();
        }
    }
}
