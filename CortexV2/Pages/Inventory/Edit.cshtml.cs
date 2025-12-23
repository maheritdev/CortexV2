using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cortex.Entities;

namespace Cortex.Pages.Inventory
{
    public class EditModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public EditModel(Cortex.Entities.HospitalManagementSystemContext context)
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

            var inventory =  await _context.Inventories.FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventory == null)
            {
                return NotFound();
            }
            Inventory = inventory;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Inventory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryExists(Inventory.InventoryId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventories.Any(e => e.InventoryId == id);
        }
    }
}
