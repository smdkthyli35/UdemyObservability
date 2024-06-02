using Microsoft.AspNetCore.Builder;

namespace Logging.Shared
{
    public static class OpenTelemetryTraceIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseOpenTelemetryTraceIdExtension(this IApplicationBuilder app)
            => app.UseMiddleware<OpenTelemetryTraceIdMiddleware>();
    }
}