using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor07.Models;

namespace razor07.Admin.Role
{
    [Authorize(Roles = "Admin")]
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager, context)
        {
        }

        public class InputModel
        {
            [Required]
            [StringLength(250, MinimumLength = 3)]
            public string RoleName {get;set;}
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(string roleId)
        {
            if(roleId == null)
            {
                return NotFound("There is no Role with this ID");
            }
            var role = await _roleManager.FindByIdAsync(roleId);
            if(role != null)
            {
               Input = new InputModel(){
                RoleName = role.Name
               };
               return Page();
            }
            return NotFound("There is no Role with this ID");
        }

        public async Task<IActionResult> OnPostAsync(string roleId)
        {
            if(roleId == null)
            {
                return NotFound("There is no Role with this ID");
            }
            var role = await _roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                return NotFound("There is no Role with this ID");
            }
            if(!ModelState.IsValid)
            {
                return Page();
            }
            var oldName = role.Name;
            role.Name = Input.RoleName;
            var result = await _roleManager.UpdateAsync(role);
            if(result.Succeeded)
            {
                StatusMessage = $"Change Role {oldName} to {role.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>{
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
        }

        
    }
}