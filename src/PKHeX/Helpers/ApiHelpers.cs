using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using PKHeX.Core;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Helpers;

public static partial class ApiHelpers
{
    [RequiresUnreferencedCode("JSON serialization may require types that cannot be statically analyzed")]
    public static string ExecuteWithErrorHandling<T>(Func<T> action)
    {
        try
        {
            var result = action()!;
            return JsonSerializer.Serialize(result, JsonContext.Default.Options);
        }
        catch (ValidationException ex)
        {
            return JsonSerializer.Serialize(new ErrorResponse(ex.Message, ex.Code ?? VALIDATION_ERROR), JsonContext.Default.Options);
        }
        catch (ArgumentException ex)
        {
            return JsonSerializer.Serialize(new ErrorResponse(ex.Message, INVALID_ARGUMENT), JsonContext.Default.Options);
        }
        catch (InvalidOperationException ex)
        {
            return JsonSerializer.Serialize(new ErrorResponse(ex.Message, INVALID_OPERATION), JsonContext.Default.Options);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new ErrorResponse(ex.Message, INTERNAL_ERROR), JsonContext.Default.Options);
        }
    }

    public static void ValidateHandle(int handle)
    {
        if (handle <= 0)
            throw new ValidationException("Handle cannot be zero", INVALID_HANDLE);
    }

    public static SaveFile GetValidatedSave(int handle)
    {
        ValidateHandle(handle);
        var save = SaveFileManager.GetSave(handle);
        if (save == null)
            throw new ValidationException("Invalid save file handle", INVALID_HANDLE);
        return save;
    }

    public static void ValidateBox(SaveFile save, int box)
    {
        if (box < 0 || box >= save.BoxCount)
            throw new ValidationException($"Box {box} is out of range (0-{save.BoxCount - 1})", INVALID_BOX);
    }

    public static void ValidateSlot(SaveFile save, int slot)
    {
        if (slot < 0 || slot >= save.BoxSlotCount)
            throw new ValidationException($"Slot {slot} is out of range (0-{save.BoxSlotCount - 1})", INVALID_SLOT);
    }

    public static PKM GetValidatedPokemon(SaveFile save, int box, int slot)
    {
        ValidateBox(save, box);
        ValidateSlot(save, slot);
        var pk = save.GetBoxSlotAtIndex(box, slot);
        if (pk.Species == 0)
            throw new ValidationException($"No Pokemon in box {box} slot {slot}", EMPTY_SLOT);
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

    public static void ValidateSpeciesId(int species, string context = "Species")
    {
        if (species < 0 || species > 1025)
            throw new ValidationException(
                $"{context} {species} is out of valid range (0-1025)",
                INVALID_SPECIES);
    }

    public static void ValidateMove(int move, string context = "Move")
    {
        if (move < 0 || move > 919)
            throw new ValidationException(
                $"{context} {move} is out of valid range (0-919)",
                INVALID_MOVE);
    }

    public static void ValidateAbility(int ability, string context = "Ability")
    {
        if (ability < 0 || ability > 307)
            throw new ValidationException(
                $"{context} {ability} is out of valid range (0-307)",
                INVALID_ABILITY);
    }

    public static void ValidateItem(int item, string context = "Item")
    {
        if (item < 0 || item > 2400)
            throw new ValidationException(
                $"{context} {item} is out of valid range (0-2400)",
                INVALID_ITEM);
    }

    public static void ValidateNature(int nature, string context = "Nature")
    {
        if (nature < 0 || nature > 24)
            throw new ValidationException(
                $"{context} {nature} is out of range (0-24)",
                INVALID_NATURE);
    }

    public static void ValidateShinyType(int shinyType, string context = "Shiny type")
    {
        if (shinyType < 0 || shinyType > 3)
            throw new ValidationException(
                $"{context} {shinyType} is out of range (0-3)",
                INVALID_SHINY_TYPE);
    }

    public static object SuccessMessage(string message)
    {
        return new { success = true, message };
    }


    [RequiresUnreferencedCode("JSON serialization may require types that cannot be statically analyzed")]
    public static string SerializeSuccess<T>(T data)
    {
        return System.Text.Json.JsonSerializer.Serialize(new { success = true, data }, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }
}
