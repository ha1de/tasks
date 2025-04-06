using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace trackingg.Pages.Account
{
    public class ExternalLoginModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Provider { get; set; } = "Google";

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/";

        public IActionResult OnGet()
        {
            // Request a redirect to the external login provider
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { ReturnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, Provider);
        }

        public async Task<IActionResult> OnGetCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Get the login info about the user from the external login provider
            var info = await HttpContext.AuthenticateAsync("External");
            if (info == null || !info.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, info.Principal);
            
            return LocalRedirect(returnUrl);
        }
    }
}