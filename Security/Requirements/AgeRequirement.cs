using Microsoft.AspNetCore.Authorization;

namespace App.Security.Requirement
{
    public class AgeRequirement : IAuthorizationRequirement
    {
        public AgeRequirement()
        {
            
        }
        public int Age { get; set; }
    }
}