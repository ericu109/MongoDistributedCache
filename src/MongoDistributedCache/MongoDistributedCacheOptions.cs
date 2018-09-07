using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDistributedCache
{
    public class MongoDistributedCacheOptions
    {
        /// <summary>
        /// The Database in Mongo to target.
        /// </summary>
        public string Database {get;set;}
        /// <summary>
        /// The Collection in Mongo to target.
        /// </summary>
        public string Collection {get;set;}
        /// <summary>
        /// The Hosts in the replica-set.
        /// </summary>
        public List<string> Hosts {get;set;}
        /// <summary>
        /// The Username required to connect to your replica-set.
        /// </summary>
        public string Username {get;set;}
        /// <summary>
        /// The Password required to connect to your replica-set.
        /// </summary>
        public string Password {get;set;}
        /// <summary>
        /// The interval to look for and remove expired cache entries.
        /// </summary>
        public TimeSpan? ExpiredRemovalInterval {get;set;}
        internal string GetConnectionString()
        {
            var sb = new StringBuilder();

            sb.Append("mongodb://");

            if(!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                sb.Append($"{Username}:{Password}@");
            }

            sb.Append(string.Join(",", Hosts));

            sb.Append($"/{Database}");

            return sb.ToString();
        }
    }
}