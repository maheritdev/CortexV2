using Cortex.Entities;
using static System.Net.WebRequestMethods;

namespace Cortex.Services
{
    public class BillingService
    {
        private readonly HospitalManagementSystemContext _context;
        private readonly HttpClient _http;
        public BillingService(HospitalManagementSystemContext context, HttpClient http)
        {
            _context = context;
            _http = http;
        }

        public async Task<Billing> CreateBill(int patientId, decimal amount)
        {
            var bill = new Billing
            {
                PatientId = patientId,
                PaidAmount = amount,
                isPaid = false,
                DueDate = DateTime.Now
            };
            _context.Billings.Add(bill);
            await _context.SaveChangesAsync();
            return bill;
        }

        public async Task<Billing> PayBill(int billingId)
        {
            var bill = await _context.Billings.FindAsync(billingId);
            if (bill == null) throw new Exception("Bill not found.");
            if (bill.isPaid == true) throw new Exception("Bill already paid.");

            bill.isPaid = true;
            bill.PaidDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return bill;
        }

        public async Task<int> GetPendingBillingCountAsync()
        {
            return await _http.GetFromJsonAsync<int>("api/Billing/count");
        }
    }
}
