using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToriGeneration.Core.Abstract.Generators;
using ToriGeneration.Core.Abstract.Services;
using ToriGeneration.Core.Abstract.Strategies;
using ToriGeneration.Core.Generators;
using ToriGeneration.Services;

namespace ToriGeneration.DependencyInjector
{
    public static class Injector
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {

            services.Add<ITorusGenerationService, TorusGenerationService>(serviceLifetime);

            services.Add<ITorusListGenerator, TorusListGenerator>(serviceLifetime);

            services.AddScoped<LinearTorusGenerator>();
            services.AddScoped<GammaTorusGenerator>();
            services.AddScoped<GaussTorusGenerator>();

            return services;
        }

        private static void Add<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(typeof(TService), typeof(TImplementation));
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(typeof(TService), typeof(TImplementation));
                    break;
                case ServiceLifetime.Scoped:
                default:
                    services.AddScoped(typeof(TService), typeof(TImplementation));
                    break;
            }
        }
    }
}
