using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public class SaveOpsImpl : ISaveOps
{
    public static uint LoadSave(byte[] data)
    {
        var save = SaveUtil.GetSaveFile(data);
        if (save == null)
            throw ImplHelpers.Validation("Unable to load save file: unrecognized format");

        var handle = SaveFileManager.CreateHandle(save);
        return (uint)handle;
    }

    public static ISaveOps.SaveInfo GetSaveInfo(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        return new ISaveOps.SaveInfo(
            generation: save.Generation.ToString(),
            gameVersion: save.Version.ToString(),
            ot: save.OT,
            displayTid: save.DisplayTID,
            displaySid: save.DisplaySID,
            boxCount: save.BoxCount
        );
    }

    public static byte[] ExportSave(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        return save.Write().ToArray();
    }

    public static void DisposeSave(uint handle)
    {
        SaveFileManager.RemoveHandle((int)handle);
    }

    public static ISaveOps.SaveRevision GetSaveRevision(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        if (save is not SAV9ZA sav9za)
            throw ImplHelpers.Unsupported("Save revision is only available in Legends Z-A saves");

        var revision = sav9za.SaveRevision;
        var revisionName = revision switch
        {
            0 => "Base",
            1 => "Mega Dimension",
            _ => $"Unknown ({revision})"
        };
        return new ISaveOps.SaveRevision(revision, revisionName);
    }

    public static uint GetActiveHandleCount()
    {
        return (uint)SaveFileManager.GetActiveHandleCount();
    }

    public static ISaveOps.PlayTime GetPlayTime(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        int totalSeconds = (save.PlayedHours * 3600) + (save.PlayedMinutes * 60) + save.PlayedSeconds;
        return new ISaveOps.PlayTime(
            save.PlayedHours,
            save.PlayedMinutes,
            save.PlayedSeconds,
            totalSeconds
        );
    }

    public static void SetPlayTime(uint handle, int hours, int minutes, int seconds)
    {
        var save = ImplHelpers.GetSave(handle);
        if (hours < 0)
            throw ImplHelpers.Validation("Hours must be non-negative");
        if (minutes < 0 || minutes >= 60)
            throw ImplHelpers.Validation("Minutes must be between 0 and 59");
        if (seconds < 0 || seconds >= 60)
            throw ImplHelpers.Validation("Seconds must be between 0 and 59");

        save.PlayedHours = hours;
        save.PlayedMinutes = minutes;
        save.PlayedSeconds = seconds;
    }

    public static uint GetSecondsToStart(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        return save.SecondsToStart;
    }

    public static void SetSecondsToStart(uint handle, uint seconds)
    {
        var save = ImplHelpers.GetSave(handle);
        save.SecondsToStart = seconds;
    }

    public static uint GetSecondsToFame(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        return save.SecondsToFame;
    }

    public static void SetSecondsToFame(uint handle, uint seconds)
    {
        var save = ImplHelpers.GetSave(handle);
        save.SecondsToFame = seconds;
    }
}
