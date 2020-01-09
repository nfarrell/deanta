using System;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Deanta.CommandAPI.Client;
using Deanta.Models.Contexts;
using Deanta.Models.Mapping;
using Deanta.QueryAPI.Client;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;

namespace Deanta.ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        private IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(x => x.EnableEndpointRouting = false);

            services.AddDbContext<DeantaContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            services.AddHttpClient("DeantaQueryAPI", c =>
            {
                c.BaseAddress = new Uri(appSettingsSection["DeantaQueryAPIUrl"]);
                //c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                //c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            });

            services.ConfigureSwaggerGen(options =>
            {
                options.CustomSchemaIds(x => x.FullName);
            });

            services.AddHttpClient("DeantaCommandAPI", c =>
            {
                c.BaseAddress = new Uri(appSettingsSection["DeantaCommandAPIUrl"]);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Deanta ApiGateway",
                    Description = "Deanta API Gateway"
                });
            });

            services.AddTransient<IDeantaQueryApiClientService, DeantaQueryApiClientService>();

            services.AddTransient<IDeantaCommandApiClientService, DeantaCommandApiClientService>();

            services.AddAutoMapper(typeof(DeantaProfile).GetTypeInfo().Assembly);
            /*var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DeantaProfile());
            });

            var mapper = config.CreateMapper();
            services.AddSingleton<IMapper>(s => mapper);*/
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Boafy API Gateway V1");
            });
        }
    }
}
