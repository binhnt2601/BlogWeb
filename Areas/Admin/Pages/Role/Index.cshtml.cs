using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using razor07.Models;

namespace razor07.Admin.Role
{
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager, context)
        {
        }

        public class RoleModel : IdentityRole
        {
            public string[] Claims { get; set; }
        }
        public List<RoleModel> roles { get; set; }
        public async Task OnGet()
        {
            // _roleManager.GetClaimsAsync();
            var r = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            roles = new List<RoleModel>();
            foreach (var _r in r)
            {
                var claims = await _roleManager.GetClaimsAsync(_r);
                var claimsString = claims.Select(c => c.Type + " = " + c.Value);
                var rm = new RoleModel()
                {
                    Name = _r.Name,
                    Id = _r.Id,
                    Claims = claimsString.ToArray()
                };
                roles.Add(rm);
            }
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
