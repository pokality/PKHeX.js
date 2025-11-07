namespace PKHeX.Models;

public static class ErrorCodes
{
    // Save File Operations
    public const string EMPTY_DATA = "EMPTY_DATA";
    public const string INVALID_BASE64 = "INVALID_BASE64";
    public const string INVALID_SAVE_FORMAT = "INVALID_SAVE_FORMAT";
    public const string INVALID_HANDLE = "INVALID_HANDLE";

    // Pokemon Operations
    public const string INVALID_BOX = "INVALID_BOX";
    public const string INVALID_SLOT = "INVALID_SLOT";
    public const string EMPTY_SLOT = "EMPTY_SLOT";
    public const string INVALID_PKM_DATA = "INVALID_PKM_DATA";
    public const string INVALID_SPECIES = "INVALID_SPECIES";
    public const string INVALID_NATURE = "INVALID_NATURE";
    public const string INVALID_PID = "INVALID_PID";
    public const string INVALID_MOVE = "INVALID_MOVE";
    public const string INVALID_ABILITY = "INVALID_ABILITY";
    public const string INVALID_ITEM = "INVALID_ITEM";
    public const string INVALID_SHINY_TYPE = "INVALID_SHINY_TYPE";
    public const string INVALID_LEVEL = "INVALID_LEVEL";
    public const string INVALID_IV = "INVALID_IV";
    public const string INVALID_IVS = "INVALID_IVS";
    public const string INVALID_EV = "INVALID_EV";
    public const string INVALID_EVS = "INVALID_EVS";

    // Data Operations
    public const string INVALID_JSON = "INVALID_JSON";
    public const string INVALID_ARGUMENT = "INVALID_ARGUMENT";
    public const string INVALID_OPERATION = "INVALID_OPERATION";
    public const string VALIDATION_ERROR = "VALIDATION_ERROR";

    // System Errors
    public const string INTERNAL_ERROR = "INTERNAL_ERROR";
}
