using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor07.Models;

namespace razor07.Admin.Role
{
    public class EditClaimModel : RolePageModel
    {
        public EditClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager, context)
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

        public IdentityRoleClaim<string> claim { get; set; }

        public async Task<IActionResult> OnGetAsync(int? claimId)
        {
            if(claimId == null)
            {
                return NotFound("Not found this Claim");
            }
            claim = _context.RoleClaims.Where(c => c.Id == claimId).FirstOrDefault();
            if(claim == null)
            {
                return NotFound("Not found this Claim");
            }
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if(role == null)
            {
                return NotFound("Not Found this Role");
            }
            Input = new InputModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? claimId)
        {
            if(claimId == null)
            {
                return NotFound("Not found this Claim");
            }
            claim = _context.RoleClaims.Where(c => c.Id == claimId).FirstOrDefault();
            if(claim == null)
            {
                return NotFound("Not found this Claim");
            }
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if(role == null)
            {
                return NotFound("Not Found this Role");
            }
            if(!ModelState.IsValid)
            {
                return Page();
            }
            if(_context.RoleClaims
                .Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id == claimId))
            {
                ModelState.AddModelError(string.Empty, $"{role.Name} has contained this Claim");
                return Page();
            };

            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;
            await _context.SaveChangesAsync();

            StatusMessage = $"Claim Updated";
            return RedirectToPage("./Edit", new {roleId = role.Id});
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? claimId)
        {
            if(claimId == null)
            {
                return NotFound("Not found this Claim");
            }
            claim = _context.RoleClaims.Where(c => c.Id == claimId).FirstOrDefault();
            if(claim == null)
            {
                return NotFound("Not found this Claim");
            }
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if(role == null)
            {
                return NotFound("Not Found this Role");
            }
            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));
            StatusMessage = $"Claim Deleted";
            return RedirectToPage("./Edit", new {roleId = role.Id});
        }        

        
    }
}
