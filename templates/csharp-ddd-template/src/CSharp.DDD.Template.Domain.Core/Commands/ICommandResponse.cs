using System;
namespace CSharp.DDD.Template.Domain.Core.Commands
{
    public interface ICommandResponse
    {
        /// <summary>
        /// 接口业务状态
        /// </summary>
        string code { get; set; }

        /// <summary>
        /// 消息状态说明
        /// </summary>
        string message { get; set; }
    }

}
