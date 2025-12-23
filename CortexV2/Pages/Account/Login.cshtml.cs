using Cortex.DTOs.auth;
using Cortex.Entities;
using Cortex.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization; // Make sure to include your DTO namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Cortex.UI.Areas.Web.Pages.Account
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        private readonly IApiClient _apiClient;


        public LoginModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }


        [BindProperty]
        public string Username { get; set; } = string.Empty;


        [BindProperty]
        public string Password { get; set; } = string.Empty;


        public string? ErrorMessage { get; set; }


        public void OnGet() { }


        public async Task<IActionResult> OnPostAsync()
        {
            var form = await Request.ReadFormAsync();
            Console.WriteLine("Raw form values:");
            foreach (var key in form.Keys)
            {
                Console.WriteLine($"{key}: {form[key]}");
            }

            // Read values directly from form instead of relying on model binding
            var username = form["Username"].ToString();
            var password = form["Password"].ToString();
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Username and password are required.";
                return Page();
            }


            var auth = await _apiClient.LoginAsync(Username.Trim(), Password);
            if (auth == null)
            {
                ErrorMessage = "Invalid credentials or API unreachable.";
                return Page();
            }


            // Create claims including the token (as claim "access_token")
            var claims = new List<Claim>
{
new Claim(ClaimTypes.Name, auth.Staff != null ? $"{auth.Staff.FirstName} {auth.Staff.LastName}" : auth.Patient != null ? $"{auth.Patient.FirstName} {auth.Patient.LastName}" : Username),
new Claim("user_type", auth.UserType.ToLower()),
new Claim("access_token", auth.Token ?? string.Empty),
};


            // If staff, add role claim
            if (auth.Staff != null && !string.IsNullOrEmpty(auth.Staff.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, auth.Staff.Role));
            }


            // Add identifiers
            if (auth.Staff != null)
                claims.Add(new Claim("staff_id", auth.Staff.StaffID.ToString()));
            /*if (auth.Patient != null)
                claims.Add(new Claim("patient_id", auth.Patient.PatientID.ToString()));*/


            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);


            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if(auth?.Staff != null)
                 return RedirectToPage("/Account/Dashboard", new { area = "Web" });
            else
                return RedirectToPage("/Account/AccessDenied", new { area = "Web" });
        }
    }

    /* public class LoginModel1 : PageModel
     {
         private readonly IApiClient _apiClient;


         public LoginModel1(IApiClient apiClient)
         {
             _apiClient = apiClient;
         }


         [BindProperty]
         public string Username { get; set; } = string.Empty;


         [BindProperty]
         public string Password { get; set; } = string.Empty;


         public string? ErrorMessage { get; set; }


         public void OnGet() { }


         public async Task<IActionResult> OnPostAsync()
         {
             ErrorMessage = null;


             if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrEmpty(Password))
             {
                 ErrorMessage = "Username and password are required.";
                 return Page();
             }


             var auth = await _apiClient.LoginAsync(Username.Trim(), Password);
             if (auth == null)
             {
                 ErrorMessage = "Invalid credentials or API unreachable.";
                 return Page();
             }


             // Create claims including the token (as claim "access_token")
             var claims = new List<Claim>
 {
 new Claim(ClaimTypes.Name, auth.Staff != null ? $"{auth.Staff.FirstName} {auth.Staff.LastName}" : auth.Patient != null ? $"{auth.Patient.FirstName} {auth.Patient.LastName}" : Username),
 new Claim("user_type", auth.UserType.ToLower()),
 new Claim("access_token", auth.Token ?? string.Empty),
 };


             // If staff, add role claim
             if (auth.Staff != null && !string.IsNullOrEmpty(auth.Staff.Role))
             {
                 claims.Add(new Claim(ClaimTypes.Role, auth.Staff.Role));
             }


             // Add identifiers
             if (auth.Staff != null)
                 claims.Add(new Claim("staff_id", auth.Staff.StaffID.ToString()));
             if (auth.Patient != null)
                 claims.Add(new Claim("patient_id", auth.Patient.PatientID.ToString()));


             var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
             var principal = new ClaimsPrincipal(identity);


             await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

             return RedirectToPage("/Account/Dashboard", new { area = "Web" });
         }
     }*/
}