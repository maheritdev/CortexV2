using Cortex.Entities;

namespace Cortex.DTOs.Users
{
    public class BillingDto
    {

        public int BillId { get; set; }

        public int PatientId { get; set; }

        public DateTime? BillDate { get; set; }

        public DateTime? DueDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal? PaidAmount { get; set; }

        public decimal? Balance { get; set; }

        public bool? isPaid { get; set; }
        public string Status { get; set; }

        public decimal? InsuranceClaimAmount { get; set; }

        public string? Notes { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? PaidDate { get; set; }


        public virtual ICollection<BillingDetail> BillingDetails { get; set; } = new List<BillingDetail>();

        public virtual Staff? Staff { get; set; }

        public virtual Patient Patient { get; set; } = null!;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
