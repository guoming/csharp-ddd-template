using Autofac;
using Autofac.Extras.DynamicProxy;
using Hummingbird.Extensions.EventBus.Abstractions;
using System;
using System.Linq;
using CSharp.DDD.Template.Application.Interceptors;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        public static void AddEventInterceptors(this ContainerBuilder builder)
        {
            #region AOP

            builder.RegisterType<TracerInterceptor>();
            builder.RegisterType<MetricInterceptor>();

            var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(a => a.GetTypes().Where(type => Array.Exists(type.GetInterfaces(), t =>
                      t.IsGenericType

                      && (t.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                      || t.GetGenericTypeDefinition() == typeof(IEventBatchHandler<>)
                      ))))
                      .ToArray();

            foreach (var type in types)
            {

                builder.RegisterType(type).EnableClassInterceptors().InterceptedBy(typeof(TracerInterceptor), typeof(MetricInterceptor));
            }


            #endregion



        }
    }
}

