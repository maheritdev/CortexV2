using Cortex.Entities;
using Cortex.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public PrescriptionsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all prescriptions with details
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionDetails)
                    .ThenInclude(d => d.Medication)
                .ToListAsync();

            return Ok(list);
        }

        // ✅ Get by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionDetails)
                    .ThenInclude(d => d.Medication)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null) return NotFound();
            return Ok(prescription);
        }

        // ✅ Create new prescription with stock deduction
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Prescription prescription)
        {
            // Deduct stock for each medication
            foreach (var detail in prescription.PrescriptionDetails)
            {
                var med = await _context.Medications.FindAsync(detail.MedicationId);
                if (med == null) return BadRequest($"Medication {detail.MedicationId} not found.");

                if (med.CurrentStock < detail.Quantity)
                    return BadRequest($"Not enough stock for {med.Name}. Available: {med.CurrentStock}, required: {detail.Quantity}");

                med.CurrentStock -= detail.Quantity;
                _context.Medications.Update(med);
            }

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
            return Ok(prescription);
        }

        // ✅ Update (stock is NOT automatically changed here, only prescription details)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Prescription prescription)
        {
            if (id != prescription.PrescriptionId) return BadRequest();

            _context.Entry(prescription).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(prescription);
        }

        // ✅ Cancel prescription (restore stock)
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionDetails)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null) return NotFound();

            if (prescription.Status == "Cancelled")
                return BadRequest("Prescription already cancelled.");

            // restore stock
            foreach (var detail in prescription.PrescriptionDetails)
            {
                var med = await _context.Medications.FindAsync(detail.MedicationId);
                if (med != null)
                {
                    med.CurrentStock += detail.Quantity;
                    _context.Medications.Update(med);
                }
            }

            prescription.Status = "Cancelled";
            _context.Prescriptions.Update(prescription);
            await _context.SaveChangesAsync();

            return Ok(prescription);
        }

        // ✅ Delete (no stock restore — use Cancel instead if you need rollback)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionDetails)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null) return NotFound();

            _context.PrescriptionDetails.RemoveRange(prescription.PrescriptionDetails);
            _context.Prescriptions.Remove(prescription);

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
