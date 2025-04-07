using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using trackingg.Data;
using trackingg.Models;

namespace trackingg.Pages.Issues
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Issue Issue { get; set; } = default!;

        public List<Project> Projects { get; set; } = new List<Project>();
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public async Task<IActionResult> OnGetAsync(uint id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            Projects = await _context.Projects.ToListAsync();
            Users = await _userManager.Users.ToListAsync(); // Fetch users for the dropdown

            var issue = await _context.Issues.Include(i => i.AssignedTo).Include(i => i.Project).FirstOrDefaultAsync(m => m.Id == id);
            if (issue == null)
            {
                return NotFound();
            }
            Issue = issue;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Projects = await _context.Projects.ToListAsync();
                Users = await _userManager.Users.ToListAsync();
                return Page();
            }

            _context.Attach(Issue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(Issue.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                // Log the inner exception for more details
                Console.WriteLine($"DbUpdateException: {ex.InnerException?.Message}");
                throw; // Re-throw the exception
            }

            return RedirectToPage("./Detail", new { id = Issue.Id });
        }

        private bool IssueExists(uint id)
        {
            return _context.Issues.Any(e => e.Id == id);
        }
    }
}