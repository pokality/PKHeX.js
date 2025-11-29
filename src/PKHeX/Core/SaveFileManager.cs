using PKHeX.Core;

namespace PKHeX.Core;

public static class SaveFileManager
{
    private static readonly Dictionary<int, SaveFileEntry> _saves = new();
    private static int _nextHandle = 1;
    private static readonly object _lock = new();
    private static readonly TimeSpan _inactivityTimeout = TimeSpan.FromMinutes(30);
    private const int MaxHandle = int.MaxValue - 1000; // Leave buffer before overflow

    private record SaveFileEntry(SaveFile Save, DateTime LastAccessed);

    public static int CreateHandle(SaveFile save)
    {
        lock (_lock)
        {
            CleanupStaleHandles();

            // Handle overflow protection: recycle handles when approaching max
            if (_nextHandle >= MaxHandle)
            {
                _nextHandle = FindNextAvailableHandle();
            }

            var handle = _nextHandle++;
            _saves[handle] = new SaveFileEntry(save, DateTime.UtcNow);
            return handle;
        }
    }

    /// <summary>
    /// Finds the next available handle starting from 1.
    /// Used when _nextHandle approaches int.MaxValue to recycle old handles.
    /// </summary>
    private static int FindNextAvailableHandle()
    {
        for (int candidate = 1; candidate < MaxHandle; candidate++)
        {
            if (!_saves.ContainsKey(candidate))
                return candidate;
        }

        // If all handles are in use (extremely unlikely), throw
        throw new InvalidOperationException("Maximum number of save file handles reached");
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

        foreach (var handle in staleHandles)
        {
            _saves.Remove(handle);
        }
    }
}
