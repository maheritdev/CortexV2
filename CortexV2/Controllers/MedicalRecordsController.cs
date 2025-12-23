using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public MedicalRecordsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all medical records
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var records = await _context.MedicalRecords
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Include(r => r.Prescriptions)
                .ToListAsync();

            return Ok(records);
        }

        // ✅ Get medical record by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _context.MedicalRecords
                .Include(r => r.Patient)
                .Include(r => r.Doctor)
                .Include(r => r.Prescriptions)
                .FirstOrDefaultAsync(r => r.RecordId == id);

            if (record == null) return NotFound();
            return Ok(record);
        }

        // ✅ Get all records for a specific patient
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var records = await _context.MedicalRecords
                .Include(r => r.Doctor)
                .Include(r => r.Prescriptions)
                .Where(r => r.PatientId == patientId)
                .ToListAsync();

            return Ok(records);
        }

        // ✅ Create new record
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicalRecord record)
        {
            record.CreatedAt = DateTime.UtcNow;

            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();

            return Ok(record);
        }

        // ✅ Update record
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicalRecord record)
        {
            if (id != record.RecordId) return BadRequest();

            _context.Entry(record).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(record);
        }

        // ✅ Delete record
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null) return NotFound();

            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ✅ Add prescription to record
        [HttpPost("{id}/add-prescription")]
        public async Task<IActionResult> AddPrescription(int id, [FromBody] Prescription prescription)
        {
            var record = await _context.MedicalRecords
                .Include(r => r.Prescriptions)
                .FirstOrDefaultAsync(r => r.RecordId == id);

            if (record == null) return NotFound();

            prescription.RecordId = id;
            prescription.IssuedAt = DateTime.UtcNow;

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok(new { record, prescription });
        }

        // ✅ Add follow-up date
        [HttpPost("{id}/follow-up")]
        public async Task<IActionResult> AddFollowUp(int id, [FromBody] DateTime followUpDate)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null) return NotFound();

            record.FollowUpDate = followUpDate;
            _context.MedicalRecords.Update(record);
            await _context.SaveChangesAsync();

            return Ok(record);
        }
    }
}
