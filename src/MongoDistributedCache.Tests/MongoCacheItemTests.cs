using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using MongoDistributedCache.Tests.Fakes;
using Xunit;

namespace MongoDistributedCache.Tests
{
    public class MongoCacheItemTests
    {
        [Fact]
        public void Constructor_SetsKeyAndValue()
        {
            var value = new byte[1];
            var sot = new MongoCacheItem("MyKey", value, new DistributedCacheEntryOptions());

            Assert.Equal("MyKey", sot.Key);
            Assert.Equal(value, sot.Value);
        }
    }
}