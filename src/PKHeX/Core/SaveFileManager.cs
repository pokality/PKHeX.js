using PKHeX.Core;

namespace PKHeX.Core;

public static class SaveFileManager
{
    private static readonly Dictionary<int, SaveFileEntry> _saves = new();
    private static int _nextHandle = 1;
    private static readonly TimeSpan _inactivityTimeout = TimeSpan.FromMinutes(30);

    private record SaveFileEntry(SaveFile Save, DateTime LastAccessed);

    public static int CreateHandle(SaveFile save)
    {
        CleanupStaleHandles();
        var handle = _nextHandle++;
        _saves[handle] = new SaveFileEntry(save, DateTime.UtcNow);
        return handle;
    }

    public static SaveFile? GetSave(int handle)
    {
        if (_saves.TryGetValue(handle, out var entry))
        {
            _saves[handle] = entry with { LastAccessed = DateTime.UtcNow };
            return entry.Save;
        }
        return null;
    }

    public static bool RemoveHandle(int handle)
    {
        return _saves.Remove(handle);
    }

    public static void Clear()
    {
        _saves.Clear();
        _nextHandle = 1;
    }

    public static int GetActiveHandleCount()
    {
        return _saves.Count;
    }

    private static void CleanupStaleHandles()
    {
        var now = DateTime.UtcNow;
        var staleHandles = _saves
            .Where(kvp => now - kvp.Value.LastAccessed > _inactivityTimeout)
            .Select(kvp => kvp.Key)
            .ToList();

        if (staleHandles.Count > 0)
        {
            foreach (var handle in staleHandles)
            {
                _saves.Remove(handle);
            }

            if (Environment.GetEnvironmentVariable("PKHEX_LOG_CLEANUP") == "1")
            {
                Console.WriteLine($"[PKHeX.js] Auto-disposed {staleHandles.Count} inactive save handle(s) after {_inactivityTimeout.TotalMinutes} minutes of inactivity");
            }
        }
    }
}
