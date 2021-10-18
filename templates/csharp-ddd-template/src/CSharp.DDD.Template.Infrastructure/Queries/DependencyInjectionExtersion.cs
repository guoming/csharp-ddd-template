using CSharp.DDD.Template.Domain.Queries;
using CSharp.DDD.Template.Infrastructure.Queries;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        public static IServiceCollection AddQueries(this IServiceCollection services)
        {
            services.AddTransient<IMessageTypeQuery, MessageTypeQuery>();
            services.AddTransient<ISubscriberConfigQuery, SubscriberConfigQuery>();
            services.AddTransient<ISubscriberMessageTypeQuery, SubscriberMessageTypeQuery>();
            return services;
        }
    }
}
