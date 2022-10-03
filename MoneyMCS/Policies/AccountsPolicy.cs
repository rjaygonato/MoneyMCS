using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MoneyMCS.Policies
{

    public class UserTypeRequirement : IAuthorizationRequirement  
    {
        public UserTypeRequirement(List<string> userTypes)
        {
            UserTypes = userTypes;
        }

        public List<string> UserTypes { get; set; }
    }

    public class AgentTypeHandler : AuthorizationHandler<UserTypeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserTypeRequirement requirement)
        {
            string userType = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var redirectContext = context.Resource as HttpContext;
            if (userType == null || !requirement.UserTypes.Contains(userType))
            {
                redirectContext.Response.Redirect("/Login");
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class MemberTypeHandler : AuthorizationHandler<UserTypeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserTypeRequirement requirement)
        {
            string userType = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var redirectContext = context.Resource as HttpContext;
            if (userType == null || !requirement.UserTypes.Contains(userType))
            {
                redirectContext.Response.Redirect("/Member/Login");
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

}