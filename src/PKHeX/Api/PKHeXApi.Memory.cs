using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;

namespace PKHeX.Api;

// Memory System Operations (Gen 6+)
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMemories(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not IMemoryOT otMemory)
                throw new ValidationException("This Pokemon does not support memories (Gen 6+ only)", "UNSUPPORTED_FEATURE");

            MemoryInfo? htMemory = null;
            if (pk is IMemoryHT htMem)
            {
                htMemory = new MemoryInfo(
                    htMem.HandlingTrainerMemory,
                    htMem.HandlingTrainerMemoryIntensity,
                    htMem.HandlingTrainerMemoryFeeling,
                    htMem.HandlingTrainerMemoryVariable,
                    GetMemoryText(pk, htMem.HandlingTrainerMemory, htMem.HandlingTrainerMemoryIntensity,
                        htMem.HandlingTrainerMemoryFeeling, htMem.HandlingTrainerMemoryVariable, false)
                );
            }

            return new MemoriesData(
                new MemoryInfo(
                    otMemory.OriginalTrainerMemory,
                    otMemory.OriginalTrainerMemoryIntensity,
                    otMemory.OriginalTrainerMemoryFeeling,
                    otMemory.OriginalTrainerMemoryVariable,
                    GetMemoryText(pk, otMemory.OriginalTrainerMemory, otMemory.OriginalTrainerMemoryIntensity,
                        otMemory.OriginalTrainerMemoryFeeling, otMemory.OriginalTrainerMemoryVariable, true)
                ),
                htMemory
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetOriginalTrainerMemory(int handle, int box, int slot, int memoryId, int intensity, int feeling, int variable)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not IMemoryOT otMemory)
                throw new ValidationException("This Pokemon does not support memories (Gen 6+ only)", "UNSUPPORTED_FEATURE");

            otMemory.OriginalTrainerMemory = (byte)Math.Clamp(memoryId, 0, 255);
            otMemory.OriginalTrainerMemoryIntensity = (byte)Math.Clamp(intensity, 0, 7);
            otMemory.OriginalTrainerMemoryFeeling = (byte)Math.Clamp(feeling, 0, 24);
            otMemory.OriginalTrainerMemoryVariable = (ushort)Math.Clamp(variable, 0, 65535);

            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Original trainer memory updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetHandlingTrainerMemory(int handle, int box, int slot, int memoryId, int intensity, int feeling, int variable)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not IMemoryHT htMemory)
                throw new ValidationException("This Pokemon does not support handling trainer memories (Gen 6+ only)", "UNSUPPORTED_FEATURE");

            htMemory.HandlingTrainerMemory = (byte)Math.Clamp(memoryId, 0, 255);
            htMemory.HandlingTrainerMemoryIntensity = (byte)Math.Clamp(intensity, 0, 7);
            htMemory.HandlingTrainerMemoryFeeling = (byte)Math.Clamp(feeling, 0, 24);
            htMemory.HandlingTrainerMemoryVariable = (ushort)Math.Clamp(variable, 0, 65535);

            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Handling trainer memory updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ClearMemories(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is IMemoryOT otMemory)
            {
                otMemory.OriginalTrainerMemory = 0;
                otMemory.OriginalTrainerMemoryIntensity = 0;
                otMemory.OriginalTrainerMemoryFeeling = 0;
                otMemory.OriginalTrainerMemoryVariable = 0;
            }

            if (pk is IMemoryHT htMemory)
            {
                htMemory.HandlingTrainerMemory = 0;
                htMemory.HandlingTrainerMemoryIntensity = 0;
                htMemory.HandlingTrainerMemoryFeeling = 0;
                htMemory.HandlingTrainerMemoryVariable = 0;
            }

            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Memories cleared successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMemoryStrings()
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var memories = GameInfo.Strings.memories;
            var feelings = GameInfo.Strings.GetMemoryFeelings(EntityContext.Gen8);
            var intensities = GameInfo.Strings.GetMemoryQualities(EntityContext.Gen8);

            var memoryList = new List<NamedEntity>();
            for (int i = 0; i < memories.Length; i++)
            {
                if (!string.IsNullOrEmpty(memories[i]))
                    memoryList.Add(new NamedEntity(i, memories[i]));
            }

            var feelingList = new List<NamedEntity>();
            for (int i = 0; i < feelings.Length; i++)
            {
                if (!string.IsNullOrEmpty(feelings[i]))
                    feelingList.Add(new NamedEntity(i, feelings[i]));
            }

            var intensityList = new List<NamedEntity>();
            for (int i = 0; i < intensities.Length; i++)
            {
                if (!string.IsNullOrEmpty(intensities[i]))
                    intensityList.Add(new NamedEntity(i, intensities[i]));
            }

            return new MemoryStringsData(memoryList, feelingList, intensityList);
        });
    }

    private static string GetMemoryText(PKM pk, int memoryId, int intensity, int feeling, int variable, bool isOT)
    {
        if (memoryId == 0)
            return string.Empty;

        try
        {
            var strings = GameInfo.Strings;
            var memories = strings.memories;

            if (memoryId >= memories.Length)
                return $"Memory {memoryId}";

            return memories[memoryId];
        }
        catch
        {
            return $"Memory {memoryId}";
        }
    }
}
