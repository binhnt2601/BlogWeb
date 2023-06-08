using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor07.Models;

namespace razor07.Admin.Role
{
    public class AddClaimsModel : RolePageModel
    {
        public AddClaimsModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager, context)
        {
        }

        public class InputModel
        {
            [Required]
            [StringLength(250, MinimumLength = 3)]
            public string ClaimType {get;set;}

            [Required]
            [StringLength(250, MinimumLength = 3)]
            public string ClaimValue { get; set; }
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public IdentityRole role {get; set; }

        public async Task<IActionResult> OnGetAsync(string roleId)
        {
            role = await _roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                return NotFound("Not Found this Role");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleId)
        {
            role = await _roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                return NotFound("Not Found this Role");
            }   
            if(!ModelState.IsValid)
            {
                return Page();
            }

            if((await _roleManager.GetClaimsAsync(role))
                .Any(c => c.Type == Input.ClaimType && c.Value == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, $"{role.Name} has contained this Claim");
                return Page();
            };

            var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result = await _roleManager.AddClaimAsync(role, newClaim);
            if(!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => {
                    ModelState.AddModelError(string.Empty, e.Description);
                });
                return Page();
            }

            StatusMessage = $"New Claim Added";
            return RedirectToPage("./Edit", new {roleId = role.Id});
        }

        
    }
}
