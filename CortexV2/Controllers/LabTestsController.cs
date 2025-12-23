using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabTestsController : Controller
    {
        private readonly HospitalManagementSystemContext _context;
        public LabTestsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all billing details
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var details = await _context.LabTests
                .Include(d => d.LabOrderDetails)
                .ToListAsync();

            return Ok(details);
        }

        // ✅ Get billing detail by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var detail = await _context.LabTests
                .Include(d => d.LabOrderDetails)
                .FirstOrDefaultAsync(d => d.TestId == id);

            if (detail == null) return NotFound();
            return Ok(detail);
        }

        // ✅ Get billing details by BillingId
        [HttpGet("billing/{billingId}")]
        public async Task<IActionResult> GetByBilling(int TestId)
        {
            var details = await _context.LabTests
                .Where(d => d.TestId == TestId)
                .ToListAsync();

            return Ok(details);
        }

        // ✅ Create billing detail (line item)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LabOrderDetail detail)
        {
            _context.LabOrderDetails.Add(detail);

            // Update parent billing total
            var labTests = await _context.LabTests.FindAsync(detail.OrderDetailId);
            if (labTests != null)
            {
                _context.LabTests.Update(labTests);
            }

            await _context.SaveChangesAsync();
            return Ok(detail);
        }

        // ✅ Update billing detail
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LabOrderDetail detail)
        {
            if (id != detail.TestId) return BadRequest();

            _context.Entry(detail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(detail);
        }

        // ✅ Delete billing detail
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var detail = await _context.LabOrders.FindAsync(id);
            if (detail == null) return NotFound();

            // Adjust parent billing total
            var labTest = await _context.LabTests.FindAsync(detail.OrderId);
            if (labTest != null)
            {
                _context.LabTests.Update(labTest);
            }

            _context.LabTests.Remove(labTest);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
