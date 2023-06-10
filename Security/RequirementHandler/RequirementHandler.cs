using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using App.Models;
using App.Security.Requirement;

namespace App.Security.RequirementHandler
{
    public class RequirementHandler : IAuthorizationHandler
    {
        private readonly ILogger<RequirementHandler> _logger;
        private readonly UserManager<AppUser> _userManager;

        public RequirementHandler(ILogger<RequirementHandler> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();
            foreach (var r in requirements)
            {
                //xu ly tuy theo requirement
                if(r is AgeRequirement)
                {
                    if(IsOverEightTeen(context.User, (AgeRequirement)r))
                    {
                        context.Succeed(r);
                    }
                }

                if(r is BlogUpdateRequirement)
                {
                    if(IsUpdatable(context.User, context.Resource, (BlogUpdateRequirement)r).Result)
                    {
                        context.Succeed(r);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private async Task<bool> IsUpdatable(ClaimsPrincipal user, object resource, BlogUpdateRequirement r)
        {
            var appUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser = appUserTask.Result;
            var userClaim = await _userManager.GetClaimsAsync(appUser);
            var result = userClaim.Any(c => c.Type == "ManageBlog" && c.Value == "Update");
            System.Console.WriteLine(result);
            return result;
        }

        private bool IsOverEightTeen(ClaimsPrincipal user, AgeRequirement r)
        {
            var appUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser = appUserTask.Result;
            if(appUser.Birthday == null)
            {
                _logger.LogInformation("User has not fill Birthday yet");
                return false;
            } 
            r.Age = DateTime.Now.Year - appUser.Birthday.Value.Year;
            var result = r.Age >= 18;
            if(result)
            {
                _logger.LogInformation($"User meets the requirement {r.Age}. Alow Access");
                
            }
            else{
                _logger.LogInformation("User not pass the requirement. Access Denied");
            }
            return result;

        }
    }
}