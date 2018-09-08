using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MongoDistributedCache.Tests.Fakes
{
    public class InMemoryMongoAccessor : IMongoAccessor
    {
        private List<MongoCacheItem> _data = new List<MongoCacheItem>();
        public void Delete(string key)
        {
            _data.RemoveAll(m => m.Key == key);
        }

        public Task DeleteAsync(string key, CancellationToken token)
        {
            return Task.Run(() => {
                Delete(key);
            });
        }

        public void DeleteMany(Expression<Func<MongoCacheItem, bool>> filter)
        {
            var compiledFilter = filter.Compile();

            _data.RemoveAll(m => compiledFilter(m));
        }

        public void Upsert(string key, MongoCacheItem cacheItem)
        {
            Delete(key);
            _data.Add(cacheItem);
        }

        public Task UpsertAsync(string key, MongoCacheItem cacheItem, CancellationToken token)
        {
            return Task.Run(() => {
                Upsert(key, cacheItem);
            });
        }

        MongoCacheItem IMongoAccessor.Get(string key)
        {
            return _data.FirstOrDefault(m => m.Key == key);
        }

        Task<MongoCacheItem> IMongoAccessor.GetAsync(string key, CancellationToken token)
        {
            return Task.Run<MongoCacheItem>(() => {
                return _data.FirstOrDefault(m => m.Key == key);
            });
        }
    }
}