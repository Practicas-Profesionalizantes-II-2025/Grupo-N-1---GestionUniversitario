using System;
using System.Collections.Concurrent;

public class ActiveUserTracker
{
    // key: session id o user id; value: last activity UTC
    private ConcurrentDictionary<string, DateTime> _sessions = new();

    public void MarkActivity(string sessionId)
    {
        _sessions[sessionId] = DateTime.UtcNow;
    }

    public int CountActive(TimeSpan activeWindow)
    {
        var cutoff = DateTime.UtcNow - activeWindow;
        return _sessions.Values.Count(t => t >= cutoff);
    }

    public void CleanupOlderThan(TimeSpan ttl)
    {
        var cutoff = DateTime.UtcNow - ttl;
        foreach (var kv in _sessions)
        {
            if (kv.Value < cutoff)
                _sessions.TryRemove(kv.Key, out _);
        }
    }

    // si querés exponer la lista de sessions para debug
    public IEnumerable<string> GetSessionIds() => _sessions.Keys;
}