using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ClientSite.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Net.Http.Headers;
using ClientShared.Middleware;
using ClientShared.AuthorizationPolicy;

namespace ClientSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpContextAccessor();
            services.AddSingleton<WeatherForecastService>();

            // add this
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "https://localhost:5001";
                options.ClientId = "blazor_client"; // 75 seconds
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.UsePkce = true;
                options.Scope.Add("api1");
                options.Scope.Add("idResource1");

                // for API add offline_access scope to get refresh_token

                options.GetClaimsFromUserInfoEndpoint = true;

                options.Events = new OpenIdConnectEvents
                {
                    // called if user clicks Cancel during login
                    OnAccessDenied = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/");
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder("AccessControl")
                    .RequireAuthenticatedUser()
                    .AddRequirements(new MinimumPermissionRequirement())
                    .Build();
            });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AccessControl", policy =>
            //    {
            //        policy.RequireAuthenticatedUser();
            //        policy.AddRequirements(new MinimumPermissionRequirement());
            //    });
            //});
            ////injection
            services.AddScoped<IAuthorizationHandler, MinimumPermissionClientHandler>();

            //create an HTTP client for accessing the API
            services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44387/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

            // add this
            // part 2: unnecessary, Blazor ignores it
            //services.AddMvcCore(options =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //        .RequireAuthenticatedUser() // site-wide auth
            //        .Build();
            //    options.Filters.Add(new AuthorizeFilter(policy));
            //});

            // part 2: add these
            services.AddSingleton<BlazorServerAuthStateCache>();
            services.AddScoped<AuthenticationStateProvider, BlazorServerAuthState>();
            services.AddScoped<IUsersService, UsersService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // add this
            app.UseAuthClientMiddleware();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();//.RequireAuthorization("AccessControl");
                endpoints.MapFallbackToPage("/_Host");//.RequireAuthorization("AccessControl");
            });
        }
    }
}
