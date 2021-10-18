using System;
using Castle.DynamicProxy;
using Hummingbird.Extensions.Tracing;

namespace CSharp.DDD.Template.Application.Interceptors
{
    /// <summary>
    /// 拦截器 需要实现 IInterceptor接口 Intercept方法
    /// </summary>
    public class TracerInterceptor : IInterceptor
    {
      

        /// <summary>
        /// 拦截方法 打印被拦截的方法执行前的名称、参数和方法执行后的 返回结果
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {
            using (var tracer = new Tracer($"{invocation.TargetType.Name}:{invocation.Method.Name}" ))
            {
                try
                {
                    tracer.LogRequest(invocation.Arguments);

                    //在被拦截的方法执行完毕后 继续执行
                    invocation.Proceed();

                    //tracer.LogResponse(invocation.ReturnValue);
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
