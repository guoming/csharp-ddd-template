using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSharp.DDD.Template.Domain.Entities;

namespace CSharp.DDD.Template.Domain.Queries
{
    public interface ISubscriberConfigQuery
    {
        /// <summary>
        /// 根据消息渠道key获得记录
        /// </summary>
        /// <param name="message_channel_key">消息渠道key</param>
        /// <returns></returns>
        Task<SubscriberConfig> GetRecordInfoByMessageChannelKey(string message_channel_key);

        /// <summary>
        /// 根据订阅者用户令牌获得记录
        /// </summary>
        /// <param name="app_token">订阅者用户令牌</param>
        /// <returns></returns>
        Task<SubscriberConfig> GetRecordInfoByAppToken(string app_token);

        /// <summary>
        /// 根据消息渠道key获得记录（从Redis中取值）
        /// </summary>
        /// <param name="message_channel_key">消息渠道key</param>
        /// <returns></returns>
        SubscriberConfig GetRecordInfoRedisByMessageChannelKey(string message_channel_key);
    }
}
