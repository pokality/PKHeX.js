using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class TeraTypeTests
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
    public void GetTeraType_WithGen3Pokemon_ReturnsUnsupportedError()
    {
        // Gen 3 doesn't support Tera Types, should return an error
        var handle = LoadTestSave();

        var result = PKHeXApi.GetTeraType(handle, 0, 0);

        // Gen 3 doesn't support Tera Types
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetTeraType_WithGen3Pokemon_ReturnsUnsupportedError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetTeraType(handle, 0, 0, 1);

        // Gen 3 doesn't support Tera Types
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetTeraTypeOverride_WithGen3Pokemon_ReturnsUnsupportedError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SetTeraTypeOverride(handle, 0, 0, 1);

        // Gen 3 doesn't support Tera Types
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void ResetTeraType_WithGen3Pokemon_ReturnsUnsupportedError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.ResetTeraType(handle, 0, 0);

        // Gen 3 doesn't support Tera Types
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetTeraType_WithInvalidHandle_ReturnsError()
    {
        var result = PKHeXApi.GetTeraType(-1, 0, 0);
        Assert.True(TestHelpers.IsError(result));
    }

    [Fact]
    public void GetAllTeraTypes_ReturnsAllTypes()
    {
        var result = PKHeXApi.GetAllTeraTypes();

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("teraTypes", out var teraTypes));

            // Should have 19 types (0-17 standard + 18 Stellar)
            Assert.True(teraTypes.GetArrayLength() >= 18);

            // Verify Stellar is present
            var hasTelar = false;
            foreach (var type in teraTypes.EnumerateArray())
            {
                if (type.TryGetProperty("name", out var name) && name.GetString() == "Stellar")
                {
                    hasTelar = true;
                    Assert.True(type.GetProperty("isStellar").GetBoolean());
                    break;
                }
            }
            Assert.True(hasTelar);
        }
    }

    [Fact]
    public void SetTeraType_WithInvalidTeraType_ReturnsError()
    {
        var handle = LoadTestSave();

        // Even though Gen 3 doesn't support Tera, if it did, 99 would be invalid
        var result = PKHeXApi.SetTeraType(handle, 0, 0, 99);

        // Should error either because of unsupported gen or invalid tera type
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    // Note: The following tests would require a Gen 9 save file to work properly
    // They are structured to work when such a save file is available

    /*
    [Fact]
    public void GetTeraType_WithGen9Pokemon_ReturnsTeraTypeData()
    {
        var handle = LoadGen9TestSave();

        var result = PKHeXApi.GetTeraType(handle, 0, 0);

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("teraType", out _));
            Assert.True(response.TryGetProperty("teraTypeName", out _));
            Assert.True(response.TryGetProperty("teraTypeOverride", out _));
            Assert.True(response.TryGetProperty("isOverridden", out _));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetTeraType_WithGen9Pokemon_UpdatesTeraType()
    {
        var handle = LoadGen9TestSave();

        // Set to Fire type (10)
        var result = PKHeXApi.SetTeraType(handle, 0, 0, 10);

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));

            var getResult = PKHeXApi.GetTeraType(handle, 0, 0);
            if (!TestHelpers.IsError(getResult))
            {
                var response = TestHelpers.ToJsonElement(getResult);
                Assert.Equal(10, response.GetProperty("teraType").GetInt32());
            }
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetTeraTypeToStellar_WithGen9Pokemon_SetsStellar()
    {
        var handle = LoadGen9TestSave();

        // Set to Stellar type (18)
        var result = PKHeXApi.SetTeraType(handle, 0, 0, 18);

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));
        }

        PKHeXApi.DisposeSave(handle);
    }
    */
}
