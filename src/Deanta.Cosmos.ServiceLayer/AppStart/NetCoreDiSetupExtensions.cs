
using Deanta.Cosmos.ServiceLayer.DatabaseServices.Concrete;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;

namespace Deanta.Cosmos.ServiceLayer.AppStart
{
    public static class NetCoreDiSetupExtensions
    {
        public static void RegisterServiceLayerDi(this IServiceCollection services)
        {
            //Auto register some items
            services.RegisterAssemblyPublicNonGenericClasses()
                .Where(c => c.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces();

            //Hand register classes that don't end in Service
            services.AddTransient<TodoGenerator>();
        }
    }
}