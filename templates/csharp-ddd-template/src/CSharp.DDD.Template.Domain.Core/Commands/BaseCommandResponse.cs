using CSharp.DDD.Template.Domain.Core.Enums;

namespace CSharp.DDD.Template.Domain.Core.Commands
{

    /// <summary>
    /// API返回消息基类
    /// </summary>
    public class BaseCommandResponse: ICommandResponse
    {
        public BaseCommandResponse()
        { }
       
        public BaseCommandResponse(EnumApiStatus status, string message)
        {
            this.message = message;
            this.code = ((int)status).ToString();
        }
        /// <summary>
        /// 接口业务状态
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 消息状态说明
        /// </summary>
        public string message { get; set; }
      
    }

    /// <summary>
    /// API返回消息基类
    /// </summary>
    public class BaseCommandResponse<T> : BaseCommandResponse
    {
        public BaseCommandResponse(EnumApiStatus code, string message) : this(code, message, default(T))
        {

        }

        public BaseCommandResponse(EnumApiStatus code, string message, T data)
        {
            this.code = ((int)code).ToString();
            this.message = message;
            this.data = data;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; set; }
    }

}

