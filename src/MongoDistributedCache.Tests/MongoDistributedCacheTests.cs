using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using MongoDistributedCache.Tests.Fakes;
using Xunit;

namespace MongoDistributedCache.Tests
{
    public class MongoDistributedCacheTests
    {
        private MongoDistributedCacheOptions _options = new MongoDistributedCacheOptions {
            ExpiredRemovalInterval = TimeSpan.FromSeconds(1)
        };

        [Fact]
        public void Get_NonExistentKey_ReturnsNull()
        {
            var sot = new MongoDistributedCache(_options, new InMemoryMongoAccessor());

            var result = sot.Get("SomeNoneExistantKey");
            
            Assert.Null(result);
        }

        [Fact]
        public void SetAndGet_KeyAndValue_ReturnsSetValue()
        {
            var sot = new MongoDistributedCache(_options, new InMemoryMongoAccessor());

            var value = new byte[1];

            sot.Set("MyKey", value);

            var result = sot.Get("MyKey");

            Assert.Equal(value, result);
        }

        [Fact]
        public void SetAndGet_KeysAreCaseSensitive_ReturnsSetValue()
        {
            var sot = new MongoDistributedCache(_options, new InMemoryMongoAccessor());

            var value = new byte[1];

            sot.Set("MyKey", value);

            var result = sot.Get("mykey");

            Assert.Null(result);
        }

        [Fact]
        public void Set_OverwritesExitingValue_ReturnsNewValue()
        {
             var sot = new MongoDistributedCache(_options, new InMemoryMongoAccessor());

            var value = new byte[1];
            var value2 = new byte[2];

            sot.Set("MyKey", value);

            var result = sot.Get("MyKey");

            Assert.Equal(value, result);

            sot.Set("MyKey", value2);

            var result2 = sot.Get("MyKey");

            Assert.Equal(result2, value2);
            Assert.NotEqual(result, value2);
        }

        [Fact]
        public async Task SetAsync_OverwritesExistingValue_ReturnsNewValue()
        {
            var sot = new MongoDistributedCache(_options, new InMemoryMongoAccessor());

            var value = new byte[1];
            var value2 = new byte[2];

            await sot.SetAsync("MyKey", value);

            var result = sot.GetAsync("MyKey");

            Assert.Equal(value, await result);

            await sot.SetAsync("MyKey", value2);

            var result2 = sot.GetAsync("MyKey");

            Assert.Equal(await result2, value2);
            Assert.NotEqual(await result, value2);
        }

        [Fact]
        public void RemoveAsync_RemovesValue()
        {
            var sot = new MongoDistributedCache(_options, new InMemoryMongoAccessor());

            var value = new byte[1];

            sot.Set("MyKey", value);

            var result = sot.Get("MyKey");

            Assert.Equal(value, result);

            sot.RemoveAsync("MyKey").Wait();

            var result2 = sot.Get("MyKey");

            Assert.Null(result2);
        }

        [Fact]
        public void Remove_RemovesValue()
        {
            var sot = new MongoDistributedCache(_options, new InMemoryMongoAccessor());

            var value = new byte[1];

            sot.Set("MyKey", value);

            var result = sot.Get("MyKey");

            Assert.Equal(value, result);

            sot.Remove("MyKey");

            var result2 = sot.Get("MyKey");

            Assert.Null(result2);
        }

        [Fact]
        public void Set_WithAbsoluteExpiration_GetReturnsNullAfterExpirationTimeSpan()
        {
            var sot = new MongoDistributedCache(_options, new InMemoryMongoAccessor());

            var value = new byte[1];

            sot.Set("MyKey", value, new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(1)));

            var result = sot.Get("MyKey");

            Assert.Equal(result, value);

            Thread.Sleep(TimeSpan.FromSeconds(1.5));

            result = sot.Get("MyKey");

            Assert.Null(result);
        }

        [Fact]
        public void Set_WithSlidingExpiration_GetReturnsNullAfterSlidingExperationTimeSpan()
        {
            var sot = new MongoDistributedCache(_options, new InMemoryMongoAccessor());

            var value = new byte[1];

            sot.Set("MyKey", value, new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(1)));

            var result = sot.Get("MyKey");

            Assert.Equal(result, value);

            Thread.Sleep(TimeSpan.FromSeconds(1.5));

            result = sot.Get("MyKey");

            Assert.Null(result);
        }

        [Fact]
        public void Set_WithSlidingExpiration_ReturnsValueWhenAccessedRefreshingExpiresAt()
        {
            var sot = new MongoDistributedCache(_options, new InMemoryMongoAccessor());

            var value = new byte[1];

            sot.Set("MyKey", value, new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(1)));

            for (int i = 0; i < 4; i++)
            {
                var result1 = sot.Get("MyKey");
                Assert.Equal(value, result1);
                Thread.Sleep(TimeSpan.FromSeconds(.5));
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));

            var result2 = sot.Get("MyKey");

            Assert.Null(result2);
        }

        [Fact]
        public void Set_WithSlidingAndAbsoluteExpiration_ReturnsValueUntilAbsoluteExpiration()
        {
            var sot = new MongoDistributedCache(_options, new InMemoryMongoAccessor());

            var value = new byte[1];

            sot.Set("MyKey", value, new DistributedCacheEntryOptions()
                                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(3))
                                        .SetSlidingExpiration(TimeSpan.FromSeconds(1))
                                    );

            for (int i = 0; i < 6; i++)
            {
                var result1 = sot.Get("MyKey");
                Assert.Equal(value, result1);
                Thread.Sleep(TimeSpan.FromSeconds(.5));
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));

            var result2 = sot.Get("MyKey");

            Assert.Null(result2);
        }
    }
}