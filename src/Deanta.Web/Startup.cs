using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Deanta.Web.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation.AspNetCore;
using Deanta.Web.Models;
using Deanta.Web.Infrastructure.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using Elastic.Apm.AspNetCore;

namespace Deanta.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        private IWebHostEnvironment Environment { get; set; }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = Configuration["BoafyAuth:Authority"];
                options.RequireHttpsMetadata = true;
                options.ClientId = Configuration["BoafyAuth:ClientId"];
                options.ClientSecret = Configuration["BoafyAuth:ClientSecret"];
                options.ResponseType = "code id_token";

                options.Scope.Clear();
                foreach (var scope in new[] { "openid", "profile", "email", "roles" })
                {
                    options.Scope.Add(scope);
                }

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
                //options.Events = new OpenIdConnectEvents
                //{
                //    OnMessageReceived = OnMessageReceived,
                //    OnRedirectToIdentityProvider = OnRedirectToIdentityProvider
                //};
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")
                    ));

            services
                .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            if (Environment.IsDevelopment())
            {
                services
                .AddRazorPages()
                .AddRazorRuntimeCompilation()
                .AddFluentValidation();
            }
            else
            {
                services
                .AddRazorPages()
                .AddFluentValidation();
            }

            services.AddTransient<IValidator<ToDoCreateModel>, ToDoCreateValidator>();

            IdentityModelEventSource.ShowPII = true;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseElasticApm(Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            IdentityModelEventSource.ShowPII = true;
        }
    }
}
