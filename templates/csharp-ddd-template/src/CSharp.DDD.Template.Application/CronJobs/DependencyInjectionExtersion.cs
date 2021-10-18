using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Extras.Quartz;
using Hummingbird.Extensions.EventBus.Abstractions;
using System;
using System.Linq;
using CSharp.DDD.Template.Application.Interceptors;
using CSharp.DDD.Template.Application.Jobs;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {

        public static ContainerBuilder AddQuartz(this ContainerBuilder cb)
        {
            // 1) Register IScheduler
            cb.RegisterModule(new QuartzAutofacFactoryModule());
            // 2) Register jobs
            cb.RegisterModule(new QuartzAutofacJobsModule(new System.Reflection.Assembly[]{System.Reflection.Assembly.GetExecutingAssembly() }));
            return cb;

            
        }
    }
}

