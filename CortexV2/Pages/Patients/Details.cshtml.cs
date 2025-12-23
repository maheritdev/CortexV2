// Areas/Web/Pages/Patients/Details.cshtml.cs
using Cortex.DTOs.Users;
using Cortex.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Cortex.UI.Areas.Web.Pages.Patients
{
    public class DetailsModel : PageModel
    {
        private readonly PatientsService _patients;

        public DetailsModel(PatientsService patients)
        {
            _patients = patients;
        }

        public PatientDto? Patient { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Patient = await _patients.GetByIdAsync(id);
            if (Patient == null) return NotFound();
            return Page();
        }
    }
}
