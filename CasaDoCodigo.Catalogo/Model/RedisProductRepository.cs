using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Model
{
    public class RedisProductRepository
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisProductRepository(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }
    }
}
