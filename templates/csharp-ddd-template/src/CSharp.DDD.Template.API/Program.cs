using System;
using System.IO;
using App.Metrics;
using App.Metrics.AspNetCore;
using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Logging;
using Hummingbird.AspNetCore.HealthChecks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace CSharp.DDD.Template.API
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
           
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
            .UseIISIntegration()
            .UseKestrel()
            .UseLibuv()
            .UseHealthChecks("/healthcheck")
            .ConfigureAppConfiguration((context, config) =>
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
                    Environment.SetEnvironmentVariable("Apollo:Secret", apollo_secret ?? "");

                    config.AddApollo(apollo_appId, apollo_metaServer)
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

                    config.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFileEx(path: "Config/appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFileEx("Config/tracing.json", false, true)
                    .AddJsonFileEx("Config/rabbitmq.json", false, true)
                    .AddJsonFileEx("Config/logging.json", false, true)
                    .AddJsonFileEx("Config/metrics.json", false, true)
                    .AddJsonFileEx("Config/mysql.json", false, true)
                    .AddJsonFileEx("Config/service.json", false, true)
                    .AddJsonFileEx("Config/redis.json", false, true);
                 

                }
                #endregion
            })
            .ConfigureLogging((context, builder) =>
            {
                builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                builder.AddLog4Net("Config/log4net.xml", true);
                LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Debug);
#if DEBUG
                builder.AddDebug();
                builder.AddConsole();

#endif

            })
            .UseMetricsWebTracking()
            .UseMetrics((builderContext, metricsBuilder) => {
                   metricsBuilder.ToPrometheus();
                   metricsBuilder.ToInfluxDb(builderContext.Configuration.GetSection("AppMetrics:Influxdb"));
               })
            .UseStartup<Startup>();
        }
    }
}
