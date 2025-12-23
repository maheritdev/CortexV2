using Cortex.Viewmodel;
using Cortex.Entities;
using Cortex.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public AppointmentsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all appointments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();
            if(list.Any()) 
                 return Ok(list);
            return BadRequest();
        }

        // ✅ Get by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null) return NotFound();
            return Ok(appointment);
        }

        // ✅ Create with conflict detection
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Appointment appointment)
        {
            // Conflict check: doctor double booking
            bool conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == appointment.DoctorId &&
                a.AppointmentDate == appointment.AppointmentDate &&
                a.Status == "Scheduled");

            if (conflict)
                return BadRequest("Doctor already has an appointment at this time.");

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return Ok(appointment);
        }

        // ✅ Update (check conflict again)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Appointment appointment)
        {
            if (id != appointment.AppointmentId) return BadRequest();

            bool conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == appointment.DoctorId &&
                a.AppointmentDate == appointment.AppointmentDate &&
                a.AppointmentId != appointment.AppointmentId &&
                a.Status == "Scheduled");

            if (conflict)
                return BadRequest("Doctor already has another appointment at this time.");

            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(appointment);
        }

        // ✅ Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ✅ Extra: Get appointments by Doctor
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctor(int doctorId)
        {
            var list = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Patient)
                .ToListAsync();
            return Ok(list);
        }

        // ✅ Extra: Get appointments by Patient
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var list = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _context.Appointments.CountAsync();
            return Ok(count);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent()
        {
            var recent = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .Select(a => new RecentAppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    Patient = (a.Patient != null) ? (a.Patient.FirstName + " " + a.Patient.LastName) : "N/A",
                    Doctor = (a.Doctor != null) ? (a.Doctor.FirstName + " " + a.Doctor.LastName) : "N/A",
                    Time = a.AppointmentDate,
                    Status = a.Status
                })
                .ToListAsync();
            Console.WriteLine($"PP - {recent}");

            return Ok(recent);
        }
    }
}
