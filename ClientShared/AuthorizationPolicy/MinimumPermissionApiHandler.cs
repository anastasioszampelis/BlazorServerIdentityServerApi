using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClientShared.AuthorizationPolicy
{
    public class MinimumPermissionApiHandler : AuthorizationHandler<MinimumPermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumPermissionRequirement requirement)
        {
            //if (!(context.Resource is Endpoint endpoint))
            //{
            //    context.Fail();
            //    return Task.CompletedTask;
            //}

            //check if token has subjectId
            //var subClaim = context.User?.Claims?.FirstOrDefault(c => c.Type == "sub");
            //if (subClaim == null)
            //{
            //    context.Fail();
            //    return Task.CompletedTask;
            //}

            //check if token is expired
            //var exp = context.User.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            //if (exp == null || new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(long.Parse(exp)).ToLocalTime() < DateTime.Now)
            //{
            //    context.Fail();
            //    return Task.CompletedTask;
            //}

            //other checkpoints
            //your db functions to check if user has desired claims
            if (context.User.Claims.Where(d => d.Type == ClaimTypes.Email).Select(d => d.Value).FirstOrDefault() != "BobSmith@email.com")
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
