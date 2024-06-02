using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;

namespace Logging.Shared
{
    public static class Logging
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogging => (builderContext, loggerConfiguration) =>
        {
            IHostEnvironment environment = builderContext.HostingEnvironment;

            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Environment", environment.EnvironmentName)
                .Enrich.WithProperty("AppName", environment.ApplicationName);

            string elasticSearchBaseUrl = builderContext.Configuration.GetSection("Elasticsearch")["BaseUrl"];
            string userName = builderContext.Configuration.GetSection("Elasticsearch")["UserName"];
            string password = builderContext.Configuration.GetSection("Elasticsearch")["Password"];
            string indexName = builderContext.Configuration.GetSection("Elasticsearch")["IndexName"];

            loggerConfiguration.WriteTo.Elasticsearch(new(new Uri(elasticSearchBaseUrl))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = Serilog.Sinks.Elasticsearch.AutoRegisterTemplateVersion.ESv8,
                IndexFormat = $"{indexName}-{environment.EnvironmentName}-logs-" + "{0:yyy.MM.dd}",
                ModifyConnectionSettings = x => x.BasicAuthentication(userName, password),
                CustomFormatter = new ElasticsearchJsonFormatter()
            });
        };
    }
}