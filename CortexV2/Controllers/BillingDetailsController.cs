using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingDetailsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public BillingDetailsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all billing details
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var details = await _context.BillingDetails
                .Include(d => d.Bill)
                .ToListAsync();

            return Ok(details);
        }

        // ✅ Get billing detail by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var detail = await _context.BillingDetails
                .Include(d => d.Bill)
                .FirstOrDefaultAsync(d => d.BillingDetailId == id);

            if (detail == null) return NotFound();
            return Ok(detail);
        }

        // ✅ Get billing details by BillingId
        [HttpGet("billing/{billingId}")]
        public async Task<IActionResult> GetByBilling(int billingId)
        {
            var details = await _context.BillingDetails
                .Where(d => d.BillId == billingId)
                .ToListAsync();

            return Ok(details);
        }

        // ✅ Create billing detail (line item)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BillingDetail detail)
        {
            _context.BillingDetails.Add(detail);

            // Update parent billing total
            var billing = await _context.Billings.FindAsync(detail.BillId);
            if (billing != null)
            {
                billing.TotalAmount += detail.Amount;
                _context.Billings.Update(billing);
            }

            await _context.SaveChangesAsync();
            return Ok(detail);
        }

        // ✅ Update billing detail
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BillingDetail detail)
        {
            if (id != detail.BillingDetailId) return BadRequest();

            _context.Entry(detail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(detail);
        }

        // ✅ Delete billing detail
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var detail = await _context.BillingDetails.FindAsync(id);
            if (detail == null) return NotFound();

            // Adjust parent billing total
            var billing = await _context.Billings.FindAsync(detail.BillId);
            if (billing != null)
            {
                billing.TotalAmount -= detail.Amount;
                _context.Billings.Update(billing);
            }

            _context.BillingDetails.Remove(detail);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
