using Prometheus;

namespace Front.Metricas
{
    public static class MetricsRegistry
    {
        // Mide cuánto tarda cada endpoint
        public static readonly Histogram EndpointResponseTime = Metrics.CreateHistogram(
            "sistema_endpoint_response_time_seconds",
            "Tiempo de respuesta por endpoint (s)",
            new HistogramConfiguration
            {
                LabelNames = new[] { "endpoint", "method", "status_code" }
            });

        // Cuenta cuántos exámenes se crean
        public static readonly Counter ExamsCreated = Metrics.CreateCounter(
            "univ_exams_created_total",
            "Total de exámenes creados",
            new CounterConfiguration { LabelNames = new[] { "department", "course" } });

       
        // Guarda el porcentaje de asistencia
        public static readonly Gauge AttendanceRate = Metrics.CreateGauge(
            "univ_attendance_rate",
            "Porcentaje de asistencia (0–100)",
            new GaugeConfiguration { LabelNames = new[] { "course_id", "course_name", "period" } });
    }
}

