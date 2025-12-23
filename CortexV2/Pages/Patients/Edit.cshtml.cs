// Areas/Web/Pages/Patients/Edit.cshtml.cs
using Cortex.DTOs.Users;
using Cortex.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Cortex.UI.Areas.Web.Pages.Patients
{
    public class EditModel : PageModel
    {
        private readonly PatientsService _patients;

        public EditModel(PatientsService patients)
        {
            _patients = patients;
        }

        [BindProperty]
        public PatientDto Input { get; set; } = new PatientDto();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var p = await _patients.GetByIdAsync(id);
            if (p == null) return NotFound();
            Input = p;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var ok = await _patients.UpdateAsync(Input.PatientID, Input);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Update failed.");
                return Page();
            }

            TempData["StatusMessage"] = "Patient updated.";
            return RedirectToPage("./Index", new { area = "Web" });
        }
    }
}
