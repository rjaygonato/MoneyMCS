using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MoneyMCS.Areas.Identity.Data;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace MoneyMCS.Policies
{

    public class AgentTypeRequirement : IAuthorizationRequirement  
    {
        public AgentTypeRequirement(string userType)
        {
            UserType = userType;
        }

        public string UserType { get; set; }
    }

    public class MemberTypeRequirement : IAuthorizationRequirement
    {
        public MemberTypeRequirement(List<string> userTypes)
        {
            UserTypes = userTypes;
        }

        public List<string> UserTypes { get; set; }
    }


    public class AgentTypeHandler : AuthorizationHandler<AgentTypeRequirement>
    {
      
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AgentTypeRequirement requirement)
        {
            string userType = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var redirectContext = context.Resource as HttpContext;
            if (userType == null)
            {
                redirectContext.Response.Redirect("/Login");
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            else
            {
                if (userType != requirement.UserType)
                {
                    redirectContext.Response.Redirect("/Member/Index");
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class MemberTypeHandler : AuthorizationHandler<MemberTypeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MemberTypeRequirement requirement)
        {
            string userType = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var redirectContext = context.Resource as HttpContext;

            if (userType == null)
            {
                redirectContext.Response.Redirect("/Member/Login");
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            else
            {
                if (!requirement.UserTypes.Contains(userType))
                {
                    redirectContext.Response.Redirect("/Index");
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

}