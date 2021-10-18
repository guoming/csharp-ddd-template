using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp.DDD.Template.Domain.Entities
{
    /// <summary>
    /// 订阅者订阅消息类型表
    /// </summary>
    public class SubscriberMessageType
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 订阅者Id
        /// </summary>
        public long subscriber_id { get; set; }
        /// <summary>
        /// 消息类型Id
        /// </summary>
        public long message_type_id { get; set; }
    }
}
