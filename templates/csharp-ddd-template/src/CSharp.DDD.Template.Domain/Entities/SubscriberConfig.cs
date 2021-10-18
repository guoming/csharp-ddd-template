using System;

namespace CSharp.DDD.Template.Domain.Entities
{
    /// <summary>
    /// 消费者订阅配置表
    /// </summary>
    public class SubscriberConfig
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 消息渠道key
        /// </summary>
        public string message_channel_key { get; set; }
        /// <summary>
        /// 订阅者用户令牌
        /// </summary>
        public string app_token { get; set; }
        /// <summary>
        /// 订阅者用户key
        /// </summary>
        public string app_key { get; set; }
        /// <summary>
        /// 订阅者Url
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime add_time { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime update_time { get; set; }
    }
}
