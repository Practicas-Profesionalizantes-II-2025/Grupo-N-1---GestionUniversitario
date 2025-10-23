using Front.Metricas;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ActiveUsersCleanupService : BackgroundService
{
    private readonly ActiveUserTracker _tracker;
    private readonly TimeSpan _activeWindow = TimeSpan.FromMinutes(5); // qué consideramos "activo"
    private readonly TimeSpan _cleanupTtl = TimeSpan.FromHours(1);

    public ActiveUsersCleanupService(ActiveUserTracker tracker)
    {
        _tracker = tracker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // limpiar sessions viejas
            _tracker.CleanupOlderThan(_cleanupTtl);

            // contar activos y setear gauge
            var activeCount = _tracker.CountActive(_activeWindow);
            DefinicionesMetricas.UsuariosActivos.Set(activeCount);

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // cada 10s actualiza
        }
    }
}
