using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public DoctorsController(HospitalManagementSystemContext context) => _context = context;

        /*[HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _context.Doctors.Include(d => d.User).ToListAsync();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _context.Doctors.Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DoctorId == id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return Ok(doctor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Doctor doctor)
        {
            if (id != doctor.DoctorId) return BadRequest();
            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(doctor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return Ok();
        }*/
    }
}
