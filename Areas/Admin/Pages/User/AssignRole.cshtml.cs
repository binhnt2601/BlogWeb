// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Admin.User
{
    public class AssignRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AssignRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
    
        [BindProperty]
        [Display(Name = "Roles")]
        public string[] RoleNames {get;set;}
        public AppUser user {get; set;}
        public SelectList allRoles {get; set;}

        public List<IdentityRoleClaim<string>> roleClaims {get; set; }
        public List<IdentityUserClaim<string>> userClaims {get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound("User not Found");
            }
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID: "+ id);
            }
            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();
            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);
            await GetUserRoleClaim(id);
            
            return Page();
        }

        async Task GetUserRoleClaim(string id)
        {
            var listRole = from r in _context.Roles
                           join ur in _context.UserRoles on r.Id equals ur.RoleId
                           where ur.UserId == id
                           select r;

            var listRoleClaim = from c in _context.RoleClaims
                             join r in listRole on c.RoleId equals r.Id
                             select c;
            roleClaims =  await listRoleClaim.ToListAsync();

            userClaims = await (from c in _context.UserClaims
                                where c.UserId == id
                                select c).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound("Not found this user");
            }
            
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{id}'.");
            }
            //Roles
            await GetUserRoleClaim(id);
            var oldRoles = (await _userManager.GetRolesAsync(user)).ToArray();
            var deleteRoles = oldRoles.Where(r => !RoleNames.Contains(r));
            var addRoles = RoleNames.Where(r => !oldRoles.Contains(r));

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRoles);
            if(!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(e => {
                    ModelState.AddModelError(string.Empty, e.Description);
                });
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(user, addRoles);
            if(!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(e => {
                    ModelState.AddModelError(string.Empty, e.Description);
                });
                return Page();
            }

            StatusMessage = $"Update Roles for user {user.UserName}";

            return RedirectToPage("./Index");
        }
    }
}
