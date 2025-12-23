using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Inventory
{
    public class DeleteModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public DeleteModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cortex.Entities.Inventory Inventory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories.FirstOrDefaultAsync(m => m.InventoryId == id);

            if (inventory == null)
            {
                return NotFound();
            }
            else
            {
                Inventory = inventory;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory != null)
            {
                Inventory = inventory;
                _context.Inventories.Remove(Inventory);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
