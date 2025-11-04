using PKHeX.Core;

namespace PKHeX.Core;

public static class SaveFileManager
{
    private static readonly Dictionary<int, SaveFile> _saves = new();
    private static int _nextHandle = 1;

    public static int CreateHandle(SaveFile save)
    {
        var handle = _nextHandle++;
        _saves[handle] = save;
        return handle;
    }

    public static SaveFile? GetSave(int handle)
    {
        return _saves.TryGetValue(handle, out var save) ? save : null;
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
}
