
using Deanta.Cosmos.Infrastructure.ConcurrencyHandlers;
using GenericEventRunner.ForSetup;
using Microsoft.Extensions.DependencyInjection;

namespace Deanta.Cosmos.Infrastructure.AppStart
{
    public static class NetCoreDiSetupExtensions
    {
        public static void RegisterInfrastructureDi(this IServiceCollection services)
        {

            //This provides a SaveChangesExceptionHandler which handles concurrency issues around CommentsCount and CommentsAverageVotes
            var config = new GenericEventRunnerConfig
            {
                SaveChangesExceptionHandler = TodoWithEventsConcurrencyHandler.HandleCacheValuesConcurrency
            };
            //Because I haven't provided any assemblies this will scan this assembly for event handlers
            services.RegisterGenericEventRunner(config);
        }
    }
}