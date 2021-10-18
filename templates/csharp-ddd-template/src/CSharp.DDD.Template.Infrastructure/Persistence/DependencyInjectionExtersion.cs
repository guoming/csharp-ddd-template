using Microsoft.Extensions.Configuration;
using CSharp.DDD.Template.Domain.Core.Persistence;
using CSharp.DDD.Template.Infrastructure.Configurations;
using CSharp.DDD.Template.Infrastructure.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class DependencyInjectionExtersion
    {
        /// <summary>
        /// 注入Mysql连接上下文
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddMySql(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDbContext,DbContext>();
            services.Configure<SqlConfiguration>(configuration);
            return services;
        }
    }
}
