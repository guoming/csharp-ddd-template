using Hummingbird.Extensions.Tracing;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSharp.DDD.Template.Application.Commands.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            using (var tracer = new Tracer(typeof(TRequest).Name))
            {
                try
                {
                    if (request is Domain.Core.Commands.ICommand)
                    {
                        var req = request as Domain.Core.Commands.ICommand;

                        tracer.LogRequest(request.ToString());

                        #region 给请求打上标记（方便查询）

                        var req_tags = req.GetRequestTags();
                        foreach (var tag in req_tags)
                        {
                            tracer.SetTag(tag.Key, tag.Value);
                        }

                        #endregion
                    }
                    else
                    {
                        tracer.LogRequest(request);
                    }

                    var response = await next();
                    tracer.LogResponse(response);
                    return response;
                }
                catch (Exception ex)
                {
                    tracer.LogException(ex);
                    throw;
                }
            }
        }
    }
}
