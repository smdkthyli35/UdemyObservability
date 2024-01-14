﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
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

                        aspNetCoreOptions.RecordException = true;
                    });

                options.AddConsoleExporter();
                options.AddOtlpExporter(); //Jaeger
            });
        }
    }
}