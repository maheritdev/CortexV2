using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public RoomsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all rooms
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Department)
                .ToListAsync();

            return Ok(rooms);
        }

        // ✅ Get room by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Department)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null) return NotFound();
            return Ok(room);
        }

        // ✅ Create room
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Room room)
        {
            room.Status = "Available"; // default
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return Ok(room);
        }

        // ✅ Update room
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Room room)
        {
            if (id != room.RoomId) return BadRequest();

            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(room);
        }

        // ✅ Delete room
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ✅ Change room status
        [HttpPost("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] string status)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            var validStatuses = new[] { "Available", "Occupied", "Maintenance", "Reserved" };
            if (!validStatuses.Contains(status))
                return BadRequest($"Invalid status. Allowed: {string.Join(", ", validStatuses)}");

            room.Status = status;
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        // ✅ Assign room to department
        [HttpPost("{id}/assign-department/{departmentId}")]
        public async Task<IActionResult> AssignDepartment(int id, int departmentId)
        {
            var room = await _context.Rooms.FindAsync(id);
            var dept = await _context.Departments.FindAsync(departmentId);

            if (room == null || dept == null) return NotFound("Room or Department not found");

            room.DepartmentId = departmentId;
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _context.Rooms.CountAsync();
            return Ok(count);
        }
    }
}
