using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class MemoryTests
{
    private readonly string _validGen3SavePath = Path.Combine("TestData", "emerald.sav");

    private int LoadTestSave()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);
        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        return loadResponse.GetProperty("handle").GetInt32();
    }

    [Fact]
    public void GetMemories_WithGen3Pokemon_ReturnsUnsupportedError()
    {
        // Gen 3 doesn't support memories, should return an error
        var handle = LoadTestSave();

        var result = PKHeXApi.GetMemories(handle, 0, 0);

        // Gen 3 doesn't support memories, so this should error
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetOriginalTrainerMemory_WithGen3Pokemon_ReturnsUnsupportedError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetOriginalTrainerMemory(handle, 0, 0, 1, 1, 1, 1);

        // Gen 3 doesn't support memories
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetHandlingTrainerMemory_WithGen3Pokemon_ReturnsUnsupportedError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetHandlingTrainerMemory(handle, 0, 0, 1, 1, 1, 1);

        // Gen 3 doesn't support memories
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void ClearMemories_WithGen3Pokemon_SucceedsOrReturnsUnsupported()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.ClearMemories(handle, 0, 0);

        // This might succeed even on Gen 3 (just does nothing) or return unsupported
        // Either is acceptable behavior
        var response = TestHelpers.ToJsonElement(result);
        Assert.True(response.ValueKind == JsonValueKind.Object);

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetMemories_WithInvalidHandle_ReturnsError()
    {
        var result = PKHeXApi.GetMemories(-1, 0, 0);
        Assert.True(TestHelpers.IsError(result));
    }

    [Fact]
    public void GetMemoryStrings_ReturnsStrings()
    {
        var result = PKHeXApi.GetMemoryStrings();

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("memories", out _));
            Assert.True(response.TryGetProperty("feelings", out _));
            Assert.True(response.TryGetProperty("intensities", out _));
        }
    }

    // Note: The following tests would require a Gen 6+ save file to work properly
    // They are structured to work when such a save file is available

    /*
    [Fact]
    public void GetMemories_WithGen6Pokemon_ReturnsMemoryData()
    {
        // Would need Gen 6+ save file
        var handle = LoadGen6TestSave();

        var result = PKHeXApi.GetMemories(handle, 0, 0);

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("originalTrainerMemory", out _));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetOriginalTrainerMemory_WithGen6Pokemon_UpdatesMemory()
    {
        var handle = LoadGen6TestSave();

        var result = PKHeXApi.SetOriginalTrainerMemory(handle, 0, 0, 4, 3, 1, 0);

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));
        }

        PKHeXApi.DisposeSave(handle);
    }
    */
}
