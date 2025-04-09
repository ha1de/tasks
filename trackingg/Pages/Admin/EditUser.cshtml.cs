using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using trackingg.Data;

namespace trackingg.Pages.Admin
{
    [Authorize(Policy = "RequireAdminRole")]
    public class EditUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditUserModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public UserViewModel? UserViewModel { get; set; }

        public List<string> AllRoles { get; set; } = new List<string>();
        
        [BindProperty]
        public List<string> SelectedRoles { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            
            UserViewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Roles = userRoles.ToList()
            };

            AllRoles = _roleManager.Roles.Select(r => r.Name).Where(n => n != null).Select(n => n!).ToList();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || UserViewModel == null)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(UserViewModel.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = UserViewModel.FirstName;
            user.LastName = UserViewModel.LastName;
            
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                AllRoles = _roleManager.Roles.Select(r => r.Name).Where(n => n != null).Select(n => n!).ToList();
                return Page();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            
            var rolesToRemove = userRoles.Where(r => !SelectedRoles.Contains(r)).ToList();
            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    foreach (var error in removeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    AllRoles = _roleManager.Roles.Select(r => r.Name).Where(n => n != null).Select(n => n!).ToList();
                    return Page();
                }
            }

            var rolesToAdd = SelectedRoles.Where(r => !userRoles.Contains(r)).ToList();
            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    AllRoles = _roleManager.Roles.Select(r => r.Name).Where(n => n != null).Select(n => n!).ToList();
                    return Page();
                }
            }

            return RedirectToPage("./Users");
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        public List<string> Roles { get; set; } = new List<string>();
    }
}
