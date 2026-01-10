using Cortex.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cortex.Pages.Laborder
{
    public class CreateModel : PageModel
    {
        private readonly Cortex.Entities.HospitalManagementSystemContext _context;

        public CreateModel(Cortex.Entities.HospitalManagementSystemContext context)
        {
            _context = context;
        }

        // ViewModel for the create form
        public class CreateLabOrderViewModel
        {
            public int PatientId { get; set; }
            public int DoctorId { get; set; }
            public string? Notes { get; set; }
            public List<SelectedTest> SelectedTests { get; set; } = new List<SelectedTest>();
            public bool IsUrgent { get; set; }
            public bool SendNotification { get; set; } = true;
            public bool PrintOrder { get; set; }
        }

        public class SelectedTest
        {
            public int TestId { get; set; }
            public string TestName { get; set; } = string.Empty;
            public string TestCode { get; set; } = string.Empty;
            public string Priority { get; set; } = "Normal";
            public string? Notes { get; set; }
        }

        [BindProperty]
        public CreateLabOrderViewModel LabOrderInput { get; set; } = new CreateLabOrderViewModel();

        // For displaying available tests
        public List<LabTest> AvailableTests { get; set; } = new List<LabTest>();

        public IActionResult OnGet()
        {
            // Load available tests
            AvailableTests = _context.LabTests
                .OrderBy(t => t.Category)
                .ThenBy(t => t.TestName)
                .ToList();

            // Populate dropdowns with proper formatting
            ViewData["DoctorId"] = new SelectList(
                _context.Staff
                    .Where(s => s.Role == "Doctor" || s.Role == "Physician")
                    .Select(s => new {
                        s.StaffID,
                        DisplayName = $"Dr. {s.FirstName} {s.LastName} - {s.Specialization}",
                        s.Specialization,
                        s.Department,
                        s.FirstName,
                        s.LastName,
                        s.ContactNumber
                    })
                    .OrderBy(s => s.FirstName)
                    .ThenBy(s => s.LastName),
                "StaffID",
                "DisplayName"
            );

            ViewData["PatientId"] = new SelectList(
                _context.Patients
                    .Select(p => new {
                        p.PatientId,
                        DisplayName = $"{p.FirstName} {p.LastName} - ID: {p.PatientId:D6}",
                        p.FirstName,
                        p.LastName,
                        p.DateOfBirth,
                        p.Gender,
                        p.ContactNumber
                    })
                    .OrderBy(p => p.LastName)
                    .ThenBy(p => p.FirstName),
                "PatientId",
                "DisplayName"
            );

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload data if validation fails
                await ReloadDataAsync();
                return Page();
            }

            // Validate selected tests
            if (LabOrderInput.SelectedTests == null || !LabOrderInput.SelectedTests.Any())
            {
                ModelState.AddModelError("", "Please select at least one test for the lab order.");
                await ReloadDataAsync();
                return Page();
            }

            try
            {
                // Create new LabOrder
                var labOrder = new LabOrder
                {
                    PatientId = LabOrderInput.PatientId,
                    DoctorId = LabOrderInput.DoctorId,
                    OrderDate = DateTime.Now,
                    Status = LabOrderInput.IsUrgent ? "Urgent" : "Pending",
                    Notes = LabOrderInput.Notes
                };

                // Add to database
                _context.LabOrders.Add(labOrder);
                await _context.SaveChangesAsync();

                // Add LabOrderDetails for each selected test
                foreach (var test in LabOrderInput.SelectedTests)
                {
                    var labOrderDetail = new LabOrderDetail
                    {
                        OrderId = labOrder.OrderId,
                        TestId = test.TestId,
                        Status = "Pending",
                        Notes = test.Notes,
                        LastUpdated = DateTime.Now
                    };

                    _context.LabOrderDetails.Add(labOrderDetail);
                }

                await _context.SaveChangesAsync();

                // Handle additional options
                if (LabOrderInput.SendNotification)
                {
                    // Send notification logic here
                    TempData["NotificationMessage"] = $"Lab Order #{labOrder.OrderId:D6} created successfully. Notification sent to lab.";
                }

                if (LabOrderInput.PrintOrder)
                {
                    TempData["PrintOrderId"] = labOrder.OrderId;
                }

                TempData["SuccessMessage"] = $"Lab Order #{labOrder.OrderId:D6} created successfully!";
                return RedirectToPage("./Details", new { id = labOrder.OrderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while creating the lab order: {ex.Message}");
                await ReloadDataAsync();
                return Page();
            }
        }

        private async Task ReloadDataAsync()
        {
            AvailableTests = await _context.LabTests
                .OrderBy(t => t.Category)
                .ThenBy(t => t.TestName)
                .ToListAsync();

            ViewData["DoctorId"] = new SelectList(
                _context.Staff
                    .Where(s => s.Role == "Doctor" || s.Role == "Physician")
                    .Select(s => new {
                        s.StaffID,
                        DisplayName = $"Dr. {s.FirstName} {s.LastName} - {s.Specialization}",
                        s.Specialization,
                        s.Department,
                        s.FirstName,
                        s.LastName,
                        s.ContactNumber
                    })
                    .OrderBy(s => s.FirstName)
                    .ThenBy(s => s.LastName),
                "StaffID",
                "DisplayName",
                LabOrderInput?.DoctorId
            );

            ViewData["PatientId"] = new SelectList(
                _context.Patients
                    .Select(p => new {
                        p.PatientId,
                        DisplayName = $"{p.FirstName} {p.LastName} - ID: {p.PatientId:D6}",
                        p.FirstName,
                        p.LastName,
                        p.DateOfBirth,
                        p.Gender,
                        p.ContactNumber
                    })
                    .OrderBy(p => p.LastName)
                    .ThenBy(p => p.FirstName),
                "PatientId",
                "DisplayName",
                LabOrderInput?.PatientId
            );
        }

        private int CalculateAge(DateOnly? birthDate)
        {
            if (!birthDate.HasValue)
                return 0;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - birthDate.Value.Year;

            if (birthDate.Value > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        // AJAX endpoint to get test details
        public IActionResult OnGetTestDetails(int testId)
        {
            var test = _context.LabTests.FirstOrDefault(t => t.TestId == testId);
            if (test == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                test.TestId,
                test.TestName,
                test.Category,
                test.Description,
                test.Price,
                test.Duration,
                test.SampleType
            });
        }

        // AJAX endpoint to get patient details
        public IActionResult OnGetPatientDetails(int patientId)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientId == patientId);
            if (patient == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                patient.PatientId,
                FullName = $"{patient.FirstName} {patient.LastName}",
                patient.FirstName,
                patient.LastName,
                patient.DateOfBirth,
                patient.Gender,
                patient.ContactNumber,
                patient.Email,
                patient.Address,
                patient.BloodType,
                patient.Allergies
            });
        }

        // AJAX endpoint to get doctor details
        public IActionResult OnGetDoctorDetails(int doctorId)
        {
            var doctor = _context.Staff.FirstOrDefault(s => s.StaffID == doctorId);
            if (doctor == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                doctor.StaffID,
                FullName = $"Dr. {doctor.FirstName} {doctor.LastName}",
                doctor.FirstName,
                doctor.LastName,
                doctor.Specialization,
                doctor.Department,
                doctor.ContactNumber,
                doctor.Email,
                doctor.Qualification
            });
        }
    }
}