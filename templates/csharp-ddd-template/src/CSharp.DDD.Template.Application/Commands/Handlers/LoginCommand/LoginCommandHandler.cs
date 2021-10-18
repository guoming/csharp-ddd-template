using System;
using System.Threading;
using System.Threading.Tasks;
using CSharp.DDD.Template.Domain.Core.Commands;
using CSharp.DDD.Template.Domain.Commands.LoginCommand;

namespace CSharp.DDD.Template.Application.Commands.Handlers.LoginCommand
{
    public class LoginCommandHandler : BaseCommandHandler<CSharp.DDD.Template.Domain.Commands.LoginCommand.LoginCommand,LoginCommandResponse>
    {
        public LoginCommandHandler()
        {
        }

       

        public override async Task<BaseCommandResponse<LoginCommandResponse>> Handle(
            CSharp.DDD.Template.Domain.Commands.LoginCommand.LoginCommand request,
            CancellationToken cancellationToken)
        {

            return await Task.FromResult(OK<LoginCommandResponse>(new LoginCommandResponse()
            {
                token = Guid.NewGuid().ToString("N"),
                role = "admin",
                permission_list = new System.Collections.Generic.List<string>() {
                     "01","02"
                 }
            }));

        }
    }
}
