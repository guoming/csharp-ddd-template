using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CSharp.DDD.Template.Domain.Core.Commands;

namespace CSharp.DDD.Template.API.Controllers.V1
{
    [ApiController]
    public class MessageServiceController : ControllerBase
    {
        private IMediator _mediator;

        public MessageServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("api/v1/user/login")]
        public async Task<ICommandResponse> PublishMessage([FromBody]Domain.Commands.LoginCommand.LoginCommand command)
        {
            return await _mediator.Send(command);
        }

    }
}