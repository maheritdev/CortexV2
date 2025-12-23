using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public PatientsController(HospitalManagementSystemContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _context.Patients.Include(p => p.User).ToListAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _context.Patients.Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == id);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return Ok(patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Patient patient)
        {
            if (id != patient.PatientId) return BadRequest();
            _context.Entry(patient).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(patient);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _context.Patients.CountAsync();
            return Ok(count);
        }
    }
}
