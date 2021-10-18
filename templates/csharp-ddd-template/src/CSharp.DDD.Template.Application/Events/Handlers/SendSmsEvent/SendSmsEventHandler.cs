using Autofac.Extras.DynamicProxy;
using Hummingbird.Extensions.EventBus.Abstractions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharp.DDD.Template.Application.Interceptors;
using CSharp.DDD.Template.Domain.Events;

namespace CSharp.DDD.Template.Application.Events
{
    /// <summary>
    /// email发送事件处理程序
    /// </summary>
    [Intercept(typeof(TracerInterceptor))]
    public class SendSmsEventHandler : IEventHandler<SendSmsEvent>
    {
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="msgDistributor"></param>
        public SendSmsEventHandler()
        {
        }
        /// <summary>
        /// 事件处理程序
        /// </summary>
        /// <param name="event"></param>
        /// <param name="headers"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<bool> Handle(SendSmsEvent @event, Dictionary<string, object> headers, CancellationToken cancellationToken)
        {
            
            return await Task.FromResult(false);
        }
    }
}
