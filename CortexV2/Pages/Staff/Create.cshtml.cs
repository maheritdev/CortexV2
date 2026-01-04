using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Cortex.Entities;

namespace Cortex.Pages.Staff
{
    public class CreateModel : PageModel
    {
        private readonly HospitalManagementSystemContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            HospitalManagementSystemContext context,
            ILogger<CreateModel> logger = null)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public StaffInputModel StaffInput { get; set; } = new StaffInputModel();

        // ViewModel for form data
        public class StaffInputModel
        {
            [Required(ErrorMessage = "First name is required")]
            [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Last name is required")]
            [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; } = string.Empty;

            [Display(Name = "Gender")]
            public string? Gender { get; set; }

            [Display(Name = "Date of Birth")]
            [DataType(DataType.Date)]
            public DateOnly? DateOfBirth { get; set; }

            [Required(ErrorMessage = "Contact number is required")]
            [Phone(ErrorMessage = "Invalid phone number format")]
            [Display(Name = "Contact Number")]
            public string ContactNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            [Display(Name = "Email Address")]
            public string Email { get; set; } = string.Empty;

            [Display(Name = "Address")]
            [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
            public string? Address { get; set; }

            [Required(ErrorMessage = "Department is required")]
            [Display(Name = "Department")]
            public int DepartmentId { get; set; }

            [Required(ErrorMessage = "Position is required")]
            [StringLength(100, ErrorMessage = "Position cannot exceed 100 characters")]
            [Display(Name = "Position")]
            public string Position { get; set; } = string.Empty;

            [Display(Name = "Specialization")]
            [StringLength(150, ErrorMessage = "Specialization cannot exceed 150 characters")]
            public string? Specialization { get; set; }

            [Display(Name = "Qualification")]
            [StringLength(150, ErrorMessage = "Qualification cannot exceed 150 characters")]
            public string? Qualification { get; set; }

            [Required(ErrorMessage = "Hire date is required")]
            [Display(Name = "Hire Date")]
            [DataType(DataType.Date)]
            public DateOnly HireDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

            [Display(Name = "Salary")]
            [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value")]
            public decimal? Salary { get; set; }

            [Required(ErrorMessage = "Username is required")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
            [Display(Name = "Username")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Role is required")]
            [Display(Name = "Role")]
            public string Role { get; set; } = "Staff";
        }

        // Properties for dropdowns
        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> RoleOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> GenderOptions { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadDropdownDataAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading data for staff creation");
                ModelState.AddModelError(string.Empty, "Error loading required data. Please try again.");
                await LoadDropdownDataAsync(); // Still load dropdowns even on error
                return Page();
            }
        }

        private async Task LoadDropdownDataAsync()
        {
            // Load departments
            var departments = await _context.Departments
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            if (departments != null && departments.Any())
            {
                Departments = departments
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.DepartmentName
                    })
                    .ToList();
            }
            else
            {
                Departments = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "No departments available", Disabled = true }
                };
            }

            // Role options
            RoleOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Administrator" },
                new SelectListItem { Value = "Manager", Text = "Manager" },
                new SelectListItem { Value = "Staff", Text = "Staff", Selected = true },
                new SelectListItem { Value = "Viewer", Text = "Viewer" }
            };

            // Gender options
            GenderOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Select Gender" },
                new SelectListItem { Value = "Male", Text = "Male" },
                new SelectListItem { Value = "Female", Text = "Female" },
                new SelectListItem { Value = "Other", Text = "Other" }
            };

            // Set default hire date
            StaffInput.HireDate = DateOnly.FromDateTime(DateTime.Today);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Load dropdown data first (needed for validation errors)
                await LoadDropdownDataAsync();

                // Validate model
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                // Check for duplicate email
                var existingEmail = await _context.Staff
                    .AnyAsync(s => s.Email == StaffInput.Email);

                if (existingEmail)
                {
                    ModelState.AddModelError(nameof(StaffInput.Email),
                        "Email address is already registered.");
                    return Page();
                }

                // Check for duplicate username
                var existingUsername = await _context.Staff
                    .AnyAsync(s => s.Username == StaffInput.Username);

                if (existingUsername)
                {
                    ModelState.AddModelError(nameof(StaffInput.Username),
                        "Username is already taken. Please choose another.");
                    return Page();
                }

                // Validate date of birth (must be at least 18 years old)
                if (StaffInput.DateOfBirth.HasValue)
                {
                    var minDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-18));
                    if (StaffInput.DateOfBirth > minDate)
                    {
                        ModelState.AddModelError(nameof(StaffInput.DateOfBirth),
                            "Staff must be at least 18 years old.");
                        return Page();
                    }
                }

                // Validate hire date (cannot be in future)
                if (StaffInput.HireDate > DateOnly.FromDateTime(DateTime.Today))
                {
                    ModelState.AddModelError(nameof(StaffInput.HireDate),
                        "Hire date cannot be in the future.");
                    return Page();
                }

                // Map from InputModel to Entity
                var staffEntity = new Entities.Staff
                {
                    FirstName = StaffInput.FirstName.Trim(),
                    LastName = StaffInput.LastName.Trim(),
                    Gender = StaffInput.Gender,
                    DateOfBirth = StaffInput.DateOfBirth,
                    ContactNumber = StaffInput.ContactNumber.Trim(),
                    Email = StaffInput.Email.Trim().ToLower(),
                    Address = StaffInput.Address?.Trim(),
                    DepartmentId = StaffInput.DepartmentId,
                    Position = StaffInput.Position.Trim(),
                    Specialization = StaffInput.Specialization?.Trim(),
                    Qualification = StaffInput.Qualification?.Trim(),
                    HireDate = StaffInput.HireDate,
                    Salary = StaffInput.Salary,
                    Username = StaffInput.Username.Trim().ToLower(),
                    PasswordHash = HashPassword(StaffInput.Password),
                    Role = StaffInput.Role
                };

                // Add to context and save
                _context.Staff.Add(staffEntity);
                await _context.SaveChangesAsync();

                // Success message
                TempData["SuccessMessage"] = $"Staff member '{staffEntity.FirstName} {staffEntity.LastName}' created successfully!";

                // Log successful creation
                _logger?.LogInformation("Staff member created: {Id} - {Name}",
                    staffEntity.StaffID, $"{staffEntity.FirstName} {staffEntity.LastName}");

                return RedirectToPage("./Index");
            }
            catch (DbUpdateException dbEx)
            {
                _logger?.LogError(dbEx, "Database error while creating staff");
                ModelState.AddModelError(string.Empty,
                    "An error occurred while saving. Please try again.");
                await LoadDropdownDataAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating staff member");
                ModelState.AddModelError(string.Empty,
                    "An unexpected error occurred. Please try again.");
                await LoadDropdownDataAsync();
                return Page();
            }
        }

        private string HashPassword(string password)
        {
            // Note: In production, use a proper hashing algorithm like BCrypt or ASP.NET Identity
            // This is a simplified example
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password + "SALT_VALUE");
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}