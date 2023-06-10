using Microsoft.AspNetCore.Authorization;

namespace razor07.Security.Requirement
{
    public class AgeRequirement : IAuthorizationRequirement
    {
        public AgeRequirement()
        {
            
        }
        public int Age { get; set; }
    }
}