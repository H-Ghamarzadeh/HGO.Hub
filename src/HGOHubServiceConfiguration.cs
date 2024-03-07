using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HGO.Hub
{
    /// <summary>
    /// Configuration for Hgo.Hub service
    /// </summary>
    public class HgoHubServiceConfiguration
    {
        /// <summary>
        /// Service lifetime to register handlers. Default value is <see cref="ServiceLifetime.Transient"/>
        /// </summary>
        public ServiceLifetime HandlersDefaultLifetime { get; set; } = ServiceLifetime.Transient;
        internal List<Assembly> AssembliesToRegister { get; set; } = new List<Assembly>();

        /// <summary>
        /// Register various handlers from assembly containing given type
        /// </summary>
        /// <typeparam name="T">Type from assembly to scan</typeparam>
        /// <returns>This</returns>
        public HgoHubServiceConfiguration RegisterServicesFromAssemblyContaining<T>()
            => RegisterServicesFromAssemblyContaining(typeof(T));

        /// <summary>
        /// Register various handlers from assembly containing given type
        /// </summary>
        /// <param name="type">Type from assembly to scan</param>
        /// <returns>This</returns>
        public HgoHubServiceConfiguration RegisterServicesFromAssemblyContaining(Type type)
            => RegisterServicesFromAssembly(type.Assembly);

        /// <summary>
        /// Register various handlers from assembly
        /// </summary>
        /// <param name="assembly">Assembly to scan</param>
        /// <returns>This</returns>
        public HgoHubServiceConfiguration RegisterServicesFromAssembly(Assembly assembly)
        {
            AssembliesToRegister.Add(assembly);

            return this;
        }

        /// <summary>
        /// Register various handlers from assemblies
        /// </summary>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns>This</returns>
        public HgoHubServiceConfiguration RegisterServicesFromAssemblies(
            params Assembly[] assemblies)
        {
            AssembliesToRegister.AddRange(assemblies);

            return this;
        }
    }
}
