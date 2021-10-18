using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSharp.DDD.Template.Domain.Entities;

namespace CSharp.DDD.Template.Domain.Repositories
{
    public interface ISubscriberMessageTypeRepository
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<bool> AddBatch(List<SubscriberMessageType> entitys);

        /// <summary>
        /// 根据订阅者Id删除消息类型绑定数据
        /// </summary>
        /// <param name="subscriber_id"></param>
        /// <returns></returns>
        Task<bool> Delete(long subscriber_id);
    }
}
