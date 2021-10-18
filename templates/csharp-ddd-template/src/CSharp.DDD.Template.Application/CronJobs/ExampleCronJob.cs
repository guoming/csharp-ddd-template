using System;
using System.Threading.Tasks;
using MediatR;
using Quartz;


namespace CSharp.DDD.Template.Application.Jobs
{
    
    [DisallowConcurrentExecution]
    public class ExampleCronJob : IJob
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// 构造注入
        /// </summary>
        public ExampleCronJob(IMediator mediator)
        {
            _mediator = mediator;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            await this._mediator.Send(new Domain.Commands.LoginCommand.LoginCommand() {});
        }
    }
}