using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MoneyMCS.Policies
{

    public class UserTypeRequirement : IAuthorizationRequirement  
    {
        public UserTypeRequirement(string userType)
        {
            UserType = userType;
        }

        public string UserType { get; set; }
    }

    public class AgentTypeHandler : AuthorizationHandler<UserTypeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserTypeRequirement requirement)
        {
            string userType = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var redirectContext = context.Resource as HttpContext;
            if (userType == null || userType != "Agent")
            {
                redirectContext.Response.Redirect("/Login");
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

}