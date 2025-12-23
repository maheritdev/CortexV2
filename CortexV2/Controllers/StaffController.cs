using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public StaffController(HospitalManagementSystemContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var staff = await _context.Staff.Include(s => s.User).ToListAsync();
            return Ok(staff);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var staff = await _context.Staff.Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StaffID == id);
            if (staff == null) return NotFound();
            return Ok(staff);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Staff staff)
        {
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
            return Ok(staff);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Staff staff)
        {
            if (id != staff.StaffID) return BadRequest();
            _context.Entry(staff).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(staff);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return NotFound();
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
