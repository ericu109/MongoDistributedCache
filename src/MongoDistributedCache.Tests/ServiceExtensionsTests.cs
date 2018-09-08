using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDistributedCache.Tests.Fakes;
using Xunit;

namespace MongoDistributedCache.Tests
{
    public class ServiceExtensionsTests
    {
        private MongoDistributedCacheOptions validMongoDistributedCacheOptions => new MongoDistributedCacheOptions{
            Username = "none",
            Password = "none",
            Database = "none",
            Collection = "none",
            Hosts = new List<string>{"host"}
        };

        [Fact]
        public void AddMongoDistributedCache_MongoDistributedCacheOptions_AreRegistered()
        {
            var service = new ServiceCollection();

            service.AddMongoDistributedCache(validMongoDistributedCacheOptions);

            var serviceProvider = service.BuildServiceProvider();

            var options = serviceProvider.GetService<IOptions<MongoDistributedCacheOptions>>().Value;

            Assert.Equal(validMongoDistributedCacheOptions.Username, options.Username);
            Assert.Equal(validMongoDistributedCacheOptions.Password, options.Password);
            Assert.Equal(validMongoDistributedCacheOptions.Database, options.Database);
            Assert.Equal(validMongoDistributedCacheOptions.Collection, options.Collection);
            Assert.Equal(validMongoDistributedCacheOptions.Hosts, options.Hosts);
            Assert.Equal(validMongoDistributedCacheOptions.ExpiredRemovalInterval, options.ExpiredRemovalInterval);
        }

        [Fact]
        public void AddMongoDistributedCache_RegistersDistributedCache()
        {
            var services = new ServiceCollection();
            
            services.AddMongoDistributedCache(validMongoDistributedCacheOptions);

            var cache = services.FirstOrDefault(m => m.ServiceType == typeof(IDistributedCache));

            Assert.NotNull(cache);
        }

        [Fact]
        public void AddMongoDistributedCache_RegistersDistributedCacheAsSingleton()
        {
            var services = new ServiceCollection();

            services.AddMongoDistributedCache(validMongoDistributedCacheOptions);

            var cache = services.FirstOrDefault(m => m.ServiceType == typeof(IDistributedCache));

            Assert.NotNull(cache);
            Assert.Equal(ServiceLifetime.Singleton, cache.Lifetime);
        }

        [Fact]
        public void AddMongoDistributedCache_ReplaceUserRegisteredDistributedCache_ThrowsInvalidOperationException()
        {
            var services = new ServiceCollection();
            services.AddScoped<IDistributedCache, FakeDistibutedCache>();

            Action action = () => services.AddMongoDistributedCache(validMongoDistributedCacheOptions);

            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public void AddMongoDistributedCache_RegisterWithNoDatabase_ThrowsArgumentException()
        {
            var services = new ServiceCollection();

            Assert.Throws<ArgumentException>(() => services.AddMongoDistributedCache(new MongoDistributedCacheOptions{
                Username = "none",
                Password = "none",
                Collection = "none",
                Hosts = new List<string>{"host"}
            }));
        }

        [Fact]
        public void AddMongoDistributedCache_RegisterWithNoCollection_ThrowsArgumentException()
        {
            var services = new ServiceCollection();

            Assert.Throws<ArgumentException>(() => services.AddMongoDistributedCache(new MongoDistributedCacheOptions{
                Username = "none",
                Password = "none",
                Database = "none",
                Hosts = new List<string>{"host"}
            }));
        }

        [Fact]
        public void AddMongoDistributedCache_RegisterWithNoHosts_ThrowsArgumentException()
        {
            var services = new ServiceCollection();

            Assert.Throws<ArgumentException>(() => services.AddMongoDistributedCache(new MongoDistributedCacheOptions{
                Username = "none",
                Password = "none",
                Database = "none",
                Collection = "none"
            }));
        }
    }
}
