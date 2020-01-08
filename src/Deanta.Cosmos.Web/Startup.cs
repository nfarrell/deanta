
using System;
using System.Reflection;
using Deanta.Cosmos.DataLayer.EfCode;
using Deanta.Cosmos.DataLayer.NoSqlCode;
using Deanta.Cosmos.DataLayerEvents.EfCode;
using Deanta.Cosmos.Infrastructure.AppStart;
using Deanta.Cosmos.Logger;
using Deanta.Cosmos.ServiceLayer.AppStart;
using Deanta.Cosmos.ServiceLayer.TodoSql.Dtos;
using Elastic.Apm.AspNetCore;
using GenericServices.Setup;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Deanta.Cosmos
{
    public enum StartupModes { NotSet, SqlOnly, SqlAndCosmosDb }

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
            services.AddAuthentication(options =>
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

            //dumb down password requirements
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddRazorPages();

            services.AddHttpContextAccessor();

            var sqlConnection = Configuration.GetConnectionString("TodoSqlConnection");
            services.AddDbContext<DeantaSqlDbContext>(options =>
                options.UseSqlServer(sqlConnection, sqlServerOptionsAction: sqlOptions => sqlOptions.EnableRetryOnFailure())
            );
            services.AddDbContext<SqlEventsDbContext>(options =>
                options.UseSqlServer(sqlConnection, sqlServerOptionsAction: sqlOptions => sqlOptions.EnableRetryOnFailure())
            );

            //This allows you to turn off the CosmosDb part if you are only interested in the 
            if (!Enum.TryParse<StartupModes>(Configuration["StartupMode"], out var startupMode))
                throw new InvalidOperationException("The appsettings.json property 'Mode' value wasn't valid.");

            if (startupMode == StartupModes.SqlAndCosmosDb)
            {
                //The user secrets provides an actual Azure Cosmos database. The takeUserSecrets flag controls this  
                var takeUserSecrets = false;
                var cosmosUtl = takeUserSecrets ? Configuration["CosmosUrl"] : Configuration["CosmosDbInfo:endpoint"];
                var cosmosKey = takeUserSecrets ? Configuration["CosmosKey"] : Configuration["CosmosDbInfo:authKey"];

                //Note you don't need to set an ExecutionStrategy on Cosmos provider 
                //see https://github.com/aspnet/EntityFrameworkCore/issues/8443#issuecomment-465836181

                services.AddDbContext<NoDeantaSqlDbContext>(options =>
                    options.UseCosmos(
                        cosmosUtl, cosmosKey, Configuration["CosmosDbInfo:database"]));

                //This registers the NoSqlTodoUpdater and persists to NoSQL
                services.AddScoped<ITodoUpdater, NoSqlTodoUpdater>();
            }
            else
            {
                //Setting this to null turns off all CosmosDb parts of the application
                services.AddScoped<NoDeantaSqlDbContext>(x => null);
                services.AddScoped<DbContextOptions<NoDeantaSqlDbContext>>(x => null);
                services.AddScoped<ITodoUpdater>(x => null);
            }

            //The other projects that need DI have their own extension methods to handle that
            services.RegisterInfrastructureDi();
            services.RegisterServiceLayerDi();

             //NOTE: must come after the call to RegisterInfrastructureDi which sets up the EventsRunner 
            services.ConfigureGenericServicesEntities(typeof(DeantaSqlDbContext), typeof(SqlEventsDbContext))
                .ScanAssemblesForDtos(Assembly.GetAssembly(typeof(TodoListDto)))
                .RegisterGenericServices();

            IdentityModelEventSource.ShowPII = true;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            app.UseElasticApm(Configuration);

            loggerFactory.AddProvider(new RequestTransientLogger(() => httpContextAccessor));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
