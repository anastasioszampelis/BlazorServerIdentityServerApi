using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientApi.Repositories;
using ClientShared.AuthorizationPolicy;
using ClientShared.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ClientApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddControllers();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
               .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:5001";

                    //options.RequireHttpsMetadata = false;

                    options.Audience = "api1";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };

                });

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("https://localhost:44312/")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AccessControl", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new MinimumPermissionRequirement());
                });
            });
            //injection
            services.AddScoped<IAuthorizationHandler, MinimumPermissionApiHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("default");

            app.UseAuthentication();
            app.UseAuthApiMiddleware();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization("AccessControl");
            });
        }
    }
}
