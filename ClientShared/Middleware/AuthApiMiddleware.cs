using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ClientShared.Middleware
{
    public class AuthApiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthApiMiddleware(RequestDelegate next, ILoggerFactory logFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _next = next;
            _logger = logFactory.CreateLogger("AuthMiddleware");
        }

        public Task Invoke(HttpContext httpContext)
        {
            var accessToken = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            if (accessToken != null)
            {
                var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

                if (_accessToken.Claims.Where(d => d.Type == "email").Select(d => d.Value).FirstOrDefault() == "BobSmith@email.com")
                {
                    _logger.LogInformation("User email authenticated");
                    return _next(httpContext);
                }
            }

            httpContext.Response.StatusCode = 401; //UnAuthorized
            _logger.LogInformation("Invalid User Email");
            return Task.CompletedTask;
        }

    }

}
