using System.Collections.Generic;
using System.Reflection;
using Autofac;
using MediatR;
using CSharp.DDD.Template.Application.Commands.Behaviors;
using CSharp.DDD.Template.Domain.Core.Commands;

namespace Microsoft.Extensions.DependencyInjection
{

    public static partial class DependencyInjectionExtersion
    {
        public static void AddCommandHandlers(this ContainerBuilder builder)
        {
           
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
            .AsImplementedInterfaces();

            //注入所有实现IRequestHandler接口的处理类 及 验证器类

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IRequestHandler<,>));
            builder.RegisterAssemblyTypes(Assembly.GetCallingAssembly()).Where(t => t.IsClosedTypeOf(typeof(FluentValidation.IValidator<>))).AsImplementedInterfaces();
        }

        public static void AddCommandBehavior(this ContainerBuilder builder)
            {
            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}
