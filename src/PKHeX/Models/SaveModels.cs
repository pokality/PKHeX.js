namespace PKHeX.Models;

public record SaveFileInfo(
    string Generation,
    string GameVersion,
    string OT,
    uint TID,
    uint SID,
    int BoxCount
);

public record SaveFileHandle(
    int Handle
);
