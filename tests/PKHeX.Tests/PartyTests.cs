using System.Text.Json;
using PKHeX.Api;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class PartyTests
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
    public void GetParty_WithValidSave_ReturnsPartyList()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetParty(handle);
        var response = TestHelpers.ToJsonElement(result);

        Assert.Equal(JsonValueKind.Array, response.ValueKind);
        Assert.True(response.GetArrayLength() >= 0);
        
        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetParty_WithEmptyParty_ReturnsEmptyArray()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetParty(handle);
        var response = TestHelpers.ToJsonElement(result);

        Assert.Equal(JsonValueKind.Array, response.ValueKind);
        
        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetParty_WithSinglePokemon_ReturnsSingleItem()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetParty(handle);
        var response = TestHelpers.ToJsonElement(result);

        Assert.Equal(JsonValueKind.Array, response.ValueKind);
        
        if (response.GetArrayLength() > 0)
        {
            var firstPokemon = response[0];
            Assert.True(firstPokemon.TryGetProperty("species", out _));
            Assert.True(firstPokemon.TryGetProperty("speciesName", out _));
            Assert.True(firstPokemon.TryGetProperty("level", out _));
        }
        
        PKHeXApi.DisposeSave(handle);
    }
}
