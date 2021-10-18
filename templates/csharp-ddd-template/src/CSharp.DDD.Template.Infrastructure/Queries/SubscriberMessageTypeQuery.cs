using System;
using System.Threading.Tasks;
using Hummingbird.Extensions.Cache;
using CSharp.DDD.Template.Domain.Core.Persistence;
using CSharp.DDD.Template.Domain.Entities;
using CSharp.DDD.Template.Domain.Queries;

namespace CSharp.DDD.Template.Infrastructure.Queries
{
    public class SubscriberMessageTypeQuery : BaseQuery, ISubscriberMessageTypeQuery
    {
        private static object _async = new object();
        private readonly IHummingbirdCache<object> _hummingbirdCache;

        public SubscriberMessageTypeQuery(IDbContext dbContext, IHummingbirdCache<object> hummingbirdCache) : base(dbContext)
        {
            this._hummingbirdCache = hummingbirdCache;
        }

        public async Task<SubscriberMessageType> GetRecordInfo(long subscriber_id, long message_type_id)
        {
            string sql = "SELECT * FROM subscriber_message_type WHERE subscriber_id = @subscriber_id AND message_type_id = @message_type_id";
            return await this._dbContext.QueryFirstOrDefaultAsync<SubscriberMessageType>(sql, new { message_type_id, subscriber_id }); ;
        }

        public SubscriberMessageType GetRecordInfoByRedis(long subscriber_id, long message_type_id)
        {
            var redis_ksy = $"{typeof(SubscriberMessageType).Name}_{subscriber_id}_{message_type_id}";
            var redis_region = typeof(SubscriberMessageType).Name;

            var cacheObj = _hummingbirdCache.Get(redis_ksy, redis_region) as SubscriberMessageType;
            if (cacheObj == null)
            {
                lock (_async)
                {
                    cacheObj = _hummingbirdCache.Get(redis_ksy, redis_region) as SubscriberMessageType;
                    if (cacheObj == null)
                    {
                        cacheObj = GetRecordInfo(subscriber_id, message_type_id).ConfigureAwait(false).GetAwaiter().GetResult();
                        if (cacheObj == null) cacheObj = new SubscriberMessageType();
                        _hummingbirdCache.Add(redis_ksy, cacheObj, TimeSpan.FromMinutes(5), redis_region);
                    }
                }
            }
            return cacheObj;
        }
    }
}
