using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp.DDD.Template.Domain.Core.Commands
{
   

    /// <summary>
    /// 基础命令（所有请求类需实现该抽象类）
    /// </summary>
    /// <typeparam name="TResponse">响应实体</typeparam>
    public abstract class BaseCommand<TResponse> : ICommand, IRequest<Core.Commands.BaseCommandResponse<TResponse>>

    {

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public virtual Dictionary<string, dynamic> GetRequestTags()
        {
            var tags = new Dictionary<string, dynamic>();
            return tags;
        }
    }

    /// <summary>
    /// 基础验证器
    /// </summary>
    /// <typeparam name="TCommand">请求类</typeparam>
    public abstract class BaseAbstractValidator<TCommand> : AbstractValidator<TCommand>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseAbstractValidator()
        {
            Validator();
        }

        /// <summary>
        /// 验证器
        /// </summary>
        public virtual void Validator()
        {
            
        }
    }
}
