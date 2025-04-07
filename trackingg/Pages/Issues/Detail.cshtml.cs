using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using trackingg.Data;
using trackingg.Models;

namespace trackingg.Pages.Issues
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailModel(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> OnGet(uint id)
        {
            Issue = await _context.Issues
                .Include(i => i.AssignedTo)
                .Include(i => i.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Issue == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnGetResolve(uint id)
        {
            var issueToUpdate = await _context.Issues.FindAsync(id);
            if (issueToUpdate == null) return NotFound();

            issueToUpdate.Completed = DateTime.Now;
            _context.Update(issueToUpdate);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostDelete(uint id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

        public Issue? Issue { get; private set; }
    }
}