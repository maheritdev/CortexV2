using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabOrderDetailsController : Controller
    {
        private readonly HospitalManagementSystemContext _context;
        public LabOrderDetailsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all inventory items
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.LabOrderDetails.ToListAsync();
            return Ok(items);
        }

        // ✅ Get inventory item by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.LabOrderDetails.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // ✅ Add new inventory item
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LabOrderDetail item)
        {
            item.LastUpdated = DateTime.UtcNow;
            _context.LabOrderDetails.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        // ✅ Update inventory item
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LabOrderDetail item)
        {
            if (id != item.OrderId) return BadRequest();

            item.LastUpdated = DateTime.UtcNow;
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        // ✅ Delete inventory item
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.LabOrderDetails.FindAsync(id);
            if (item == null) return NotFound();

            _context.LabOrderDetails.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ✅ Restock inventory item
        [HttpPost("{id}/restock")]
        public async Task<IActionResult> Restock(int id, [FromBody] int quantity)
        {
            var item = await _context.LabOrderDetails.FindAsync(id);
            if (item == null) return NotFound();

            if (quantity <= 0) return BadRequest("Quantity must be greater than zero");


            _context.LabOrderDetails.Update(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        // ✅ Issue (consume) inventory item
        [HttpPost("{id}/issue")]
        public async Task<IActionResult> Issue(int id, [FromBody] int quantity)
        {
            var item = await _context.Inventories.FindAsync(id);
            if (item == null) return NotFound();

            if (quantity <= 0) return BadRequest("Quantity must be greater than zero");
            if (item.QuantityInStock < quantity) return BadRequest("Insufficient stock");

            item.QuantityInStock -= quantity;
            item.LastRestockedDate = DateTime.UtcNow;

            _context.Inventories.Update(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }
    }
}
