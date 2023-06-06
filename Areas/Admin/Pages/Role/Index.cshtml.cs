using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using razor07.Models;

namespace razor07.Admin.Role
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager, context)
        {
        }
        public List<IdentityRole> roles { get; set; }
        public async Task OnGet()
        {
            roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        }

        public void OnPost() => RedirectToPage();

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var deleteRole = await _roleManager.FindByIdAsync(id);
            System.Console.WriteLine(deleteRole.Name);
            if(deleteRole == null)
            {

                return Page();
            }
            var result = await _roleManager.DeleteAsync(deleteRole);
            if(result.Succeeded)
            {
                return RedirectToPage("./Index");
            }
            else{
                result.Errors.ToList().ForEach(error =>{
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
            
        }
    }
}
