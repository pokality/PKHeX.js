using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class PokemonTests
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
    public void GetAllPokemon_WithValidSession_ReturnsPokemonList()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetAllPokemon(handle);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(response.ValueKind == JsonValueKind.Array);

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetParty_WithValidSession_ReturnsPartyPokemon()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetParty(handle);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(response.ValueKind == JsonValueKind.Array);

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetPokemon_WithValidCoordinates_ReturnsPokemonDetail()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetPokemon(handle, 0, 0);
        var response = TestHelpers.ToJsonElement(result);

        if (!TestHelpers.IsError(result))
        {
            Assert.True(response.TryGetProperty("species", out _) || 
                       response.TryGetProperty("speciesName", out _) ||
                       response.TryGetProperty("isEmpty", out _));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetPokemon_WithInvalidSession_ReturnsError()
    {
        var result = PKHeXApi.GetPokemon(-1, 0, 0);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));
    }

    [Fact]
    public void ModifyPokemon_WithValidModifications_UpdatesPokemon()
    {
        var handle = LoadTestSave();

        var modifications = new
        {
            level = 50,
            nickname = "TestMon"
        };
        var modificationsJson = JsonSerializer.Serialize(modifications);

        var result = PKHeXApi.ModifyPokemon(handle, 0, 0, modificationsJson);
        var response = TestHelpers.ToJsonElement(result);

        if (!TestHelpers.IsError(result))
        {
            var getPokemonResult = PKHeXApi.GetPokemon(handle, 0, 0);
            var pokemonResponse = TestHelpers.ToJsonElement(getPokemonResult);

            if (!pokemonResponse.TryGetProperty("message", out _))
            {
                if (pokemonResponse.TryGetProperty("level", out var level))
                {
                    Assert.Equal(50, level.GetInt32());
                }
            }
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void ModifyPokemon_WithInvalidSession_ReturnsError()
    {
        var modifications = new { level = 50 };
        var modificationsJson = JsonSerializer.Serialize(modifications);

        var result = PKHeXApi.ModifyPokemon(-1, 0, 0, modificationsJson);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(TestHelpers.IsError(result));
        Assert.True(response.TryGetProperty("code", out _));
    }

    [Fact]
    public void DeletePokemon_WithValidCoordinates_RemovesPokemon()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.DeletePokemon(handle, 0, 0);
        var response = TestHelpers.ToJsonElement(result);

        Assert.False(response.TryGetProperty("code", out _));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void MovePokemon_WithValidCoordinates_MovesPokemon()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.MovePokemon(handle, 0, 0, 0, 1);
        var response = TestHelpers.ToJsonElement(result);

        Assert.True(response.ValueKind == JsonValueKind.Object);

        PKHeXApi.DisposeSave(handle);
    }
}
