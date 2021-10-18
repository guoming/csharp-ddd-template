using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSharp.DDD.Template.Domain.Entities;

namespace CSharp.DDD.Template.Domain.Repositories
{
    public interface ISubscriberConfigRepository
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="messageTypes"></param>
        /// <param name="insertMessageTypes"></param>
        /// <returns></returns>
        Task<SubscriberConfig> Add(SubscriberConfig entity, List<MessageType> messageTypes, List<MessageType> insertMessageTypes);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="messageTypes"></param>
        /// <param name="insertMessageTypes"></param>
        /// <returns></returns>
        Task<bool> Update(SubscriberConfig entity, List<MessageType> messageTypes, List<MessageType> insertMessageTypes);
    }
}
