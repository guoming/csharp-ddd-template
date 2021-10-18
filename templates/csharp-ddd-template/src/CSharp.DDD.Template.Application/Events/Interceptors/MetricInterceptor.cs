using Castle.DynamicProxy;

namespace CSharp.DDD.Template.Application.Interceptors
{
    /// <summary>
    /// 拦截器 需要实现 IInterceptor接口 Intercept方法
    /// </summary>
    public class MetricInterceptor : IInterceptor
    {
        //private readonly IBussinessMetricRepository bussinessMetricRepository;

        //public MetricInterceptor(IBussinessMetricRepository bussinessMetricRepository)
        //{
        //    this.bussinessMetricRepository = bussinessMetricRepository;
        //}

        /// <summary>
        /// 拦截方法 打印被拦截的方法执行前的名称、参数和方法执行后的 返回结果
        /// </summary>
        /// <param name="invocation">包含被拦截方法的信息</param>
        public void Intercept(IInvocation invocation)
        {


            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            //在被拦截的方法执行完毕后 继续执行
            invocation.Proceed();
            stopwatch.Stop();

            //bussinessMetricRepository.Transactions(typeof(CSharp.DDD.Template.Application.Events.OrderCancelEvent).FullName);
            //bussinessMetricRepository.ResponseTime(typeof(CSharp.DDD.Template.Application.Events.OrderCancelEvent).FullName, stopwatch.ElapsedMilliseconds);



        }
    }
}
