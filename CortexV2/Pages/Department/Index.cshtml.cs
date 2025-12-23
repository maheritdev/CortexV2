using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Department
{
    public class IndexModel : PageModel
    {
        private readonly HospitalManagementSystemContext _context;

        public IndexModel(HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public IList<Cortex.Entities.Department> Department { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Department = await _context.Departments
                .Include(d => d.HeadOfDepartmentNavigation).ToListAsync();
        }
    }
}
