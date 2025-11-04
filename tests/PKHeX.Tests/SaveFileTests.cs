using PKHeX.Api;
using PKHeX.Core;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class SaveFileTests
{
    private readonly string _validGen3SavePath = Path.Combine("TestData", "emerald.sav");

    [Fact]
    public void LoadSave_WithValidData_ReturnsSessionId()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);

        var result = PKHeXApi.LoadSave(base64Data);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsSuccess(result));
        Assert.True(response.TryGetProperty("handle", out var handle));
        var handleValue = handle.GetInt32();
        Assert.True(handleValue > 0);

        PKHeXApi.DisposeSave(handleValue);
    }

    [Fact]
    public void LoadSave_WithInvalidData_ReturnsError()
    {
        var invalidData = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 });

        var result = PKHeXApi.LoadSave(invalidData);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(TestHelpers.TryGetProperty(result, "error", out _));
        Assert.True(TestHelpers.TryGetProperty(result, "code", out _));
    }

    [Fact]
    public void LoadSave_WithEmptyData_ReturnsError()
    {
        var result = PKHeXApi.LoadSave("");

        Assert.True(TestHelpers.IsError(result));
        Assert.True(TestHelpers.TryGetProperty(result, "error", out _));
    }

    [Fact]
    public void GetSaveInfo_WithValidSession_ReturnsMetadata()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);
        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32()!;

        var result = PKHeXApi.GetSaveInfo(handle);

        Assert.True(TestHelpers.IsSuccess(result));
        Assert.True(TestHelpers.TryGetProperty(result, "generation", out _));
        Assert.True(TestHelpers.TryGetProperty(result, "gameVersion", out _));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetSaveInfo_WithInvalidSession_ReturnsError()
    {
        var result = PKHeXApi.GetSaveInfo(-1);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(TestHelpers.TryGetProperty(result, "error", out _));
    }

    [Fact]
    public void ExportSave_WithValidSession_ReturnsBase64Data()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);
        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32()!;

        var result = PKHeXApi.ExportSave(handle);

        Assert.True(TestHelpers.IsSuccess(result));
        Assert.True(TestHelpers.TryGetProperty(result, "base64Data", out var exportedData));
        Assert.NotEmpty(exportedData.GetString()!);

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void DisposeSave_WithValidSession_RemovesSession()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);
        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32()!;

        var disposeResult = PKHeXApi.DisposeSave(handle);

        Assert.True(TestHelpers.IsSuccess(disposeResult));

        var infoResult = PKHeXApi.GetSaveInfo(handle);
        Assert.True(TestHelpers.IsError(infoResult));
    }

    [Fact]
    public void SaveFileHandleLifecycle_MultipleOperations_WorksCorrectly()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);

        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32()!;

        var infoResult = PKHeXApi.GetSaveInfo(handle);
        Assert.True(TestHelpers.IsSuccess(infoResult));

        var trainerResult = PKHeXApi.GetTrainerInfo(handle);
        Assert.True(TestHelpers.IsSuccess(trainerResult));

        var exportResult = PKHeXApi.ExportSave(handle);
        Assert.True(TestHelpers.IsSuccess(exportResult));

        PKHeXApi.DisposeSave(handle);

        var afterDisposeResult = PKHeXApi.GetSaveInfo(handle);
        Assert.True(TestHelpers.IsError(afterDisposeResult));
    }
}
