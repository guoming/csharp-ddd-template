using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hummingbird.Extensions.EventBus.Abstractions;
using Hummingbird.Extensions.EventBus.Models;
using CSharp.DDD.Template.Domain.Core.Commands;
using CSharp.DDD.Template.Domain.Events;

namespace CSharp.DDD.Template.Application.Commands.Handlers.SendSmsCommand
{
    /// <summary>
    /// 发送SMS处理程序
    /// </summary>
    public class SendSmsCommandHandler : BaseCommandHandler<CSharp.DDD.Template.Domain.Commands.SendSmsCommand.SendSmsCommand,bool>
    {
        private readonly IEventBus _eventBus;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventBus"></param>
        /// <param name="options"></param>
        public SendSmsCommandHandler(IEventBus eventBus)
        {
            this._eventBus = eventBus;
        }

     

        public override async Task<BaseCommandResponse<bool>> Handle(
            Domain.Commands.SendSmsCommand.SendSmsCommand request,
            CancellationToken cancellationToken)
        {
            var events = new List<EventLogEntry>() {
                new EventLogEntry("CSharp.DDD.Template.SendSmsEvent", new SendSmsEvent()
                {
                   TemplateCode=request.TemplateCode,
                   PhoneAreaCode=request.PhoneAreaCode,
                   PhoneNumber=request.PhoneNumber,
                   ParamterList=request.ParamterList
                })};

            var ret = await this._eventBus.PublishAsync(events);

            if (ret)
            {
                return OK<bool>(ret);

            }
            else
            {
                return Error<bool>();
            }
        }
    }
}
