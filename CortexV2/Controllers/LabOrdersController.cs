using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabOrdersController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public LabOrdersController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all lab orders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.LabOrders.ToListAsync();
            return Ok(orders);
        }

        // ✅ Get lab order by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.LabOrders
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {

            var order = await _context.LabOrders.Where(a => a.PatientId == patientId).ToListAsync();

            if (order == null) return NotFound();
            return Ok(order);
        }

        // ✅ Create new lab order
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LabOrder order)
        {
            order.Status = "Pending";
            _context.LabOrders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // ✅ Update lab order (basic info only)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LabOrder order)
        {
            if (id != order.OrderId) return BadRequest();

            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // ✅ Delete lab order
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.LabOrders.FindAsync(id);
            if (order == null) return NotFound();

            _context.LabOrders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ✅ Change status
        [HttpPost("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var order = await _context.LabOrders.FindAsync(id);
            if (order == null) return NotFound();

            var validStatuses = new[] { "Pending", "InProgress", "Completed", "ReportUploaded", "Cancelled" };
            if (!validStatuses.Contains(status))
                return BadRequest($"Invalid status. Allowed: {string.Join(", ", validStatuses)}");

            order.Status = status;
            _context.LabOrders.Update(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        // ✅ Attach a report to the lab order
        [HttpPost("{id}/add-report")]
        public async Task<IActionResult> AddReport(int id, [FromBody] LabOrder report)
        {
            var order = await _context.LabOrders
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            report.OrderId = id;
            _context.LabOrders.Add(report);

            order.Status = "ReportUploaded";
            _context.LabOrders.Update(order);

            await _context.SaveChangesAsync();
            return Ok(new { order, report });
        }
    }
}
