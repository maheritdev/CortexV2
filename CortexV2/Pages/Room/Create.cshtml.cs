using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cortex.Entities;

namespace Cortex.Pages.Room
{
    public class CreateModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public CreateModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId");
            return Page();
        }

        [BindProperty]
        public Cortex.Entities.Room Room { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Rooms.Add(Room);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
