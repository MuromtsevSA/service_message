using Serilog;
using System.Diagnostics;

namespace service_message.Logger
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            Log.Information("Handling request: {Method} {Url}", context.Request.Method, context.Request.Path);

            await _next(context);

            stopwatch.Stop();

            Log.Information("Finished handling request: {Method} {Url} with status code {StatusCode} in {Elapsed:0.0000} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}
