using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CSharp.DDD.Template.Domain.Core.Enums;
using CSharp.DDD.Template.Domain.Core.Externsions;

namespace CSharp.DDD.Template.Domain.Core.Commands
{   /// <summary>
    /// 基础命令处理基类
    /// </summary>
    /// <typeparam name="TRequest">继承BaseCommand的实体类</typeparam>
    /// <typeparam name="TResponse">响应实体</typeparam>
    public abstract class BaseCommandHandler<TRequest, TResponse>
    : IRequestHandler<TRequest, BaseCommandResponse<TResponse>>
            //where TRequest : ICommand
            where TRequest : IRequest<BaseCommandResponse<TResponse>>
    {
       

        public virtual Task<BaseCommandResponse<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public BaseCommandResponse Error(string message = "")
        {
            return new BaseCommandResponse()
            {
                code = ((int)EnumApiStatus.BizError).ToString(),
                message = string.IsNullOrEmpty(message) ? EnumApiStatus.BizError.GetEnumDescript() : message
            };
        }

        public BaseCommandResponse<T> Error<T>(string message = "")
        {
            return new BaseCommandResponse<T>(EnumApiStatus.BizError, string.IsNullOrEmpty(message) ? EnumApiStatus.BizError.GetEnumDescript() : message);
        }

        public BaseCommandResponse OK(string message = "")
        {
            return new BaseCommandResponse(EnumApiStatus.BizOK, string.IsNullOrEmpty(message) ? EnumApiStatus.BizOK.GetEnumDescript() : message);
        }

        public BaseCommandResponse<T> OK<T>(T data, string message = "")
        {
            return new BaseCommandResponse<T>(EnumApiStatus.BizOK, string.IsNullOrEmpty(message) ? EnumApiStatus.BizOK.GetEnumDescript() : message, data);
        }
    }
}
