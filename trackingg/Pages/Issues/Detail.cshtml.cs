using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using trackingg.Data;
using trackingg.Models;

namespace trackingg.Pages.Issues
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IssueDbContext _context;

        public DetailModel(IssueDbContext context) => _context = context;

        public async Task<IActionResult> OnGet(uint id)
        {
            Issue = await _context.Issues.FindAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnGetResolve(uint id)
        {
            var issueToUpdate = _context.Issues.SingleOrDefault(i => i.Id == id);
            if (issueToUpdate == null) return NotFound();

            issueToUpdate.Completed = DateTime.Now;
            _context.Update(issueToUpdate);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("/Index");
        }

        public Issue? Issue { get; private set; }
    }
}