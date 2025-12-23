using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryTransactionsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public InventoryTransactionsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all transactions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var txs = await _context.InventoryTransactions
                .Include(t => t.Inventory)
                .Include(t => t.Staff)
                .ToListAsync();

            return Ok(txs);
        }

        // ✅ Get transaction by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tx = await _context.InventoryTransactions
                .Include(t => t.Inventory)
                .Include(t => t.Staff)
                .FirstOrDefaultAsync(t => t.TransactionId == id);

            if (tx == null) return NotFound();
            return Ok(tx);
        }

        // ✅ Log a transaction manually (Admin only usually)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InventoryTransaction tx)
        {
            tx.TransactionDate = DateTime.UtcNow;

            _context.InventoryTransactions.Add(tx);
            await _context.SaveChangesAsync();
            return Ok(tx);
        }

        // ✅ Delete a transaction (not recommended in production)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tx = await _context.InventoryTransactions.FindAsync(id);
            if (tx == null) return NotFound();

            _context.InventoryTransactions.Remove(tx);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ✅ Get transactions by Inventory Item
        [HttpGet("item/{itemId}")]
        public async Task<IActionResult> GetByItem(int itemId)
        {
            var txs = await _context.InventoryTransactions
                .Where(t => t.InventoryId == itemId)
                .Include(t => t.Staff)
                .ToListAsync();

            return Ok(txs);
        }

        // ✅ Get transactions by Staff member
        [HttpGet("staff/{staffId}")]
        public async Task<IActionResult> GetByStaff(int staffId)
        {
            var txs = await _context.InventoryTransactions
                .Where(t => t.StaffId == staffId)
                .Include(t => t.Inventory)
                .ToListAsync();

            return Ok(txs);
        }
    }
}
