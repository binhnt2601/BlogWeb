using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Admin.User
{
    public class UserClaimModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public UserClaimModel(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public class InputModel
        {
            [Required]
            [StringLength(250, MinimumLength = 3)]
            public string ClaimType { get; set; }

            [Required]
            [StringLength(250, MinimumLength = 3)]
            public string ClaimValue { get; set; }
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public AppUser user { get; set; }

        public IdentityUserClaim<string> userClaim { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public NotFoundObjectResult OnGet() => NotFound("asdas");

        public async Task<IActionResult> OnGetCreateAsync(string userId)
        {
            user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostCreateAsync(string userId)
        {
            user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var currentClaims = _context.UserClaims.Where(c => c.UserId == userId);
            if (currentClaims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "User has already has this claim");
                return Page();
            }
            await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
            StatusMessage = "Add new Claim to User";
            return RedirectToPage("./AssignRole", new { id = userId });

        }

        public async Task<IActionResult> OnGetEditAsync(int? claimId)
        {
            if (claimId == null)
            {
                return NotFound("ClaimID == null");
            }
            userClaim = _context.UserClaims.Where(c => c.Id == claimId).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userClaim.UserId);

            if (user == null)
            {
                return NotFound();
            }
            Input = new InputModel()
            {
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue
            };
            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync(int? claimId)
        {
            if (claimId == null)
            {
                return NotFound("ClaimID == null");
            }
            userClaim = _context.UserClaims.Where(c => c.Id == claimId).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userClaim.UserId);

            if (user == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var currentClaim = _context.UserClaims.Where(c => c.UserId == userClaim.UserId);
            if (currentClaim.Any(c => c.ClaimType == userClaim.ClaimType && c.ClaimValue == userClaim.ClaimValue && c.Id != userClaim.Id))
            {
                ModelState.AddModelError(string.Empty, "User already has this claim");
                return Page();
            }

            userClaim.ClaimType = Input.ClaimType;
            userClaim.ClaimValue = Input.ClaimValue;
            await _context.SaveChangesAsync();
            StatusMessage = "User Claim Updated";
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? claimId)
        {
            if(claimId == null)
            {
                return NotFound();
            }
            userClaim = await _context.UserClaims.Where(c => c.Id == claimId).FirstOrDefaultAsync();
            if(userClaim == null)
            {
                return NotFound();
            }
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            await _userManager.RemoveClaimAsync(user, new Claim(userClaim.ClaimType, userClaim.ClaimValue));
            StatusMessage = "Remove Claim Successfully";
            return RedirectToPage("./AssignRole", new {id = user.Id});
        }
    }
}
