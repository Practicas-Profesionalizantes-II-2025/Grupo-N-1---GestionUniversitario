using Prometheus;
using System.Collections.Concurrent;

namespace Front.Metricas
{
    public static class DefinicionesMetricas
    {
        // contador para inscripciones (solo incrementa)
        public static readonly Counter InscripcionesCounter = Metrics
            .CreateCounter("miapp_inscripciones_total", "Cantidad total de inscripciones realizadas", new CounterConfiguration
            {
                LabelNames = new[] { "materia" }
            });

        // gauge para usuarios activos
        public static readonly Gauge UsuariosActivos = Metrics
            .CreateGauge("miapp_active_users", "Usuarios activos conectados actualmente");

        // histograma para latencia de peticiones http
        public static readonly Histogram HttpRequestDuration = Metrics
            .CreateHistogram("miapp_http_request_duration_seconds", "Duración de peticiones HTTP", new HistogramConfiguration
            {
                // buckets ejemplo en segundos
                Buckets = Histogram.LinearBuckets(start: 0.01, width: 0.05, count: 20)
            });
    }
}