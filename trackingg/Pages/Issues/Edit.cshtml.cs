using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public List<Project> Projects { get; set; } = new();
        public List<ApplicationUser> Users { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            Issue = await _context.Issues
                .Include(i => i.Project)
                .Include(i => i.AssignedTo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Issue == null)
                return NotFound();

            Projects = await _context.Projects.ToListAsync();
            Users = await _userManager.Users.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                Projects = await _context.Projects.ToListAsync();
                Users = await _userManager.Users.ToListAsync();
                return Page();
            }

            var issueToUpdate = await _context.Issues.FindAsync((uint)id);
            if (issueToUpdate == null)
                return NotFound();

            issueToUpdate.Title = Issue.Title;
            issueToUpdate.Description = Issue.Description;
            issueToUpdate.Priority = Issue.Priority;
            issueToUpdate.IssueType = Issue.IssueType;
            issueToUpdate.ProjectId = Issue.ProjectId;
            issueToUpdate.AssignedToId = Issue.AssignedToId;

            await _context.SaveChangesAsync();
            return RedirectToPage("/Index");
        }
    }
}
