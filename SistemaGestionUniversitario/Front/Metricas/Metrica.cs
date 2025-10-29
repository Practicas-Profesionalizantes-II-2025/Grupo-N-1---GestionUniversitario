using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Prometheus;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Front.Metricas
{
    public static class MetricsRegistry
    {
        public static readonly Histogram HttpRequestDuration = Metrics.CreateHistogram(
            "univ_http_request_duration_seconds",
            "Histogram of HTTP request durations in seconds.",
            new HistogramConfiguration
            {
                Buckets = new double[] { 0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1, 2.5, 5, 10 },
                LabelNames = new[] { "method", "route", "status" }
            });
    }

    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestTimingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();

                // Método HTTP
                var method = context.Request?.Method ?? "UNKNOWN";

                // Código de estado (puede ser 0 si no se ha escrito)
                var status = context.Response?.StatusCode.ToString() ?? "0";

                // Intentar obtener route pattern: /api/exams/{id} en lugar de /api/exams/5
                string route = "unknown";
                var endpoint = context.GetEndpoint();
                if (endpoint != null)
                {
                    if (endpoint is RouteEndpoint routeEndpoint)
                    {
                        route = routeEndpoint.RoutePattern?.RawText ?? endpoint.DisplayName ?? "unknown";
                    }
                    else
                    {
                        route = endpoint.DisplayName ?? "unknown";
                    }
                }

                try
                {
                    MetricsRegistry.HttpRequestDuration
                        .WithLabels(method, route, status)
                        .Observe(sw.Elapsed.TotalSeconds);
                }
                catch
                {
                    // No queremos romper la pipeline por metricas
                }
            }
        }
    }
}
