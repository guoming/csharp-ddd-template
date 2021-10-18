using System;
using System.Collections.Generic;

namespace CSharp.DDD.Template.Domain.Commands.LoginCommand
{
    public class LoginCommandResponse
    {
        public string token { get; set; }

        public string role { get; set; }

        public List<string> permission_list { get; set; }
     
    }
}