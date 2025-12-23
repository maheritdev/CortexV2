using Cortex.Entities;

namespace Cortex.Services
{
    public class PrescriptionService
    {
        private readonly HospitalManagementSystemContext _context;
        public PrescriptionService(HospitalManagementSystemContext context) => _context = context;

        public async Task<Prescription> CreatePrescription(Prescription prescription)
        {
            var medication = await _context.Medications.FindAsync(prescription.MedicationId);
            if (medication == null) throw new Exception("Medication not found.");

            if (medication.CurrentStock < prescription.Quantity)
                throw new Exception("Insufficient stock.");

            medication.CurrentStock -= prescription.Quantity;

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
            return prescription;
        }
    }
}
