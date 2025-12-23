using Cortex.Entities;
using Cortex.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Cortex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly HospitalManagementSystemContext _context;
        public BillingController(HospitalManagementSystemContext context) => _context = context;

        // ✅ Get all bills
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bills = await _context.Billings
                .Include(b => b.Patient)
                .Include(b => b.BillingDetails)
                .Include(b => b.Payments)
                .ToListAsync();

            return Ok(bills);
        }

        // ✅ Get bill by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bill = await _context.Billings
                .Include(b => b.Patient)
                .Include(b => b.BillingDetails)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BillId == id);

            if (bill == null) return NotFound();
            return Ok(bill);
        }

        // ✅ Create new bill
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Billing bill)
        {
            bill.Balance = bill.TotalAmount - bill.PaidAmount - bill.InsuranceClaimAmount;
            bill.Status = bill.Balance <= 0 ? "Paid" : "Pending";

            _context.Billings.Add(bill);
            await _context.SaveChangesAsync();
            return Ok(bill);
        }

        // ✅ Update bill (recalculate balance & status)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Billing bill)
        {
            if (id != bill.BillId) return BadRequest();

            bill.Balance = bill.TotalAmount - bill.PaidAmount - bill.InsuranceClaimAmount;
            bill.Status = bill.Balance <= 0 ? "Paid" : bill.PaidAmount > 0 ? "Partial" : "Pending";

            _context.Entry(bill).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(bill);
        }

        // ✅ Delete bill
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bill = await _context.Billings.FindAsync(id);
            if (bill == null) return NotFound();

            _context.Billings.Remove(bill);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ✅ Apply a payment to a bill
        [HttpPost("{id}/pay")]
        public async Task<IActionResult> Pay(int id, [FromBody] Payment payment)
        {
            var bill = await _context.Billings.FindAsync(id);
            if (bill == null) return NotFound("Bill not found.");

            if (payment.Amount <= 0)
                return BadRequest("Payment amount must be positive.");

            payment.BillId = id;
            payment.PatientId = bill.PatientId;

            bill.PaidAmount += payment.Amount;
            bill.Balance = bill.TotalAmount - bill.PaidAmount - bill.InsuranceClaimAmount;

            if (bill.Balance <= 0)
                bill.Status = "Paid";
            else if (bill.PaidAmount > 0)
                bill.Status = "Partial";
            else
                bill.Status = "Pending";

            _context.Payments.Add(payment);
            _context.Billings.Update(bill);
            await _context.SaveChangesAsync();

            return Ok(new { bill, payment });
        }

        // ✅ Apply insurance claim
        [HttpPost("{id}/insurance")]
        public async Task<IActionResult> ApplyInsurance(int id, [FromBody] decimal claimAmount)
        {
            var bill = await _context.Billings.FindAsync(id);
            if (bill == null) return NotFound("Bill not found.");

            if (claimAmount <= 0) return BadRequest("Invalid insurance claim amount.");

            bill.InsuranceClaimAmount += claimAmount;
            bill.Balance = bill.TotalAmount - bill.PaidAmount - bill.InsuranceClaimAmount;

            if (bill.Balance <= 0)
                bill.Status = "Paid";
            else if (bill.PaidAmount > 0 || bill.InsuranceClaimAmount > 0)
                bill.Status = "Partial";

            _context.Billings.Update(bill);
            await _context.SaveChangesAsync();

            return Ok(bill);
        }

        // ✅ Extra: Get appointments by Patient
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var list = await _context.Billings
                .Where(a => a.PatientId == patientId)
                .Include(a => a.BillingDetails)
                .ToListAsync();
            return Ok(list);
        }


        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _context.Billings.CountAsync(obj => obj.Status == "Pending");
            return Ok(count);
        }
    }
}
