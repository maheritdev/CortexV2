using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Staff
{
    public class IndexModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public IndexModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public IList<Cortex.Entities.Staff> Staff { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Staff = await _context.Staff
                .Include(s => s.Department).ToListAsync();
        }
    }
}
