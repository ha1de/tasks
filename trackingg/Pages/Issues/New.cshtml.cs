using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using trackingg.Data;
using trackingg.Models;

namespace trackingg.Pages.Issues
{
    [Authorize]
    public class NewModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NewModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager) 
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Issue Issue { get; set; } = new Issue();

        public void OnGet()
        {
            // Initialize any default values if needed
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid();
            }

            Issue.Created = DateTime.Now;
            Issue.AssignedToId = user.Id; // Assign to current user by default
            
            await _context.Issues.AddAsync(Issue);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Index");
        }
    }
}