using System;
using System.Threading.Tasks;
using Hummingbird.Extensions.Cache;
using CSharp.DDD.Template.Domain.Core.Persistence;
using CSharp.DDD.Template.Domain.Entities;
using CSharp.DDD.Template.Domain.Queries;

namespace CSharp.DDD.Template.Infrastructure.Queries
{
    public class SubscriberConfigQuery : BaseQuery, ISubscriberConfigQuery
    {
        private static object _async = new object();
        private readonly IHummingbirdCache<object> _hummingbirdCache;

        public SubscriberConfigQuery(IDbContext dbContext, IHummingbirdCache<object> hummingbirdCache) : base(dbContext)
        {
            this._hummingbirdCache = hummingbirdCache;
        }

        public async Task<SubscriberConfig> GetRecordInfoByAppToken(string app_token)
        {
            string sql = "SELECT * FROM subscriber_config WHERE app_token = @app_token";
            return await this._dbContext.QueryFirstOrDefaultAsync<SubscriberConfig>(sql, new { app_token });
        }

        public async Task<SubscriberConfig> GetRecordInfoByMessageChannelKey(string message_channel_key)
        {
            string sql = "SELECT * FROM subscriber_config WHERE message_channel_key = @message_channel_key";
            return await this._dbContext.QueryFirstOrDefaultAsync<SubscriberConfig>(sql, new { message_channel_key });
        }


        public SubscriberConfig GetRecordInfoRedisByMessageChannelKey(string message_channel_key)
        {
            var redis_ksy = $"{typeof(SubscriberConfig).Name}_{message_channel_key}";
            var redis_region = typeof(SubscriberConfig).Name;

            var cacheObj = _hummingbirdCache.Get(redis_ksy, redis_region) as SubscriberConfig;
            if (cacheObj == null)
            {
                lock (_async)
                {
                    cacheObj = _hummingbirdCache.Get(redis_ksy, redis_region) as SubscriberConfig;
                    if (cacheObj == null)
                    {
                        cacheObj = GetRecordInfoByMessageChannelKey(message_channel_key).ConfigureAwait(false).GetAwaiter().GetResult();
                        if (cacheObj == null) cacheObj = new SubscriberConfig();
                        _hummingbirdCache.Add(redis_ksy, cacheObj, TimeSpan.FromMinutes(5), redis_region);
                    }
                }
            }
            return cacheObj;
        }
    }
}
