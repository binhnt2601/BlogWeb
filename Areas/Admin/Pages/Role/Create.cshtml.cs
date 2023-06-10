using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace App.Admin.Role
{
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager, context)
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

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            var newRole = new IdentityRole(Input.RoleName);
            var result = await _roleManager.CreateAsync(newRole);
            if(result.Succeeded)
            {
                StatusMessage = $"Added new Role : {Input.RoleName}";
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
