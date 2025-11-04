using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class PokedexTests
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
    public void GetPokedex_WithValidSession_ReturnsPokedexData()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetPokedex(handle);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(response.ValueKind == JsonValueKind.Array);

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetPokedex_WithInvalidSession_ReturnsError()
    {
        var result = PKHeXApi.GetPokedex(-1);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));
    }

    [Fact]
    public void SetPokedexSeen_WithValidSpecies_UpdatesPokedex()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetPokedexSeen(handle, 25, 0);
        var response = TestHelpers.ToJsonElement(result);

        Assert.False(response.TryGetProperty("code", out _));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetPokedexCaught_WithValidSpecies_UpdatesPokedex()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetPokedexCaught(handle, 25, 0);
        var response = TestHelpers.ToJsonElement(result);

        Assert.False(response.TryGetProperty("code", out _));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetPokedexSeen_WithInvalidSession_ReturnsError()
    {
        var result = PKHeXApi.SetPokedexSeen(-1, 25, 0);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));
    }

    [Fact]
    public void PokedexOperations_MultipleUpdates_WorkCorrectly()
    {
        var handle = LoadTestSave();

        var seenResult = PKHeXApi.SetPokedexSeen(handle, 1, 0);
        var seenResponse = TestHelpers.ToJsonElement(seenResult);
        Assert.False(seenResponse.TryGetProperty("code", out _));

        var caughtResult = PKHeXApi.SetPokedexCaught(handle, 1, 0);
        var caughtResponse = TestHelpers.ToJsonElement(caughtResult);
        Assert.False(caughtResponse.TryGetProperty("code", out _));

        var getResult = PKHeXApi.GetPokedex(handle);
        var getResponse = TestHelpers.ToJsonElement(getResult);
        Assert.True(getResponse.ValueKind == JsonValueKind.Array);

        PKHeXApi.DisposeSave(handle);
    }
}
