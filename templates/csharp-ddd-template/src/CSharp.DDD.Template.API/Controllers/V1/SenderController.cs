using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CSharp.DDD.Template.Domain.Core.Commands;
using CSharp.DDD.Template.Domain.Commands.SendSmsCommand;
using CSharp.DDD.Template.Infrastructure;

namespace CSharp.DDD.Template.API.Controllers.V1
{
    [ApiController]
    public class SenderController : Controller
    {
        private IMediator _mediator;

        public SenderController(IMediator mediator)
        {
            _mediator = mediator;
        }
       

        /// <summary>
        /// 发送SMS
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("api/v1/sms/send")]
        public async Task<ICommandResponse> SendSMS([FromBody]SendSmsCommand command)
        {
            return await _mediator.Send(command);
        }

        
    }
}