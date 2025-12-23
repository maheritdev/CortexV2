using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public DepartmentsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all departments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _context.Departments
                .Include(d => d.HeadOfDepartmentNavigation) // Assuming navigation property
                .ToListAsync();

            return Ok(departments);
        }

        // ✅ Get department by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dept = await _context.Departments
                .Include(d => d.HeadOfDepartmentNavigation)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (dept == null) return NotFound();
            return Ok(dept);
        }

        // ✅ Create department
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return Ok(department);
        }

        // ✅ Update department
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Department department)
        {
            if (id != department.DepartmentId) return BadRequest();

            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(department);
        }

        // ✅ Delete department
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return NotFound();

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ✅ Assign head of department
        [HttpPost("{id}/assign-head/{staffId}")]
        public async Task<IActionResult> AssignHead(int id, int staffId)
        {
            var dept = await _context.Departments.FindAsync(id);
            var staff = await _context.Staff.FindAsync(staffId);

            if (dept == null || staff == null) return NotFound("Department or Staff not found");

            dept.HeadOfDepartment = staffId;
            _context.Departments.Update(dept);
            await _context.SaveChangesAsync();

            return Ok(dept);
        }
    }
}
