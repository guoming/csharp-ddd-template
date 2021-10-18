using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using CSharp.DDD.Template.Domain.Core.Enums;

namespace CSharp.DDD.Template.Infrastructure.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private ExceptionHandlerOptions _options;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        public ErrorHandlingMiddleware(RequestDelegate next, IOptions<ExceptionHandlerOptions> options)
        {
            this.next = next;
            this._options = options.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                var result = JsonConvert.SerializeObject(new Domain.Core.Commands.BaseCommandResponse()
                {
                    code = ((int)EnumApiStatus.BizError).ToString(),
                    message = string.Join("\n\r", ex.Errors.Select(a => a.ErrorMessage))
                });

                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(result);
            }
            catch (Domain.Core.Exceptions.DomainException ex)
            {
                var result = JsonConvert.SerializeObject(new Domain.Core.Commands.BaseCommandResponse()
                {
                    code = ((int)EnumApiStatus.BizError).ToString(),
                    message = string.Join("\n\r", ex.Message)
                });

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(result);
            }
            catch (Exception ex)
            {
                var result = JsonConvert.SerializeObject(new Domain.Core.Commands.BaseCommandResponse()
                {
                    code = ((int)EnumApiStatus.BizError).ToString(),
                    message = ex.Message
                });
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(result);
            }
        }
    }
}
