using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class ItemTests
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
    public void GetPouchItems_WithValidSession_ReturnsItemList()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetPouchItems(handle);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(response.ValueKind == JsonValueKind.Array);

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetPouchItems_WithInvalidSession_ReturnsError()
    {
        var result = PKHeXApi.GetPouchItems(-1);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));
    }

    [Fact]
    public void AddItemToPouch_WithValidItem_AddsItem()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.AddItemToPouch(handle, 1, 10, 0);
        var response = TestHelpers.ToJsonElement(result);

        Assert.False(response.TryGetProperty("code", out _));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void AddItemToPouch_WithInvalidSession_ReturnsError()
    {
        var result = PKHeXApi.AddItemToPouch(-1, 1, 10, 0);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));
    }

    [Fact]
    public void RemoveItemFromPouch_WithValidItem_RemovesItem()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.RemoveItemFromPouch(handle, 1, 1);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(response.ValueKind == JsonValueKind.Object);

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void ItemModifications_AddAndRemove_WorksCorrectly()
    {
        var handle = LoadTestSave();

        var addResult = PKHeXApi.AddItemToPouch(handle, 1, 5, 0);
        var addResponse = TestHelpers.ToJsonElement(addResult);
        Assert.True(addResponse.ValueKind == JsonValueKind.Object);

        var removeResult = PKHeXApi.RemoveItemFromPouch(handle, 1, 3);
        var removeResponse = TestHelpers.ToJsonElement(removeResult);
        Assert.True(removeResponse.ValueKind == JsonValueKind.Object);

        PKHeXApi.DisposeSave(handle);
    }
}
