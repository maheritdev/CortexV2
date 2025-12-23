using Cortex.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Staff,Doctor")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdmissionsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public AdmissionsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all admissions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var admissions = await _context.Admissions
                .Include(a => a.Patient)
                .Include(a => a.Room)
                .ToListAsync();

            return Ok(admissions);
        }

        // ✅ Get admission by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var admission = await _context.Admissions
                .Include(a => a.Patient)
                .Include(a => a.Room)
                .FirstOrDefaultAsync(a => a.AdmissionId == id);

            if (admission == null) return NotFound();
            return Ok(admission);
        }

        // ✅ Admit a patient into a room
        [HttpPost("admit")]
        public async Task<IActionResult> Admit([FromBody] Admission admission)
        {
            var patient = await _context.Patients.FindAsync(admission.PatientId);
            var room = await _context.Rooms.FindAsync(admission.RoomId);

            if (patient == null || room == null)
                return NotFound("Patient or Room not found");

            if (room.Status != "Available")
                return BadRequest("Room is not available");

            admission.AdmissionDate = DateTime.UtcNow;
            admission.DischargeDate = null;

            _context.Admissions.Add(admission);

            // Update room status
            room.Status = "Occupied";
            _context.Rooms.Update(room);

            await _context.SaveChangesAsync();
            return Ok(admission);
        }

        // ✅ Discharge a patient
        [HttpPost("{id}/discharge")]
        public async Task<IActionResult> Discharge(int id)
        {
            var admission = await _context.Admissions
                .Include(a => a.Room)
                .FirstOrDefaultAsync(a => a.AdmissionId == id);

            if (admission == null) return NotFound("Admission not found");
            if (admission.DischargeDate != null) return BadRequest("Patient already discharged");

            admission.DischargeDate = DateTime.UtcNow;

            // Free up room
            if (admission.Room != null)
            {
                admission.Room.Status = "Available";
                _context.Rooms.Update(admission.Room);
            }

            _context.Admissions.Update(admission);
            await _context.SaveChangesAsync();

            return Ok(admission);
        }

        // ✅ Update admission (for corrections)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Admission admission)
        {
            if (id != admission.AdmissionId) return BadRequest();

            _context.Entry(admission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(admission);
        }

        // ✅ Delete admission record (admin only usually)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var admission = await _context.Admissions.FindAsync(id);
            if (admission == null) return NotFound();

            _context.Admissions.Remove(admission);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
