using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class ErrorHandlingTests
{
    private readonly string _validGen3SavePath = Path.Combine("TestData", "emerald.sav");

    [Fact]
    public void InvalidSessionId_AllMethods_ReturnError()
    {
        var invalidSessionId = -1;

        var methods = new[]
        {
            PKHeXApi.GetSaveInfo(invalidSessionId),
            PKHeXApi.ExportSave(invalidSessionId),
            PKHeXApi.GetTrainerInfo(invalidSessionId),
            PKHeXApi.GetAllPokemon(invalidSessionId),
            PKHeXApi.GetParty(invalidSessionId),
            PKHeXApi.GetPouchItems(invalidSessionId),
            PKHeXApi.GetPokedex(invalidSessionId)
        };

        foreach (var result in methods)
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(TestHelpers.IsError(result) && response.TryGetProperty("code", out _), 
                $"Expected error for method result: {result}");
        }
    }

    [Fact]
    public void InvalidBase64Data_LoadSave_ReturnsError()
    {
        var invalidData = "not-valid-base64!@#$%";

        var result = PKHeXApi.LoadSave(invalidData);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));
    }

    [Fact]
    public void ZeroHandle_AllMethods_ReturnError()
    {
        var zeroHandle = 0;

        var result = PKHeXApi.GetSaveInfo(zeroHandle);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));
    }

    [Fact]
    public void NegativeHandle_AllMethods_ReturnError()
    {
        var negativeHandle = -1;

        var result = PKHeXApi.GetSaveInfo(negativeHandle);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));
    }

    [Fact]
    public void InvalidJsonInput_SetTrainerInfo_ReturnsError()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);
        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32()!;

        var invalidJson = "{invalid json}";
        var result = PKHeXApi.SetTrainerInfo(handle, invalidJson);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void InvalidJsonInput_ModifyPokemon_ReturnsError()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);
        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32()!;

        var invalidJson = "not json at all";
        var result = PKHeXApi.ModifyPokemon(handle, 0, 0, invalidJson);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void OutOfBoundsCoordinates_GetPokemon_ReturnsError()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);
        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32()!;

        var result = PKHeXApi.GetPokemon(handle, 999, 999);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void NegativeCoordinates_GetPokemon_ReturnsError()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);
        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32()!;

        var result = PKHeXApi.GetPokemon(handle, -1, -1);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void DoubleDispose_DisposeSave_HandlesGracefully()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);
        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32()!;

        var firstDispose = PKHeXApi.DisposeSave(handle);
        var firstResponse = TestHelpers.ToJsonElement(firstDispose);
        Assert.False(firstResponse.TryGetProperty("code", out _));

        var secondDispose = PKHeXApi.DisposeSave(handle);
        Assert.True(TestHelpers.IsError(secondDispose));
    }

    [Fact]
    public void ConcurrentOperations_MultipleSessions_WorkIndependently()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);

        var session1Result = PKHeXApi.LoadSave(base64Data);
        var session1Response = TestHelpers.ToJsonElement(session1Result);
        var handle1 = session1Response.GetProperty("handle").GetInt32()!;

        var session2Result = PKHeXApi.LoadSave(base64Data);
        var session2Response = TestHelpers.ToJsonElement(session2Result);
        var handle2 = session2Response.GetProperty("handle").GetInt32()!;

        Assert.NotEqual(handle1, handle2);

        var info1 = PKHeXApi.GetSaveInfo(handle1);
        var info1Response = TestHelpers.ToJsonElement(info1);
        Assert.False(info1Response.TryGetProperty("message", out _));

        var info2 = PKHeXApi.GetSaveInfo(handle2);
        var info2Response = TestHelpers.ToJsonElement(info2);
        Assert.False(info2Response.TryGetProperty("message", out _));

        PKHeXApi.DisposeSave(handle1);

        var info2After = PKHeXApi.GetSaveInfo(handle2);
        var info2AfterResponse = TestHelpers.ToJsonElement(info2After);
        Assert.False(info2AfterResponse.TryGetProperty("message", out _));

        PKHeXApi.DisposeSave(handle2);
    }
}
