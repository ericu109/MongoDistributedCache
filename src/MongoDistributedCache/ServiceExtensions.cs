using System;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace MongoDistributedCache
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMongoDistributedCache(this IServiceCollection services, MongoDistributedCacheOptions options)
        {
            if(services == null) throw new ArgumentNullException(nameof(services));

            ensureValidOptions(options);

            var currentlyRegisteredDistributedCache = services.FirstOrDefault(m => m.ServiceType == typeof(IDistributedCache));
            if(currentlyRegisteredDistributedCache != null)
                throw new InvalidOperationException($"Registration of a MongoDistributedCache would replace another registered cache of type {currentlyRegisteredDistributedCache.ServiceType.FullName}!");

            services.AddOptions();
            services.Configure<MongoDistributedCacheOptions>(m => {
                m.Username = options.Username;
                m.Password = options.Password;
                m.Database = options.Database;
                m.Collection = options.Collection;
                m.ExpiredRemovalInterval = options.ExpiredRemovalInterval;
                m.Hosts = options.Hosts;
            });
            services.AddSingleton<IMongoAccessor, MongoAccessor>();
            services.AddSingleton<IDistributedCache, MongoDistributedCache>();

            return services;
        }

        private static void ensureValidOptions(MongoDistributedCacheOptions options)
        {
            if(string.IsNullOrEmpty(options.Collection))
            {
                throw new ArgumentException($"{nameof(options.Collection)} is null or empty!");
            }

            if(string.IsNullOrEmpty(options.Database))
            {
                throw new ArgumentException($"{nameof(options.Database)} is null or empty!");
            }

            if(options.Hosts == null || options.Hosts.Count < 1)
            {
                throw new ArgumentException($"{nameof(options.Hosts)} must have at least one host!");
            }
        }
    }
}