using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hummingbird.Extensions.Cache;
using CSharp.DDD.Template.Domain.Core.Persistence;
using CSharp.DDD.Template.Domain.Entities;
using CSharp.DDD.Template.Domain.Queries;

namespace CSharp.DDD.Template.Infrastructure.Queries
{
    public class MessageTypeQuery : BaseQuery, IMessageTypeQuery
    {
        private static object _async = new object();
        private readonly IHummingbirdCache<object> _hummingbirdCache;

        public MessageTypeQuery(IDbContext dbContext, IHummingbirdCache<object> hummingbirdCache) : base(dbContext)
        {
            this._hummingbirdCache = hummingbirdCache;
        }

        public async Task<List<MessageType>> GetRecordInfoByCodes(List<string> code)
        {
            string where = " WHERE 1 = 1";
            if (code?.Count > 0)
            {
                where = "WHERE code IN @code";
            }

            string sql = $"SELECT * FROM message_type {where}";
            var result = await this._dbContext.QueryAsync<MessageType>(sql, new { code });
            return result.ToList();
        }

        public List<MessageType> GetRecordInfoRedisByCodes(List<string> code)
        {
            var redis_ksy = $"{typeof(MessageType).Name}_All";
            var redis_region = typeof(MessageType).Name;

            var cacheObj = _hummingbirdCache.Get(redis_ksy, redis_region) as List<MessageType>;
            if (cacheObj == null)
            {
                lock (_async)
                {
                    cacheObj = _hummingbirdCache.Get(redis_ksy, redis_region) as List<MessageType>;
                    if (cacheObj == null)
                    {
                        cacheObj = GetRecordInfoByCodes(null).ConfigureAwait(false).GetAwaiter().GetResult();
                        if (cacheObj == null) cacheObj = new List<MessageType>();
                        _hummingbirdCache.Add(redis_ksy, cacheObj, TimeSpan.FromMinutes(5), redis_region);
                    }
                }
            }
            if (code?.Count > 0)
            {
                return cacheObj.Where(p => code.Contains(p.code)).ToList();
            }
            return cacheObj;
        }
    }
}
