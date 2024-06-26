﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Shared
{
    public static class OpenTelemetryExtensions
    {
        public static void AddOpenTelemetryExt(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenTelemetryConstants>(configuration.GetSection("OpenTelemetry"));
            var openTelemetryConstants = (configuration.GetSection("OpenTelemetry").Get<OpenTelemetryConstants>())!;
            ActivitySourceProvider.Source = new System.Diagnostics.ActivitySource(openTelemetryConstants.ActivitySourceName);

            services.AddOpenTelemetry().WithTracing(options =>
            {
                options
                    .AddSource(openTelemetryConstants.ActivitySourceName)
                    .ConfigureResource(resource =>
                    {
                        resource.AddService(openTelemetryConstants.ServiceName, serviceVersion: openTelemetryConstants.ServiceVersion);
                    });

                options
                    .AddAspNetCoreInstrumentation(aspNetCoreOptions =>
                    {
                        aspNetCoreOptions.Filter = (context) =>
                        {
                            return !string.IsNullOrEmpty(context.Request.Path.Value)
                                ? context.Request.Path.Value.Contains("api", StringComparison.InvariantCulture)
                                : false;
                        };

                        // aspNetCoreOptions.RecordException = true; hataları sadece elastic'de görebilmek için kapattık.
                    });

                options
                    .AddEntityFrameworkCoreInstrumentation(efCoreOptions =>
                    {
                        efCoreOptions.SetDbStatementForText = true;
                        efCoreOptions.SetDbStatementForStoredProcedure = true;
                    });

                options
                    .AddHttpClientInstrumentation(httpClientOptions =>
                    {
                        httpClientOptions.FilterHttpRequestMessage = (request) =>
                        {
                            return !request.RequestUri.AbsoluteUri.Contains("9200", StringComparison.InvariantCulture);
                        };

                        httpClientOptions.EnrichWithHttpRequestMessage = async (activity, request) =>
                        {
                            var requestContent = "empty";

                            if (request.Content is not null)
                            {
                                requestContent = await request.Content.ReadAsStringAsync();
                            }

                            activity.SetTag("http.request.body", requestContent);
                        };

                        httpClientOptions.EnrichWithHttpResponseMessage = async (activity, response) =>
                        {
                            if (response.Content is not null)
                            {
                                activity.SetTag("http.response.body", await response.Content.ReadAsStringAsync());
                            }
                        };
                    });

                options
                    .AddRedisInstrumentation(redisOptions =>
                    {
                        redisOptions.SetVerboseDatabaseStatements = true;
                    });

                options.AddConsoleExporter();
                options.AddOtlpExporter(); //Jaeger
            });
        }
    }
}