using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSharp.DDD.Template.Domain.Entities;

namespace CSharp.DDD.Template.Domain.Queries
{
    public interface ISubscriberMessageTypeQuery
    {
        /// <summary>
        /// 获得记录
        /// </summary>
        /// <param name="subscriber_id">订阅者Id</param>
        /// <param name="message_type_id">消息类型Id</param>
        /// <returns></returns>
        Task<SubscriberMessageType> GetRecordInfo(long subscriber_id, long message_type_id);

        /// <summary>
        /// 获得记录（从Redis中取值）
        /// </summary>
        /// <param name="subscriber_id">订阅者Id</param>
        /// <param name="message_type_id">消息类型Id</param>
        /// <returns></returns>
        SubscriberMessageType GetRecordInfoByRedis(long subscriber_id, long message_type_id);
    }
}
