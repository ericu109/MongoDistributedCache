using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MongoDistributedCache
{
    public interface IMongoAccessor
    {
        MongoCacheItem Get(string key);
        Task<MongoCacheItem> GetAsync(string key, CancellationToken token);
        void Upsert(string key, MongoCacheItem cacheItem);
        Task UpsertAsync(string key, MongoCacheItem cacheItem, CancellationToken token);
        void Delete(string key);
        Task DeleteAsync(string key, CancellationToken token);
        void DeleteMany(Expression<Func<MongoCacheItem, bool>> filter);
    }
}