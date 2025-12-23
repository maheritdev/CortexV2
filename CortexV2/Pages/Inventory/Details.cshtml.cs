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
    public class DetailsModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public DetailsModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

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
    }
}
