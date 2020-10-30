using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientShared.Middleware
{
    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthApiMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthApiMiddleware>();
        }

        public static IApplicationBuilder UseAuthClientMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthClientMiddleware>();
        }
    }
}
