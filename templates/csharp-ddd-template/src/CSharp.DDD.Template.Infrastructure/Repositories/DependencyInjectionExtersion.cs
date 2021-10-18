using CSharp.DDD.Template.Domain.Repositories;
using CSharp.DDD.Template.Infrastructure.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IMessageTypeRepository, MessageTypeRepository>();
            services.AddTransient<ISubscriberConfigRepository, SubscriberConfigRepository>();
            services.AddTransient<ISubscriberMessageTypeRepository, SubscriberMessageTypeRepository>();
            return services;

        }
    }
}
