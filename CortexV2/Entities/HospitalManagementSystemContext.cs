using Cortex.DTOs.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Cortex.Entities;

public partial class HospitalManagementSystemContext : DbContext
{
    public HospitalManagementSystemContext()
    {
    }

    public HospitalManagementSystemContext(DbContextOptions<HospitalManagementSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admission> Admissions { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Billing> Billings { get; set; }

    public virtual DbSet<BillingDetail> BillingDetails { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    public virtual DbSet<LabOrder> LabOrders { get; set; }

    public virtual DbSet<LabOrderDetail> LabOrderDetails { get; set; }

    public virtual DbSet<LabTest> LabTests { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

    public virtual DbSet<Medication> Medications { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<PrescriptionDetail> PrescriptionDetails { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=HospitalManagementSystem;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admission>(entity =>
        {
            entity.HasKey(e => e.AdmissionId).HasName("PK__Admissio__C97EEFA2F948E51E");

            entity.Property(e => e.AdmissionId).HasColumnName("AdmissionID");
            entity.Property(e => e.AdmissionDate).HasColumnType("datetime");
            entity.Property(e => e.AssignedDoctorId).HasColumnName("AssignedDoctorID");
            entity.Property(e => e.Diagnosis).HasMaxLength(500);
            entity.Property(e => e.DischargeDate).HasColumnType("datetime");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Admitted");

            entity.HasOne(d => d.AssignedDoctor).WithMany(p => p.Admissions)
                .HasForeignKey(d => d.AssignedDoctorId)
                .HasConstraintName("FK__Admission__Assig__08B54D69");

            entity.HasOne(d => d.Patient).WithMany(p => p.Admissions)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Admission__Patie__06CD04F7");

            entity.HasOne(d => d.Room).WithMany(p => p.Admissions)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__Admission__RoomI__07C12930");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA2730D8BF5");

            entity.HasIndex(e => e.AppointmentDate, "IX_Appointments_Date");

            entity.HasIndex(e => e.DoctorId, "IX_Appointments_DoctorID");

            entity.HasIndex(e => e.PatientId, "IX_Appointments_PatientID");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Scheduled");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AppointmentCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Appointme__Creat__4F7CD00D");

            entity.HasOne(d => d.Doctor).WithMany(p => p.AppointmentDoctors)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Docto__4BAC3F29");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Patie__4AB81AF0");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__AuditLog__5E5499A85ED3AB23");

            entity.ToTable("AuditLog");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Action).HasMaxLength(20);
            entity.Property(e => e.ChangeDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.RecordId)
                .HasMaxLength(100)
                .HasColumnName("RecordID");
            entity.Property(e => e.TableName).HasMaxLength(100);

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("FK__AuditLog__Change__208CD6FA");
        });

        modelBuilder.Entity<Billing>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__Billing__11F2FC4AEC8E8738");

            entity.ToTable("Billing");

            entity.HasIndex(e => e.PatientId, "IX_Billing_PatientID");

            entity.HasIndex(e => e.Status, "IX_Billing_Status");

            entity.Property(e => e.BillId).HasColumnName("BillID");
            entity.Property(e => e.Balance).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.BillDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.InsuranceClaimAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PaidAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Billings)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Billing__Created__72C60C4A");

            entity.HasOne(d => d.Patient).WithMany(p => p.Billings)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Billing__Patient__6D0D32F4");
        });

        modelBuilder.Entity<BillingDetail>(entity =>
        {
            entity.HasKey(e => e.BillingDetailId).HasName("PK__BillingD__77F5CCC45CF23523");

            entity.Property(e => e.BillingDetailId).HasColumnName("BillingDetailID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillId).HasColumnName("BillID");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ItemType).HasMaxLength(50);
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.TaxAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Bill).WithMany(p => p.BillingDetails)
                .HasForeignKey(d => d.BillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BillingDe__BillI__75A278F5");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BCD09D800E9");

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Location).HasMaxLength(100);

            entity.HasOne(d => d.HeadOfDepartmentNavigation).WithMany(p => p.Departments)
                .HasForeignKey(d => d.HeadOfDepartment)
                .HasConstraintName("FK_Departments_Staff");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__F5FDE6D34B970D0E");

            entity.ToTable("Inventory");

            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ItemName).HasMaxLength(100);
            entity.Property(e => e.LastRestockedDate).HasColumnType("datetime");
            entity.Property(e => e.QuantityInStock).HasDefaultValue(0);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.Supplier).HasMaxLength(100);
            entity.Property(e => e.UnitOfMeasure).HasMaxLength(20);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Inventor__55433A4B4447FBB2");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TransactionType).HasMaxLength(20);

            entity.HasOne(d => d.Inventory).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.InventoryId)
                .HasConstraintName("FK__Inventory__Inven__6754599E");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.PerformedBy)
                .HasConstraintName("FK__Inventory__Perfo__6A30C649");
        });

        modelBuilder.Entity<LabOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__LabOrder__C3905BAF1CB478BD");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Doctor).WithMany(p => p.LabOrders)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LabOrders__Docto__123EB7A3");

            entity.HasOne(d => d.Patient).WithMany(p => p.LabOrders)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LabOrders__Patie__114A936A");
        });

        modelBuilder.Entity<LabOrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__LabOrder__D3B9D30C35F2E1D4");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ResultDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TestId).HasColumnName("TestID");

            entity.HasOne(d => d.Order).WithMany(p => p.LabOrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LabOrderD__Order__17F790F9");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.LabOrderDetailPerformedByNavigations)
                .HasForeignKey(d => d.PerformedBy)
                .HasConstraintName("FK__LabOrderD__Perfo__19DFD96B");

            entity.HasOne(d => d.Test).WithMany(p => p.LabOrderDetails)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LabOrderD__TestI__18EBB532");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.LabOrderDetailVerifiedByNavigations)
                .HasForeignKey(d => d.VerifiedBy)
                .HasConstraintName("FK__LabOrderD__Verif__1AD3FDA4");
        });

        modelBuilder.Entity<LabTest>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("PK__LabTests__8CC331008BCEA6E1");

            entity.Property(e => e.TestId).HasColumnName("TestID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Duration).HasMaxLength(50);
            entity.Property(e => e.NormalRange).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SampleType).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.TestName).HasMaxLength(100);
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__MedicalR__FBDF78C978B75F53");

            entity.HasIndex(e => e.PatientId, "IX_MedicalRecords_PatientID");

            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.FollowUpDate).HasColumnType("datetime");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.RecordedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.VisitDate).HasColumnType("datetime");

            entity.HasOne(d => d.Doctor).WithMany(p => p.MedicalRecordDoctors)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MedicalRe__Docto__534D60F1");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalRecords)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MedicalRe__Patie__52593CB8");

            entity.HasOne(d => d.RecordedByNavigation).WithMany(p => p.MedicalRecordRecordedByNavigations)
                .HasForeignKey(d => d.RecordedBy)
                .HasConstraintName("FK__MedicalRe__Recor__5441852A");
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(e => e.MedicationId).HasName("PK__Medicati__62EC1ADA8D0543C9");

            entity.Property(e => e.MedicationId).HasColumnName("MedicationID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CurrentStock).HasDefaultValue(0);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.GenericName).HasMaxLength(100);
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UnitType).HasMaxLength(20);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC346976B13CE");

            entity.HasIndex(e => e.ContactNumber, "IX_Patients_ContactNumber");

            entity.HasIndex(e => e.LastName, "IX_Patients_LastName");

            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.AlternateContact).HasMaxLength(20);
            entity.Property(e => e.BloodType).HasMaxLength(10);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EmergencyContactName).HasMaxLength(100);
            entity.Property(e => e.EmergencyContactNumber).HasMaxLength(20);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.InsurancePolicyNumber).HasMaxLength(50);
            entity.Property(e => e.InsuranceProvider).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LastVisitDate).HasColumnType("datetime");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.ZipCode).HasMaxLength(20);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A582A23FA90");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.BillId).HasColumnName("BillID");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.TransactionReference).HasMaxLength(100);

            entity.HasOne(d => d.Bill).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BillId)
                .HasConstraintName("FK__Payments__BillID__7B5B524B");

            entity.HasOne(d => d.Patient).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Patien__7C4F7684");

            entity.HasOne(d => d.ReceivedByNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ReceivedBy)
                .HasConstraintName("FK__Payments__Receiv__7F2BE32F");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId).HasName("PK__Prescrip__40130812C4ABE836");

            entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.PrescriptionDate).HasColumnType("datetime");
            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__Docto__59FA5E80");

            entity.HasOne(d => d.Patient).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__Patie__59063A47");

            entity.HasOne(d => d.Record).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.RecordId)
                .HasConstraintName("FK__Prescript__Recor__5812160E");
        });

        modelBuilder.Entity<PrescriptionDetail>(entity =>
        {
            entity.HasKey(e => e.PrescriptionDetailId).HasName("PK__Prescrip__6DB7668A02AE4E01");

            entity.Property(e => e.PrescriptionDetailId).HasColumnName("PrescriptionDetailID");
            entity.Property(e => e.Dosage).HasMaxLength(100);
            entity.Property(e => e.Duration).HasMaxLength(100);
            entity.Property(e => e.Frequency).HasMaxLength(100);
            entity.Property(e => e.Instructions).HasMaxLength(500);
            entity.Property(e => e.MedicationId).HasColumnName("MedicationID");
            entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");

            entity.HasOne(d => d.Medication).WithMany(p => p.PrescriptionDetails)
                .HasForeignKey(d => d.MedicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__Medic__5FB337D6");

            entity.HasOne(d => d.Prescription).WithMany(p => p.PrescriptionDetails)
                .HasForeignKey(d => d.PrescriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__Presc__5EBF139D");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Rooms__32863919592FEE03");

            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.RatePerDay).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RoomNumber).HasMaxLength(20);
            entity.Property(e => e.RoomType).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Available");

            entity.HasOne(d => d.Department).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Rooms__Departmen__02084FDA");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffID).HasName("PK__Staff__96D4AAF70B661DD8");

            entity.HasIndex(e => e.DepartmentId, "IX_Staff_DepartmentID");

            entity.Property(e => e.StaffID).HasColumnName("StaffID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Qualification).HasMaxLength(100);
            entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Specialization).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.Username).HasMaxLength(50);

            /*entity.HasOne(d => d.Department).WithMany(p => p.Staff)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Staff__Departmen__3A81B327");*/
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
