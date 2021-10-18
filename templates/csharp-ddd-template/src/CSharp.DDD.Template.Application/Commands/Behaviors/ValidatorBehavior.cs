using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSharp.DDD.Template.Application.Commands.Behaviors
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidator<TRequest>[] _validators;
        public ValidatorBehavior(IValidator<TRequest>[] validators) => _validators = validators;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var failures = _validators
               .Select(v => v.Validate(request))
               .SelectMany(result => result.Errors)
               .Where(error => error != null)
               .ToList();

            if (failures.Any())
            {
                throw new FluentValidation.ValidationException(failures);
            }

            var response = await next();
            return response;
        }

    }
}
