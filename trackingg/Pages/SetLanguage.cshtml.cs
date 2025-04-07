using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace trackingg.Pages
{
    public class SetLanguageModel : PageModel
    {
        [BindProperty]
        public string Culture { get; set; }

        [BindProperty]
        public string ReturnUrl { get; set; }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Culture))
                return BadRequest();

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(Culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true
                });

            if (!Url.IsLocalUrl(ReturnUrl))
                ReturnUrl = "/";

            return LocalRedirect(ReturnUrl);
        }
    }
}