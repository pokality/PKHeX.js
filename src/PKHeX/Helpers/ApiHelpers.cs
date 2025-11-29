using System.Diagnostics.CodeAnalysis;
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
        catch (IndexOutOfRangeException ex)
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

    public static SaveFile GetValidatedSave(int handle)
    {
        if (handle <= 0)
            throw new ValidationException("Handle cannot be zero", INVALID_HANDLE);
        var save = SaveFileManager.GetSave(handle);
        if (save == null)
            throw new ValidationException("Invalid save file handle", INVALID_HANDLE);
        return save;
    }

    public static PKM GetValidatedPokemon(SaveFile save, int box, int slot)
    {
        var pk = save.GetBoxSlotAtIndex(box, slot);
        if (pk.Species == 0)
            throw new ValidationException($"No Pokemon in box {box} slot {slot}", EMPTY_SLOT);
        return pk;
    }
}
