using System;
using System.Collections.Generic;
using MediatR;

namespace CSharp.DDD.Template.Domain.Core.Commands
{
    public interface ICommand
    {

        Dictionary<string, dynamic> GetRequestTags();
    }

  

}