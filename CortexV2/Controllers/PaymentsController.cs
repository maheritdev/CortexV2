using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public PaymentsController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all payments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _context.Payments
                .Include(p => p.Bill)
                .Include(p => p.Patient)
                .Include(p => p.ReceivedByNavigation)
                .ToListAsync();

            return Ok(payments);
        }

        // ✅ Get payment by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Bill)
                .Include(p => p.Patient)
                .Include(p => p.ReceivedByNavigation)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null) return NotFound();
            return Ok(payment);
        }

        // ✅ Get payments by patient
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var payments = await _context.Payments
                .Where(p => p.PatientId == patientId)
                .ToListAsync();

            return Ok(payments);
        }

        // ✅ Create a payment (updates billing automatically)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Payment payment)
        {
            var billing = await _context.Billings.FindAsync(payment.BillId);
            if (billing == null) return BadRequest("Invalid Bill ID");

            // Update billing amounts
            billing.PaidAmount += payment.Amount;
            billing.Balance = billing.TotalAmount - billing.PaidAmount;

            // Update billing status
            if (billing.PaidAmount == 0)
                billing.Status = "Pending";
            else if (billing.PaidAmount < billing.TotalAmount)
                billing.Status = "Partial";
            else
                billing.Status = "Paid";

            _context.Payments.Add(payment);
            _context.Billings.Update(billing);
            await _context.SaveChangesAsync();

            return Ok(payment);
        }

        // ✅ Update payment (rarely done, but possible)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Payment payment)
        {
            if (id != payment.PaymentId) return BadRequest();

            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(payment);
        }

        // ✅ Delete payment (reverts billing amounts)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            var billing = await _context.Billings.FindAsync(payment.BillId);
            if (billing != null)
            {
                billing.PaidAmount -= payment.Amount;
                billing.Balance = billing.TotalAmount - billing.PaidAmount;

                if (billing.PaidAmount == 0)
                    billing.Status = "Pending";
                else if (billing.PaidAmount < billing.TotalAmount)
                    billing.Status = "Partial";
                else
                    billing.Status = "Paid";

                _context.Billings.Update(billing);
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
