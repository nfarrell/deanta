
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Deanta.Cosmos
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static IHost BuildWebHost(string[] args)
        {
            var iHost = CreateHostBuilder(args)
                .Build();
            var currDir = Directory.GetCurrentDirectory();
            iHost.SetupDevelopmentDatabase(currDir);

            return iHost;
        }
    }
}
