using Microsoft.Extensions.Hosting;
using Serilog.Sinks.Elasticsearch;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace common.Logging
{

    public static class SeriLogger
    {
        
        public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
           (context, configuration) =>
           {
               var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
               var applicationName = context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-");
               var environmentName = context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-");
               var logFilePath = Path.Combine("C:\\log\\services", applicationName, "Log_");
               configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Debug()
                    .WriteTo.Console()
                    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day) // Adjust the file path as needed
                    //.WriteTo.Elasticsearch(
                    //    new ElasticsearchSinkOptions(new Uri(elasticUri))
                    //    {
                    //        IndexFormat = $"applogs-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                    //        AutoRegisterTemplate = true,
                    //        NumberOfShards = 2,
                    //        NumberOfReplicas = 1
                    //    })
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .ReadFrom.Configuration(context.Configuration);

               
           };
    }
}