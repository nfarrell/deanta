﻿
using System.Collections.Generic;
using System.Reflection;
using GenericEventRunner.ForHandlers;
using GenericEventRunner.ForSetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TestSupport.EfHelpers;

namespace Deanta.Cosmos.Test.Helpers
{
    public static class SetupToTestEvents
    {

        /// <summary>
        /// This extension method provides a way to set up a DbContext with an EventsRunner and also registers all
        /// the event handlers in the assembly that the TRunner class is in. 
        /// </summary>
        /// <typeparam name="TContext">Your DbContext type</typeparam>
        /// <typeparam name="TRunner">The type of one of your event handlers.
        /// The whole assembly that the TRunner is in will be scanned for event handlers</typeparam>
        /// <param name="options">The <code>T:DbContextOptions{TContext}</code> for your DbContext</param>
        /// <param name="logs">Optional. If provided the it uses the EfCore.TestSupport logging provider to return logs</param>
        /// <param name="config">Optional. Allows you to change the configuration setting for GenericEventRunner</param>
        /// <returns>An instance of the DbContext created by DI and therefore containing the EventsRunner</returns>
        public static TContext CreateDbWithDiForHandlers<TContext, TRunner>(this DbContextOptions<TContext> options,
            List<LogOutput> logs = null, IGenericEventRunnerConfig config = null) where TContext : DbContext where TRunner : class
        {
            var services = new ServiceCollection();
            if (logs != null)
            {
                services.AddSingleton<ILogger<EventsRunner>>(new Logger<EventsRunner>(new LoggerFactory(new[] { new MyLoggerProvider(logs) })));
            }
            else
            {
                services.AddSingleton<ILogger<EventsRunner>>(new NullLogger<EventsRunner>());
            }

            var assembliesToScan = new Assembly[]
            {
                Assembly.GetAssembly(typeof(TRunner)),
                Assembly.GetExecutingAssembly()         //This will pick up any event handlers in your unit tests assembly
            };

            services.RegisterGenericEventRunner(config ?? new GenericEventRunnerConfig(), assembliesToScan);
            services.AddSingleton(options);
            services.AddScoped<TContext>();
            var serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<TContext>();
            return context;
        }
    }
}