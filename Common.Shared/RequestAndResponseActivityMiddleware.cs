using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using System.Diagnostics;

namespace Common.Shared
{
    public class RequestAndResponseActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestAndResponseActivityMiddleware(RequestDelegate next)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await AddRequestBodyContentToActivityTag(context);
            await AddResponseBodyContentToActivityTag(context);
        }

        private async Task AddRequestBodyContentToActivityTag(HttpContext context)
        {
            context.Request.EnableBuffering();
            StreamReader requestBodyStreamReader = new StreamReader(context.Request.Body);
            string requestBodyContent = await requestBodyStreamReader.ReadToEndAsync();
            Activity.Current?.SetTag("http.request.body", requestBodyContent);
            context.Request.Body.Position = 0;
        }

        private async Task AddResponseBodyContentToActivityTag(HttpContext context)
        {
            Stream originalResponse = context.Response.Body;
            await using var responseBodyMemoryStream = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBodyMemoryStream;
            await _next(context);
            responseBodyMemoryStream.Position = 0;
            StreamReader responseBodyStreamReader = new(responseBodyMemoryStream);
            string responseBodyContent = await responseBodyStreamReader.ReadToEndAsync();
            Activity.Current?.SetTag("http.response.body", responseBodyContent);
            responseBodyMemoryStream.Position = 0;
            await responseBodyMemoryStream.CopyToAsync(originalResponse);
        }
    }
}
