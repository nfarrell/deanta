using Deanta.UnitTests.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Deanta.Models.Contexts;
using Swashbuckle.AspNetCore.Swagger;
using Deanta.CommandAPI.Services;
using Microsoft.Extensions.Hosting;

namespace Deanta.CommandAPI
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
            services.AddMvc(x => x.EnableEndpointRouting = false);

            services.AddDbContext<DeantaContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Deanta CommandAPI",
                    Description = "Deanta Command API"
                });
            });

            services.AddTransient<IDeantaRepositoryService, DeantaRepositoryService>();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            var appSettings = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            if (appSettings.fakeDBMode) //NoopDB
            {
                var fakeContext = FakeDeantaContextFactory.CreateFakeDeantaContext();
                services.AddSingleton<IDeantaContext>(fakeContext);
            }
            else
            {
                services.AddTransient<IDeantaContext, DeantaContext>();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<IDeantaContext>() as DeantaContext;
                context?.Database.EnsureCreated(); //should possibly be done in release pipeline for horizontal scaling
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Boafy Command API V1");
            });
        }
    }
}
