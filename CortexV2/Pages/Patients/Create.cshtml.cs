// Areas/Web/Pages/Patients/Create.cshtml.cs
using BCrypt.Net;
using Cortex.DTOs.Users;
using Cortex.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cortex.UI.Areas.Web.Pages.Patients
{
    public class CreateModel : PageModel
    {
        private readonly PatientsService _patients;

        public CreateModel(PatientsService patients)
        {
            _patients = patients;
        }

        [BindProperty]
        public PatientDto Input { get; set; } = new PatientDto();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var pass  = Input.ContactNumber.Replace("-", "");
            Input.PasswordHash = helper.Helper.IncreptPassword(pass);
            string name = Input.FirstName;
            Input.Username = name[0] + Input.LastName.ToLower();
            
            if (!ModelState.IsValid) return Page();
            var created = await _patients.CreateAsync(Input);

            if (created == null)
            {
                ModelState.AddModelError(string.Empty, "Create failed");
                return Page();
            }

            TempData["StatusMessage"] = "Patient created.";
            return RedirectToPage("./Index", new { area = "Web" });

        }
    }
}
