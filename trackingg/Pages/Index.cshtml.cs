using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using trackingg.Data;
using trackingg.Models;

namespace trackingg.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context) => _context = context;

        public async Task OnGet()
        {
            Issues = await _context.Issues
                .Include(i => i.AssignedTo)
                .Include(i => i.Project)
                .Where(i => i.Completed == null)
                .OrderByDescending(i => i.Created)
                .ToListAsync();
        }

        public IEnumerable<Issue> Issues { get; set; } = Enumerable.Empty<Issue>();

        public async Task<IActionResult> OnPostDeleteAsync(uint id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}