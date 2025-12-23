using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cortex.Entities;

namespace Cortex.Pages.Department
{
    public class CreateModel : PageModel
    {
        private readonly HospitalManagementSystemContext _context;

        public CreateModel(HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["HeadOfDepartment"] = new SelectList(_context.Staff, "StaffID", "StaffID");
            return Page();
        }

        [BindProperty]
        public Cortex.Entities.Department Department { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Departments.Add(Department);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
