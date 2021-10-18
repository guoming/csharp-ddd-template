using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using CSharp.DDD.Template.Domain.Core.Commands;

namespace CSharp.DDD.Template.Domain.Commands.LoginCommand
{
    /// <summary>
    /// 发送SMS
    /// </summary>
    public class LoginCommand : BaseCommand<LoginCommandResponse>
    {

        public string user_name { get; set; }

        public string user_password { get; set; }

    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(command => command.user_name).NotEmpty();
            RuleFor(command => command.user_password).NotEmpty();
          
        }
    }
}
