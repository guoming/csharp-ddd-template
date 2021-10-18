using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSharp.DDD.Template.Domain.Entities;

namespace CSharp.DDD.Template.Domain.Queries
{
    public interface IMessageTypeQuery
    {
        /// <summary>
        /// 根据消息类型代码获得记录
        /// </summary>
        /// <param name="code">消息类型代码</param>
        /// <returns></returns>
        Task<List<MessageType>> GetRecordInfoByCodes(List<string> code);

        /// <summary>
        /// 根据消息类型代码获得记录（从Redis中取值）
        /// </summary>
        /// <param name="code">消息类型代码</param>
        /// <returns></returns>
        List<MessageType> GetRecordInfoRedisByCodes(List<string> code);
    }
}
