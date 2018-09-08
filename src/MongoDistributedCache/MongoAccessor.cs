using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MongoDistributedCache
{
    public class MongoAccessor : IMongoAccessor
    {
        private readonly IMongoCollection<MongoCacheItem> _mongoCollection;

        public MongoAccessor(IOptions<MongoDistributedCacheOptions> options)
        {
            var opts = options.Value;
            var client = new MongoClient(opts.GetConnectionString());
            var database = client.GetDatabase(opts.Database);

            _mongoCollection = database.GetCollection<MongoCacheItem>(opts.Collection);
        }

        private IFindFluent<MongoCacheItem, MongoCacheItem> getQuery(string key)
        {
            return _mongoCollection.Find(m => m.Key == key);
        }

        public MongoCacheItem Get(string key)
        {
            return getQuery(key).FirstOrDefault();
        }

        public Task<MongoCacheItem> GetAsync(string key, CancellationToken token)
        {
            return getQuery(key).FirstOrDefaultAsync(token);
        }

        public void Upsert(string key, MongoCacheItem cacheItem)
        {
            _mongoCollection.ReplaceOne(getQuery(key).Filter, cacheItem, new UpdateOptions{IsUpsert = true});
        }

        public Task UpsertAsync(string key, MongoCacheItem cacheItem, CancellationToken token)
        {
            return _mongoCollection.ReplaceOneAsync(getQuery(key).Filter, cacheItem, new UpdateOptions{IsUpsert = true}, token);
        }

        public void Delete(string key)
        {
            _mongoCollection.DeleteMany(getQuery(key).Filter);
        }
        public Task DeleteAsync(string key, CancellationToken token)
        {
            return _mongoCollection.DeleteManyAsync(getQuery(key).Filter, token);
        }

        public void DeleteMany(Expression<Func<MongoCacheItem, bool>> filter)
        {
            _mongoCollection.DeleteMany(filter);
        }
    }
}