using System;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hummingbird.Extensions.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using CSharp.DDD.Template.Infrastructure;
using MediatR;
using System.Reflection;

namespace CSharp.DDD.Template.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
      

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors()
            .AddMvc(a=>a.EnableEndpointRouting=false)
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddControllersAsServices()  //全局配置Json序列化处理
            
            .AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //忽略空值数据
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                //返回标准时区
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                //日期格式转换
                options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            });
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddVersionedApiExplorer(option =>
            {
                // 版本名的格式：v+版本号
                option.GroupNameFormat = "'v'V";
                option.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddAutofac();
            services.AddHealthChecks(checks =>
            {
                checks.WithDefaultCacheDuration(TimeSpan.FromSeconds(5));

                #region Redis

                var redis_Host = Configuration["Redis:Host"];
                var redis_Port = Configuration["Redis:Port"];
                var redis_Password = Configuration["Redis:Password"];

                checks.AddRedisCheck("redis", TimeSpan.FromMinutes(1),
                    $"{redis_Host}:{redis_Port},password={redis_Password},allowAdmin=true,ssl=false,abortConnect=false,connectTimeout=5000");

                #endregion

                #region MySql
                checks.AddMySqlCheck("MySql", $"{Configuration["MySql:connectionString"]}"); //物流轨迹
                #endregion

                #region Rabbitmq                
                checks.AddRabbitMQCheck($"{Configuration["EventBus:HostName"]}", rabbitmq =>
                {
                    rabbitmq.WithEndPoint(Configuration["EventBus:HostName"] ?? "localhost", int.Parse(Configuration["EventBus:Port"] ?? "5672"));
                    rabbitmq.WithAuth(Configuration["EventBus:UserName"] ?? "guest", Configuration["EventBus:Password"] ?? "guest");
                    rabbitmq.WithExchange(Configuration["EventBus:VirtualHost"] ?? "/");
                });
                #endregion

            });
            services.AddHummingbird(hummingbird =>
                 {
                     var redis_host = Configuration["Redis:Host"];
                     var redis_port = int.Parse(Configuration["Redis:Port"]);
                     var redis_password = Configuration["Redis:Password"];

                     hummingbird.AddDistributedLock(locker =>
                     {
                         locker.WithDb(0);
                         locker.WithKeyPrefix("CSharp.DDD.Template");
                         locker.WithServerList($"{redis_host}:{redis_port}");
                         locker.WithPassword(redis_password);
                     });

                     hummingbird.AddCacheing(a =>
                     {
                         a.WithPassword(redis_password);
                         a.WithWriteServerList($"{redis_host}:{redis_port}");
                         a.WithReadServerList($"{redis_host}:{redis_port}");
                         a.WithKeyPrefix("CSharp.DDD.Template");
                         a.WithDb(0);
                     });

                     //缓存
                     hummingbird.AddCache(config =>
                     {
                         config.WithDatabase(0);
                         config.WithEndpoint(redis_host, redis_port);
                         config.WithPassword(redis_password);
                     }, "CSharp.DDD.Template");

                     //弹性Http连接
                     hummingbird.AddResilientHttpClient((origin, config) =>
                     {
                         config.RetryCount = 3;
                         config.TimeoutMillseconds = 30 * 1000;
                         config.ExceptionsAllowedBeforeBreaking = 10;
                         config.DurationSecondsOfBreak = 50;

                     });

                     hummingbird.AddSnowflakeUniqueIdGenerator(option =>
                     {
                         option.CenterId = 0;
                         option.AddConsulWorkIdCreateStrategy(Configuration, "CSharp.DDD.Template");

                     });

                     hummingbird.AddConsulDynamicRoute(Configuration);

                     hummingbird.AddOpenTracing(tracing =>
                     {
                         tracing.AddJaeger(Configuration.GetSection("Tracing"));

                     });

                     //事件总线
                     hummingbird.AddEventBus(eventbus =>
                     {
                         eventbus.AddRabbitmq(rabbitmq =>
                         {
                             rabbitmq.WithEndPoint(Configuration["EventBus:HostName"] ?? "localhost", int.Parse(Configuration["EventBus:Port"] ?? "5672"));
                             rabbitmq.WithAuth(Configuration["EventBus:UserName"] ?? "guest", Configuration["EventBus:Password"] ?? "guest");
                             rabbitmq.WithExchange(Configuration["EventBus:VirtualHost"] ?? "/");
                             rabbitmq.WithSender(int.Parse(Configuration["EventBus:SenderMaxConnections"] ?? "10"), int.Parse(Configuration["EventBus:SenderAcquireRetryAttempts"] ?? "3"));
                             rabbitmq.WithReceiver(
                                 ReceiverMaxConnections: int.Parse(Configuration["EventBus:ReceiverMaxConnections"] ?? "5"),
                                 ReveiverMaxDegreeOfParallelism: int.Parse(Configuration["EventBus:ReveiverMaxDegreeOfParallelism"] ?? "5"),
                                 ReceiverAcquireRetryAttempts: int.Parse(Configuration["EventBus:ReceiverAcquireRetryAttempts"] ?? "3"));
                         });

                     });


                 });
            services
            .AddMetrics(Configuration.GetSection("AppMetrics:Influxdb"))
            .AddMySql(Configuration.GetSection("MySql"))
            .AddQueries()
            .AddRepositories()
            .AddHttpClient();
           
           
            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.AddCommandHandlers();
            containerBuilder.AddCommandBehavior();
            containerBuilder.AddEventInterceptors();
            var container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor accessor)
        {
           
            if (env.EnvironmentName=="dev")
            {
                app.UseDeveloperExceptionPage();

               
            }
            else
            {
                app.UseHsts();
            }
           
            app.UseStaticFiles();
            app.UseMiddlewares();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
                //.AllowCredentials());

            app.UseAuthentication();
            app.UseMvc();
            HttpContextProvider.Accessor = accessor;
        }
    }
}
