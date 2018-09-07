using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MongoDistributedCache
{
    public class MongoDistributedCache : IDistributedCache
    {
        private readonly TimeSpan _expiredRemovalInterval;
        private readonly MongoAccessor _mongoAccessor;
        private DateTimeOffset _lastRemoval = DateTime.UtcNow;

        private void deleteExpired()
        {
            var utcNow = DateTime.UtcNow;
            
            if(_lastRemoval.Add(_expiredRemovalInterval) < utcNow)
            {
                _mongoAccessor.DeleteMany(Builders<MongoCacheItem>.Filter.Eq(m => m.ExpiresAt, utcNow));
                _lastRemoval = utcNow;
            }
        }

        public MongoDistributedCache(IOptions<MongoDistributedCacheOptions> opts)
        {
            var options = opts.Value;

            _expiredRemovalInterval = options.ExpiredRemovalInterval.HasValue ? options.ExpiredRemovalInterval.Value : TimeSpan.FromMinutes(3);

            _mongoAccessor = new MongoAccessor(opts);
        }

        public byte[] Get(string key)
        {
            Refresh(key);
            var rval = _mongoAccessor.Get(key).Value;
            deleteExpired();
            return rval;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            await RefreshAsync(key, token);
            var rval = _mongoAccessor.GetAsync(key, token);
            deleteExpired();
            return (await rval).Value;
        }

        public void Refresh(string key)
        {
            var cacheItem = _mongoAccessor.Get(key);
            cacheItem.RefreshExpiresAt();
            _mongoAccessor.Upsert(key, cacheItem);
            deleteExpired();
        }

        public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var cacheItem = await _mongoAccessor.GetAsync(key, token);
            cacheItem.RefreshExpiresAt();
            await _mongoAccessor.UpsertAsync(key, cacheItem, token);
            deleteExpired();
        }

        public void Remove(string key)
        {
            _mongoAccessor.Delete(key);
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            await _mongoAccessor.DeleteAsync(key, token);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            _mongoAccessor.Upsert(key, new MongoCacheItem(key, value, options));
            deleteExpired();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            await _mongoAccessor.UpsertAsync(key, new MongoCacheItem(key, value, options), token);
            deleteExpired();
        }
    }
}
