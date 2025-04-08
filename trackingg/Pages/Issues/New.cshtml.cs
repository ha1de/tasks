using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations; 
using Microsoft.AspNetCore.Antiforgery;
using trackingg.Data;
using trackingg.Models;

namespace trackingg.Pages.Issues
{
    [Authorize]
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class NewModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NewModel> _logger;
        private readonly IAntiforgery _antiforgery;

        public NewModel(ApplicationDbContext context,
                        UserManager<ApplicationUser> userManager,
                        ILogger<NewModel> logger,
                        IAntiforgery antiforgery)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _antiforgery = antiforgery;
        }

        [BindProperty]
        public Issue Issue { get; set; } = new Issue();

        public IEnumerable<Project> Projects { get; private set; } = Enumerable.Empty<Project>();

        public IAntiforgery Antiforgery => _antiforgery;

        private async Task PopulateProjectsAsync()
        {
            try
            {
                _logger.LogInformation("PopulateProjectsAsync: Querying database for projects...");
                Projects = await _context.Projects
                                         .OrderBy(p => p.Name)
                                         .AsNoTracking()
                                         .ToListAsync();
                _logger.LogInformation("PopulateProjectsAsync: Found {Count} projects.", Projects.Count());
                if (!Projects.Any())
                {
                    _logger.LogWarning("PopulateProjectsAsync: No projects found in the database! Dropdown will be empty unless projects are created.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PopulateProjectsAsync: Error querying projects from database.");
                Projects = Enumerable.Empty<Project>();
            }
        }

        public async Task OnGetAsync()
        {
            _logger.LogInformation("OnGetAsync called. Populating projects...");
            await PopulateProjectsAsync();
            _logger.LogInformation("OnGetAsync finished populating projects.");
        }

        public async Task<IActionResult> OnPostAsync()
        {
             _logger.LogInformation("OnPostAsync (Issue Submission) invoked.");
             bool isAjaxRequest = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
             var user = await _userManager.GetUserAsync(User);

             if (user == null) {
                 _logger.LogWarning("OnPostAsync: Current user not found unexpectedly.");
                 ModelState.AddModelError("", "Unable to identify current user.");
                 await PopulateProjectsAsync();
                 if (isAjaxRequest) return Unauthorized();
                 return Page();
             }

            Issue.AssignedToId = user.Id;

            LogModelStateErrors("DIAGNOSTIC - Before Check (Issue Submission)");

            if (!ModelState.IsValid) {
                _logger.LogWarning("OnPostAsync: ModelState is invalid for Issue submission.");
                LogModelStateErrors("INVALID STATE (Issue Submission)");
                _logger.LogInformation("OnPostAsync: ModelState invalid, repopulating projects before returning Page.");
                await PopulateProjectsAsync();
                if (isAjaxRequest) return new BadRequestObjectResult(ModelState);
                return Page();
            }

            _logger.LogInformation("OnPostAsync: ModelState is valid for Issue submission. Saving issue...");
            Issue.Created = DateTime.UtcNow;
            _context.Issues.Add(Issue);

            try {
                await _context.SaveChangesAsync();
                _logger.LogInformation("OnPostAsync: Saved new issue '{IssueTitle}' (ID: {IssueId})", Issue.Title, Issue.Id);
                if (isAjaxRequest) {
                     var redirectUrl = Url.Page("/Index");
                     if (string.IsNullOrEmpty(redirectUrl)) { _logger.LogWarning("Could not generate redirect URL."); return StatusCode(500, "Issue saved, redirect failed."); }
                     return new JsonResult(new { redirectUrl });
                }
                return RedirectToPage("/Index");
            }
            catch (Exception ex) {
                 _logger.LogError(ex, "OnPostAsync: Error saving new issue '{IssueTitle}'.", Issue.Title);
                 _logger.LogInformation("OnPostAsync: Exception occurred, repopulating projects before returning Page.");
                 await PopulateProjectsAsync(); // Repopulate for page redisplay
                 if (isAjaxRequest) { return StatusCode(500, "An unexpected error occurred while saving the issue."); }
                 ModelState.AddModelError(string.Empty, "An unexpected error occurred saving the issue.");
                 return Page();
            }
        }

        public class CreateProjectAjaxModel
        {
            [Required(ErrorMessage = "Project Name is required.")]
            [StringLength(100)]
            public string ProjectName { get; set; }

            [StringLength(300)]
            public string ProjectDescription { get; set; }
        }

        public async Task<IActionResult> OnPostCreateProjectAsync([FromBody] CreateProjectAjaxModel data)
        {
            _logger.LogInformation("OnPostCreateProjectAsync invoked via AJAX. Data received: ProjectName='{ProjectName}'", data?.ProjectName);

             var keysToClear = ModelState.Keys.Where(k => k.StartsWith("Issue")).ToList();
             if (keysToClear.Any())
             {
                 _logger.LogInformation("Attempting to remove ModelState entries for keys: {Keys}", string.Join(", ", keysToClear));
                 foreach (var key in keysToClear) { ModelState.Remove(key); } // Try removing directly
             }
             if (ModelState.ContainsKey("Issue")) { ModelState.Remove("Issue"); _logger.LogInformation("Removed top-level 'Issue' key."); }
    
            try {
                await _antiforgery.ValidateRequestAsync(HttpContext);
                 _logger.LogInformation("Antiforgery token validated successfully for CreateProject.");
            } catch (AntiforgeryValidationException ex) {
                 _logger.LogWarning(ex, "Antiforgery token validation failed for CreateProject.");
                 return new JsonResult(new { success = false, errors = new[] { "Security validation failed." } }) { StatusCode = 400 };
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                 _logger.LogWarning("CreateProject: User not authenticated.");
                 return new JsonResult(new { success = false, errors = new[] { "User not authenticated." } }) { StatusCode = 401 };
            }

            var projectValidationErrors = new List<string>();
            if (data == null) {
                projectValidationErrors.Add("No project data received.");
            } else {
                 if (string.IsNullOrWhiteSpace(data.ProjectName)) { projectValidationErrors.Add("Project Name is required."); }
                 else { data.ProjectName = data.ProjectName.Trim(); if (data.ProjectName.Length > 100) projectValidationErrors.Add("Project Name cannot exceed 100 characters."); }
                 if (data.ProjectDescription != null && data.ProjectDescription.Length > 300) { projectValidationErrors.Add("Project Description cannot exceed 300 characters."); }
            }
            
            if (projectValidationErrors.Any()) {
                _logger.LogWarning("CreateProject: Invalid project data based on manual checks. Errors: {Errors}", string.Join("; ", projectValidationErrors));
                return new JsonResult(new { success = false, errors = projectValidationErrors }) { StatusCode = 400 };
            }

             bool nameExists = await _context.Projects.AnyAsync(p => p.Name == data.ProjectName);
             if (nameExists) {
                  _logger.LogWarning("CreateProject: Project name '{ProjectName}' already exists.", data.ProjectName);
                  return new JsonResult(new { success = false, errors = new[] { $"Project name '{data.ProjectName}' already exists." } }) { StatusCode = 400 };
             }
             
            var newProject = new Project {
                Name = data.ProjectName,
                Description = data.ProjectDescription ?? string.Empty,
                Created = DateTime.UtcNow,
                OwnerId = user.Id
            };
            _context.Projects.Add(newProject);

            try {
                await _context.SaveChangesAsync();
                _logger.LogInformation("CreateProject: New project '{ProjectName}' (ID: {ProjectId}) created by User {UserId}.", newProject.Name, newProject.Id, user.Id);
                // 6. Return Success
                return new JsonResult(new { success = true, newProjectId = newProject.Id, newProjectName = newProject.Name });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "CreateProject: Error saving new project '{ProjectName}'.", data.ProjectName);
                return new JsonResult(new { success = false, errors = new[] { "An unexpected error occurred while saving the project." } }) { StatusCode = 500 };
            }
        }

        private void LogModelStateErrors(string contextMessage)
        {
             if (!ModelState.IsValid) {
                 _logger.LogWarning("{Context}: ModelState is invalid. Error details:", contextMessage);
                 foreach (var key in ModelState.Keys) {
                     var state = ModelState[key];
                     if (state != null && state.Errors.Any()) {
                         var firstError = state.Errors.First();
                         _logger.LogWarning("- Field: {Field}, Error: {ErrorMessage}", key, firstError.ErrorMessage);
                         if(firstError.Exception != null) { _logger.LogWarning(firstError.Exception, "  Exception for Field {Field}:", key); }
                     }
                 }
             }
             else { _logger.LogInformation("{Context}: ModelState is valid.", contextMessage); }
        }
    }
}