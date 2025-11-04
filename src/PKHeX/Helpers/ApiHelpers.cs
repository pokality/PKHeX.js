using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Models;

namespace PKHeX.Helpers;

public static class ApiHelpers
{
    public static object ExecuteWithErrorHandling<T>(Func<T> action)
    {
        try
        {
            return action()!;
        }
        catch (ValidationException ex)
        {
            return new { error = ex.Message, code = ex.Code ?? "VALIDATION_ERROR" };
        }
        catch (ArgumentException ex)
        {
            return new { error = ex.Message, code = "INVALID_ARGUMENT" };
        }
        catch (InvalidOperationException ex)
        {
            return new { error = ex.Message, code = "INVALID_OPERATION" };
        }
        catch (Exception ex)
        {
            return new { error = ex.Message, code = "INTERNAL_ERROR" };
        }
    }

    public static void ValidateHandle(int handle)
    {
        if (handle <= 0)
            throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");
    }

    public static SaveFile GetValidatedSave(int handle)
    {
        ValidateHandle(handle);
        var save = SaveFileManager.GetSave(handle);
        if (save == null)
            throw new ValidationException("Invalid save file handle", "INVALID_HANDLE");
        return save;
    }

    public static void ValidateBox(SaveFile save, int box)
    {
        if (box < 0 || box >= save.BoxCount)
            throw new ValidationException($"Box {box} is out of range (0-{save.BoxCount - 1})", "INVALID_BOX");
    }

    public static void ValidateSlot(SaveFile save, int slot)
    {
        if (slot < 0 || slot >= save.BoxSlotCount)
            throw new ValidationException($"Slot {slot} is out of range (0-{save.BoxSlotCount - 1})", "INVALID_SLOT");
    }

    public static PKM GetValidatedPokemon(SaveFile save, int box, int slot)
    {
        ValidateBox(save, box);
        ValidateSlot(save, slot);
        var pk = save.GetBoxSlotAtIndex(box, slot);
        if (pk.Species == 0)
            throw new ValidationException($"No Pokemon in box {box} slot {slot}", "EMPTY_SLOT");
        return pk;
    }

    public static void ValidateNonNegative(int value, string name, string code)
    {
        if (value < 0)
            throw new ValidationException($"{name} {value} must be non-negative", code);
    }

    public static void ValidateRange(int value, int min, int max, string name, string code)
    {
        if (value < min || value > max)
            throw new ValidationException($"{name} {value} is out of range ({min}-{max})", code);
    }

    public static void ValidateNotEmpty(string value, string name, string code)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException($"{name} cannot be empty", code);
    }

    public static object SuccessMessage(string message)
    {
        return new { success = true, message };
    }

    // For test compatibility - serializes objects to JSON
    public static string SerializeSuccess<T>(T data)
    {
        return System.Text.Json.JsonSerializer.Serialize(new { success = true, data }, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }
}
