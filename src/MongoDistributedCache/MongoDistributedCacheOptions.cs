using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDistributedCache
{
    public class MongoDistributedCacheOptions
    {
        public string Database {get;set;}
        public string Collection {get;set;}
        public List<string> Hosts {get;set;}
        public string UserName {get;set;}
        public string Password {get;set;}
        public TimeSpan? ExpiredRemovalInterval {get;set;}
        internal string GetConnectionString()
        {
            var sb = new StringBuilder();

            sb.Append("mongodb://");

            if(!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                sb.Append($"{UserName}:{Password}@");
            }

            sb.Append(string.Join(",", Hosts));

            sb.Append($"/{Database}");

            return sb.ToString();
        }
    }
}