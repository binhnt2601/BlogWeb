using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Admin.User
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public class UsersRoles:AppUser
        {
            public string userRoles { get; set; }
        }
        public List<UsersRoles> users { get; set; }

        public List<string> roles {get; set;}

        [TempData]
        public string StatusMessage { get; set; }

        //Paging Properties
        public const int itemPerPage = 10;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }
        public int countPages { get; set; }

        public async Task OnGet()
        {
            // users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            // var qr = _userManager.Users.OrderBy(u => u.UserName);

            int totalUsers = await _userManager.Users.CountAsync();
            countPages = (int)Math.Ceiling((double)totalUsers/itemPerPage);
            if(currentPage < 1)
            {
                currentPage = 1;
            }
            if(currentPage>countPages)
            {
                currentPage = countPages;
            }

            var qr = _userManager.Users.OrderBy(u => u.UserName).Skip((currentPage - 1)*itemPerPage)
                        .Take(itemPerPage).Select(u => new UsersRoles(){
                            Id = u.Id,
                            UserName = u.UserName
                        });
            users = await qr.ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.userRoles = string.Join(", ", roles);
            }
        }

        public void OnPost() => RedirectToPage();

    }
}
