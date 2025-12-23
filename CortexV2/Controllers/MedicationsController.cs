using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicationsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public MedicationsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all medications
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var meds = await _context.Medications.ToListAsync();
            return Ok(meds);
        }

        // ✅ Get by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var med = await _context.Medications.FindAsync(id);
            if (med == null) return NotFound();
            return Ok(med);
        }

        // ✅ Create new medication
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Medication medication)
        {
            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();
            return Ok(medication);
        }

        // ✅ Update medication
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Medication medication)
        {
            if (id != medication.MedicationId) return BadRequest();

            _context.Entry(medication).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(medication);
        }

        // ✅ Delete medication
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var med = await _context.Medications.FindAsync(id);
            if (med == null) return NotFound();

            _context.Medications.Remove(med);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ✅ Restock (increase stock)
        [HttpPost("{id}/restock")]
        public async Task<IActionResult> Restock(int id, [FromBody] int quantity)
        {
            var med = await _context.Medications.FindAsync(id);
            if (med == null) return NotFound();

            if (quantity <= 0) return BadRequest("Quantity must be greater than zero.");

            med.CurrentStock += quantity;
            _context.Medications.Update(med);
            await _context.SaveChangesAsync();

            return Ok(med);
        }
    }
}
