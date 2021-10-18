using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSharp.DDD.Template.Domain.Entities;

namespace CSharp.DDD.Template.Domain.Repositories
{
    public interface IMessageTypeRepository
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<MessageType> Add(MessageType entity);
    }
}
