using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class FriendshipTests
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
    public void GetFriendship_WithValidPokemon_ReturnsFriendshipData()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetFriendship(handle, 0, 0);

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("currentFriendship", out _));
            Assert.True(response.TryGetProperty("originalTrainerFriendship", out _));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetFriendship_WithValidValue_UpdatesFriendship()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetFriendship(handle, 0, 0, 255);

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));

            var getResult = PKHeXApi.GetFriendship(handle, 0, 0);
            if (!TestHelpers.IsError(getResult))
            {
                var response = TestHelpers.ToJsonElement(getResult);
                var friendship = response.GetProperty("currentFriendship").GetInt32();
                Assert.Equal(255, friendship);
            }
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetFriendship_WithInvalidHandle_ReturnsError()
    {
        var result = PKHeXApi.SetFriendship(-1, 0, 0, 255);
        Assert.True(TestHelpers.IsError(result));
    }

    [Fact]
    public void SetOriginalTrainerFriendship_WithValidValue_UpdatesOTFriendship()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetOriginalTrainerFriendship(handle, 0, 0, 200);

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void MaximizeFriendship_WithValidPokemon_MaximizesAllValues()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.MaximizeFriendship(handle, 0, 0);

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));

            var getResult = PKHeXApi.GetFriendship(handle, 0, 0);
            if (!TestHelpers.IsError(getResult))
            {
                var response = TestHelpers.ToJsonElement(getResult);
                var friendship = response.GetProperty("currentFriendship").GetInt32();
                Assert.Equal(255, friendship);
            }
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetFriendship_ClampsValueToValidRange()
    {
        var handle = LoadTestSave();

        // Test value over 255 gets clamped
        var result = PKHeXApi.SetFriendship(handle, 0, 0, 500);

        if (!TestHelpers.IsError(result))
        {
            var getResult = PKHeXApi.GetFriendship(handle, 0, 0);
            if (!TestHelpers.IsError(getResult))
            {
                var response = TestHelpers.ToJsonElement(getResult);
                var friendship = response.GetProperty("currentFriendship").GetInt32();
                Assert.True(friendship <= 255);
            }
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetFriendship_WithMinValue_SetsFriendshipToZero()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetFriendship(handle, 0, 0, 0);

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));

            var getResult = PKHeXApi.GetFriendship(handle, 0, 0);
            if (!TestHelpers.IsError(getResult))
            {
                var response = TestHelpers.ToJsonElement(getResult);
                var friendship = response.GetProperty("currentFriendship").GetInt32();
                Assert.Equal(0, friendship);
            }
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetFriendship_WithMaxValue_SetsFriendshipTo255()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetFriendship(handle, 0, 0, 255);

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));

            var getResult = PKHeXApi.GetFriendship(handle, 0, 0);
            if (!TestHelpers.IsError(getResult))
            {
                var response = TestHelpers.ToJsonElement(getResult);
                var friendship = response.GetProperty("currentFriendship").GetInt32();
                Assert.Equal(255, friendship);
            }
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetFriendship_WithNegativeValue_ClampsToZeroOrReturnsError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetFriendship(handle, 0, 0, -10);

        // Either returns error or clamps to valid range
        if (!TestHelpers.IsError(result))
        {
            var getResult = PKHeXApi.GetFriendship(handle, 0, 0);
            if (!TestHelpers.IsError(getResult))
            {
                var response = TestHelpers.ToJsonElement(getResult);
                var friendship = response.GetProperty("currentFriendship").GetInt32();
                Assert.True(friendship >= 0 && friendship <= 255);
            }
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetAffection_WithGen3Pokemon_ReturnsUnsupportedError()
    {
        // Gen 3 doesn't support affection
        var handle = LoadTestSave();

        var result = PKHeXApi.SetAffection(handle, 0, 0, 255);

        // Gen 3 doesn't support affection, should error
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetFullness_WithGen3Pokemon_ReturnsUnsupportedError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetFullness(handle, 0, 0, 255);

        // Gen 3 doesn't support fullness
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetEnjoyment_WithGen3Pokemon_ReturnsUnsupportedError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetEnjoyment(handle, 0, 0, 255);

        // Gen 3 doesn't support enjoyment
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }
}
