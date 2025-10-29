using Front.Metricas;
using Microsoft.AspNetCore.Http;
using Prometheus;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Front.Metricas
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTimingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            var endpoint = context.Request.Path.Value ?? "unknown";
            var method = context.Request.Method;
            var status = context.Response.StatusCode.ToString();

            MetricsRegistry.EndpointResponseTime
                .WithLabels(endpoint, method, status)
                .Observe(sw.Elapsed.TotalSeconds);
        }
    }
}

