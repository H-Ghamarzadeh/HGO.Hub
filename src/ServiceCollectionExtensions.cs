using System.Reflection;
using HGO.Hub.Interfaces;
using HGO.Hub.Interfaces.Actions;
using HGO.Hub.Interfaces.Events;
using HGO.Hub.Interfaces.Filters;
using HGO.Hub.Interfaces.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace HGO.Hub
{
    /// <summary>
    /// Extensions to registers HGO.Hub services and handlers from the specified assemblies.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers HGO.Hub services and handlers from the specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration options</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddHgoHub(this IServiceCollection services, Action<HgoHubServiceConfiguration> configuration)
        {
            var serviceConfig = new HgoHubServiceConfiguration();

            configuration.Invoke(serviceConfig);

            return services.AddHgoHub(serviceConfig);
        }

        /// <summary>
        /// Registers HGO.Hub services and handlers from the specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration options</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddHgoHub(this IServiceCollection services, HgoHubServiceConfiguration configuration)
        {
            services.Add(new ServiceDescriptor(typeof(IHub), typeof(Hub), configuration.HubServiceLifetime));
            
            ScanAssembliesAndRegisterHandlers(services, configuration.HandlersDefaultLifetime,
                configuration.AssembliesToRegister);

            return services;
        }

        private static void ScanAssembliesAndRegisterHandlers(IServiceCollection services, ServiceLifetime lifetime, List<Assembly> assemblies)
        {
            foreach (var assembly in assemblies.Distinct())
            {
                var eventHandlerType = typeof(IEventHandler<>);
                var actionHandlerType = typeof(IActionHandler<>);
                var filterHandlerType = typeof(IFilterHandler<>);
                var requestHandlerType = typeof(IRequestHandler<,>);
                
                var eventHandlerTypes = assembly.DefinedTypes.Where(p => p.ImplementedInterfaces.Any(x=> x.GenericTypeArguments.Length > 0 && x.Name == eventHandlerType.Name && x.Namespace == eventHandlerType.Namespace && x.Module == eventHandlerType.Module));
                var actionHandlerTypes = assembly.DefinedTypes.Where(p => p.ImplementedInterfaces.Any(x=> x.GenericTypeArguments.Length > 0 && x.Name == actionHandlerType.Name && x.Namespace == actionHandlerType.Namespace && x.Module == actionHandlerType.Module));
                var filterHandlerTypes = assembly.DefinedTypes.Where(p => p.ImplementedInterfaces.Any(x => x.GenericTypeArguments.Length > 0 &&  x.Name == filterHandlerType.Name && x.Namespace == filterHandlerType.Namespace && x.Module == filterHandlerType.Module));
                var requestHandlerTypes = assembly.DefinedTypes.Where(p => p.ImplementedInterfaces.Any(x => x.GenericTypeArguments.Length > 0 &&  x.Name == requestHandlerType.Name && x.Namespace == requestHandlerType.Namespace && x.Module == requestHandlerType.Module));

                foreach (var handlerType in eventHandlerTypes)
                {
                    var serviceTypes = handlerType.ImplementedInterfaces.Distinct();
                    foreach (var serviceType in serviceTypes)
                    {
                        services.Add(new ServiceDescriptor(serviceType, handlerType, lifetime));
                    }
                }

                foreach (var handlerType in actionHandlerTypes)
                {
                    var serviceTypes = handlerType.ImplementedInterfaces.Distinct();
                    foreach (var serviceType in serviceTypes)
                    {
                        services.Add(new ServiceDescriptor(serviceType, handlerType, lifetime));
                    }
                }

                foreach (var handlerType in filterHandlerTypes)
                {
                    var serviceTypes = handlerType.ImplementedInterfaces.Distinct();
                    foreach (var serviceType in serviceTypes)
                    {
                        services.Add(new ServiceDescriptor(serviceType, handlerType, lifetime));
                    }
                }

                foreach (var handlerType in requestHandlerTypes)
                {
                    var serviceTypes = handlerType.ImplementedInterfaces.Distinct();
                    foreach (var serviceType in serviceTypes)
                    {
                        services.Add(new ServiceDescriptor(serviceType, handlerType, lifetime));
                    }
                }
            }
        }
    }
}
