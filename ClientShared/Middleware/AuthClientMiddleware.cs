using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClientShared.Middleware
{
    class AuthClientMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthClientMiddleware(RequestDelegate next, ILoggerFactory logFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _next = next;
            _logger = logFactory.CreateLogger("AuthMiddleware");
        }

        public Task Invoke(HttpContext httpContext)
        {
            var accessToken = _httpContextAccessor.HttpContext.GetTokenAsync("id_token").Result;
            if (accessToken != null)
            {
                var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

                if (_accessToken.Claims.Where(d => d.Type == "email").Select(d => d.Value).FirstOrDefault() == "BobSmith@email.com")
                {
                    _logger.LogInformation("User email authenticated");
                    if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
                    {
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Role, "Admin")
                        };

                        var appIdentity = new ClaimsIdentity(claims);
                        httpContext.User.AddIdentity(appIdentity);
                    }
                    return _next(httpContext);
                }
                else
                {
                    _logger.LogInformation("Invalid User Email");
                    httpContext.Response.StatusCode = 401; //UnAuthorized
                    //httpContext.Response.WriteAsync("Invalid User Email").Wait();


                    httpContext.SignOutAsync("Cookies").Wait();
                    var prop = new AuthenticationProperties()
                    {
                        RedirectUri = "/"
                    };
                    // after signout this will redirect to your provided target
                    httpContext.SignOutAsync("oidc", prop).Wait();



                    //httpContext.SignOutAsync().Wait();
                    //httpContext.Response.Redirect("/");
                    
                    return Task.CompletedTask;
                }
            }
            else
            {
                return _next(httpContext);
            }
        }
    }
}
