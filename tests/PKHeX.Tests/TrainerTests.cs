using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class TrainerTests
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
    public void GetTrainerInfo_WithValidSession_ReturnsTrainerData()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetTrainerInfo(handle);
        var response = TestHelpers.ToJsonElement(result);

        Assert.False(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("ot", out _) || response.TryGetProperty("OT", out _));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetTrainerInfo_WithInvalidSession_ReturnsError()
    {
        var result = PKHeXApi.GetTrainerInfo(-1);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));
    }

    [Fact]
    public void SetTrainerInfo_WithValidData_UpdatesTrainer()
    {
        var handle = LoadTestSave();

        var trainerData = new
        {
            ot = "TESTOT",
            money = 999999
        };
        var trainerJson = JsonSerializer.Serialize(trainerData);

        var result = PKHeXApi.SetTrainerInfo(handle, trainerJson);
        var response = TestHelpers.ToJsonElement(result);

        Assert.False(response.TryGetProperty("code", out _));

        var getResult = PKHeXApi.GetTrainerInfo(handle);
        var getResponse = TestHelpers.ToJsonElement(getResult);

        if (!getResponse.TryGetProperty("code", out _))
        {
            if (getResponse.TryGetProperty("ot", out var ot))
            {
                Assert.Equal("TESTOT", ot.GetString());
            }
            else if (getResponse.TryGetProperty("OT", out var otUpper))
            {
                Assert.Equal("TESTOT", otUpper.GetString());
            }
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetTrainerInfo_WithInvalidSession_ReturnsError()
    {
        var trainerData = new { ot = "TEST" };
        var trainerJson = JsonSerializer.Serialize(trainerData);

        var result = PKHeXApi.SetTrainerInfo(-1, trainerJson);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));
    }

    [Fact]
    public void SetTrainerInfo_WithInvalidJson_ReturnsError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetTrainerInfo(handle, "invalid-json");
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));

        PKHeXApi.DisposeSave(handle);
    }
}
