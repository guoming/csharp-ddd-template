using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Logging;
using Hummingbird.Extensions.EventBus.RabbitMQ;
using Hummingbird.Extensions.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using CSharp.DDD.Template.Application.Events;
using CSharp.DDD.Template.Domain.Events;

namespace CSharp.DDD.Template.Client
{
    internal class ServiceContainerFactory : IServiceProviderFactory<ContainerBuilder>
    {
      

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            return containerBuilder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var container = containerBuilder.Build();
            var sp = new AutofacServiceProvider(container);
            var logger = sp.GetRequiredService<ILogger<ServiceContainerFactory>>();

            sp.UseSubscriber(eventbus =>
            {
                eventbus.Register<SendSmsEvent, SendSmsEventHandler>("CSharp.DDD.Template.SendSmsEventHandler", "CSharp.DDD.Template.SendSmsEvent");

                eventbus.Subscribe((Messages) =>
                {
                    #region 输出日志
                    foreach (var message in Messages)
                    {
                        logger.LogDebug($"ACK: queue {message.QueueName} routeKey={message.RouteKey} MessageId:{message.MessageId}");
                    }
                    #endregion

                }, async (obj) =>
                {
                    #region 输出日志
                    foreach (var message in obj.Messages)
                    {
                        logger.LogError($"NAck: queue {message.QueueName} routeKey={message.RouteKey} MessageId:{message.MessageId}");
                    }

                    //消息消费失败执行以下代码
                    if (obj.Exception != null)
                    {
                        logger.LogError(obj.Exception, obj.Exception.Message);
                    }
                    #endregion

                    #region 写入重试队列

                    var eventBus = sp.GetRequiredService<Hummingbird.Extensions.EventBus.Abstractions.IEventBus>();

                    var ret = !(await eventBus.PublishAsync(obj.Messages.Select(@event => @event.WaitAndRetry(5, 9)).ToList()));

                    return ret;
                    #endregion

                });


            });

            return sp;
        }
    }

    class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                 .ConfigureHostConfiguration(configHost =>
                 {
                     configHost.SetBasePath(Directory.GetCurrentDirectory());
                     configHost.AddJsonFile("hostsettings.json", optional: true);
                     configHost.AddEnvironmentVariables(prefix: "ZT_");
                     configHost.AddCommandLine(args);
                 })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables()
                          .AddCommandLine(args);


                    #region 集成apollo

                    var apollo_appId = Environment.GetEnvironmentVariable("Apollo_AppId");
                    var apollo_metaServer = Environment.GetEnvironmentVariable("Apollo_MetaServer");
                    var apollo_cluster = Environment.GetEnvironmentVariable("Apollo_Cluster");
                    var apollo_secret = Environment.GetEnvironmentVariable("Apollo_Secret");


                    //成功获取到apollo配置，则集成apollo，apollo 中需将namespace 映射配置文件
                    if (!string.IsNullOrEmpty(apollo_appId))
                    {
                        Environment.SetEnvironmentVariable("Apollo:AppId", apollo_appId);
                        Environment.SetEnvironmentVariable("Apollo:Cluster", apollo_cluster);
                        Environment.SetEnvironmentVariable("Apollo:MetaServer", apollo_metaServer);
                        Environment.SetEnvironmentVariable("Apollo:Secret", apollo_secret??"");

                        config.AddApollo(apollo_appId,apollo_metaServer)
                            .AddDefault(ConfigFileFormat.Xml)
                            .AddDefault(ConfigFileFormat.Json)
                            .AddDefault(ConfigFileFormat.Yml)
                            .AddDefault(ConfigFileFormat.Yaml)
                            .AddDefault()
                            .AddNamespace("appsettings", ConfigFileFormat.Json)
                            .AddNamespace("tracing", ConfigFileFormat.Json)
                            .AddNamespace("rabbitmq", ConfigFileFormat.Json)
                            .AddNamespace("logging", ConfigFileFormat.Json)
                            .AddNamespace("metrics", ConfigFileFormat.Json)
                            .AddNamespace("mysql", ConfigFileFormat.Json)
                            .AddNamespace("service", ConfigFileFormat.Json)
                            .AddNamespace("redis", ConfigFileFormat.Json);
                        config.Build();
                    }
                    else
                    {
                        #region config
                        config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFileEx(path: "Config/appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFileEx("Config/rabbitmq.json", false, true)
                        .AddJsonFileEx("Config/logging.json", false, true)
                        .AddJsonFileEx("Config/metrics.json", false, true)
                        .AddJsonFileEx("Config/tracing.json", false, true)
                        .AddJsonFileEx("Config/mysql.json", false, true)
                        .AddJsonFileEx("Config/service.json", false, true)
                        .AddJsonFileEx("Config/redis.json", false, true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                        #endregion
                    }

                    #endregion
                })
                .ConfigureServices((hostContext, services) =>
                {
                    #region  services

                    services.AddHttpContextAccessor()
                      .AddTransient<IConfiguration>(a => hostContext.Configuration)
                      .AddAutofac()
                      .AddHealthChecks(checks =>
                      {
                          checks.WithPartialSuccessStatus(CheckStatus.Unhealthy);
                          checks.WithDefaultCacheDuration(TimeSpan.FromSeconds(5));

                          #region Redis

                          var redis_Host = hostContext.Configuration["Redis:Host"];
                          var redis_Port = hostContext.Configuration["Redis:Port"];
                          var redis_Password = hostContext.Configuration["Redis:Password"];

                          checks.AddRedisCheck("redis", TimeSpan.FromMinutes(1),
                              $"{redis_Host}:{redis_Port},password={redis_Password},allowAdmin=true,ssl=false,abortConnect=false,connectTimeout=5000");

                          #endregion

                          #region MySql
                          checks.AddMySqlCheck("MySql", $"{hostContext.Configuration["MySql:ConnectionString"]}"); //物流轨迹
                          #endregion

                          #region Rabbitmq                
                          checks.AddRabbitMQCheck($"{hostContext.Configuration["EventBus:HostName"]}", rabbitmq =>
                          {
                              rabbitmq.WithEndPoint(hostContext.Configuration["EventBus:HostName"] ?? "localhost", int.Parse(hostContext.Configuration["EventBus:Port"] ?? "5672"));
                              rabbitmq.WithAuth(hostContext.Configuration["EventBus:UserName"] ?? "guest", hostContext.Configuration["EventBus:Password"] ?? "guest");
                              rabbitmq.WithExchange(hostContext.Configuration["EventBus:VirtualHost"] ?? "/");
                          });
                          #endregion

                      })
                      .AddHummingbird(hummingbird =>
                      {
                          var redis_host = hostContext.Configuration["Redis:Host"];
                          var redis_port = int.Parse(hostContext.Configuration["Redis:Port"]);
                          var redis_password = hostContext.Configuration["Redis:Password"];

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
                          hummingbird.AddConsulDynamicRoute(hostContext.Configuration, s =>
                          {
                              s.AddTags($"LogLevel={hostContext.Configuration["Logging:LogLevel:Default"]}");
                              s.AddTags($"CenterId ={hostContext.Configuration["CenterId"]}");
                          });
                          hummingbird.AddSnowflakeUniqueIdGenerator(option =>
                          {
                              option.CenterId = int.Parse(hostContext.Configuration["CenterId"]);
                              option.AddConsulWorkIdCreateStrategy(hostContext.Configuration, "CSharp.DDD.Template");

                          });

                          hummingbird.AddOpenTracing(builder =>
                          {
                              builder.AddJaeger(hostContext.Configuration.GetSection("Tracing"));
                          });

                          //事件总线
                          hummingbird.AddEventBus(eventbus =>
                          {
                              eventbus.AddRabbitmq(rabbitmq =>
                              {
                                  rabbitmq.WithEndPoint(hostContext.Configuration["EventBus:HostName"] ?? "localhost", int.Parse(hostContext.Configuration["EventBus:Port"] ?? "5672"));
                                  rabbitmq.WithAuth(hostContext.Configuration["EventBus:UserName"] ?? "guest", hostContext.Configuration["EventBus:Password"] ?? "guest");
                                  rabbitmq.WithExchange(hostContext.Configuration["EventBus:VirtualHost"] ?? "/");
                                  rabbitmq.WithSender(
                                      SenderMaxConnections: int.Parse(hostContext.Configuration["EventBus:SenderMaxConnections"] ?? "10"),
                                      AcquireRetryAttempts: int.Parse(hostContext.Configuration["EventBus:SenderAcquireRetryAttempts"] ?? "3"));
                                  rabbitmq.WithReceiver(
                                          PreFetch: ushort.Parse(hostContext.Configuration["EventBus:ReceiverPrefetch"] ?? "100"),
                                          ReceiverHandlerTimeoutMillseconds: int.Parse(hostContext.Configuration["EventBus:ReceiverHandlerTimeoutMillseconds"] ?? "10000"),
                                          ReceiverMaxConnections: int.Parse(hostContext.Configuration["EventBus:ReceiverMaxConnections"] ?? "5"),
                                          ReveiverMaxDegreeOfParallelism: int.Parse(hostContext.Configuration["EventBus:ReveiverMaxDegreeOfParallelism"] ?? "5"),
                                          ReceiverAcquireRetryAttempts: int.Parse(hostContext.Configuration["EventBus:ReceiverAcquireRetryAttempts"] ?? "3"));
                              });


                          });
                          hummingbird.AddQuartz(hostContext.Configuration.GetSection("Quartz"));

                      })
                      .AddMetrics(hostContext.Configuration.GetSection("AppMetrics:Influxdb"))
                      .AddMySql(hostContext.Configuration.GetSection("MySql"))
                      .AddQueries()
                      .AddRepositories()
                      .AddHttpClient();
                      
                    #endregion

                })
                .UseServiceProviderFactory<ContainerBuilder>(new ServiceContainerFactory())
                .ConfigureContainer<ContainerBuilder>((container) =>
                {
                    container.AddQuartz();
                    container.AddEventInterceptors();
                    container.AddCommandHandlers();
                    container.AddCommandBehavior();

                })
                .ConfigureLogging((hostingContext, logging) =>
                {

                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddLog4Net("Config/log4net.xml", true);
                    LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Debug);


#if DEBUG
                    logging.AddConsole();
                    logging.AddDebug();
#endif
                })
                .UseConsoleLifetime();


        static void Main(string[] args)
        {
         
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

    }
}