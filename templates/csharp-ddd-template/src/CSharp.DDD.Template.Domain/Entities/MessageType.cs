using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp.DDD.Template.Domain.Entities
{
    /// <summary>
    /// 消息类型表
    /// </summary>
    public class MessageType
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 消息类型代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 消息类型名称
        /// </summary>
        public string name { get; set; }
    }
}
