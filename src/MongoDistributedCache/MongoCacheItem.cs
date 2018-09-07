using System;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDistributedCache
{
    public class MongoCacheItem
    {
        [BsonId]
        public string Key {get; set;}

        [BsonElement("Value")]
        public byte[] Value {get;set;}

        [BsonElement("ExpiresAt")]
        public DateTimeOffset? ExpiresAt {get;set;}

        [BsonElement("SlidingExpirationSeconds")]
        public double? SlidingExpirationSeconds {get;set;}

        [BsonElement("AbsoluteExpiration")]
        public DateTimeOffset? AbsoluteExpiration {get;set;}

        public MongoCacheItem(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var utcNow = DateTime.UtcNow;

            Key = key;
            Value = value;
            
            AbsoluteExpiration = getAbsoluteExpiration(options, utcNow);
            SlidingExpirationSeconds = options.SlidingExpiration?.TotalSeconds;
            ExpiresAt = getExpiresAt(utcNow, options?.SlidingExpiration, AbsoluteExpiration);
        }

        public void RefreshExpiresAt()
        {
            var utcNow = DateTime.UtcNow;

            ExpiresAt = getExpiresAt(utcNow, TimeSpan.FromSeconds(SlidingExpirationSeconds.GetValueOrDefault()), AbsoluteExpiration);
        }

        private DateTimeOffset? getExpiresAt(DateTimeOffset now, TimeSpan? slidingExpiration, DateTimeOffset? absoluteExpiration)
        {
            if(slidingExpiration == null && absoluteExpiration == null) return null;
            if(slidingExpiration == null && absoluteExpiration != null) return absoluteExpiration;

            var slidingExperationAt = now.Add(slidingExpiration.GetValueOrDefault());

            if(slidingExperationAt > absoluteExpiration)
            {
                return absoluteExpiration;
            }
            else 
            {
                return slidingExperationAt;
            }
        }

        private DateTimeOffset? getAbsoluteExpiration(DistributedCacheEntryOptions options, DateTimeOffset now)
        {
            var rval = options?.AbsoluteExpiration;

            if(options?.AbsoluteExpirationRelativeToNow != null)
            {
                rval = now.Add(options.AbsoluteExpirationRelativeToNow.Value);
            }

            if(rval < now)
            {
                throw new InvalidOperationException($"Absolute expiration must be in the future! Now is \"{now}\", AbsoluteExpiration is \"{rval}\"");
            }

            return rval;
        }
    }
}