using System.Text.Json;
using PKHeX.Helpers;
using PKHeX.Models;
using Xunit;

namespace PKHeX.Tests;

public class JsonSerializationTests
{
    [Fact]
    public void SerializeSuccess_WithSaveFileHandle_ReturnsValidJson()
    {
        var handle = new SaveFileHandle(123);
        
        var json = ApiHelpers.SerializeSuccess(handle);
        
        Assert.NotNull(json);
        Assert.Contains("\"success\"", json);
        Assert.Contains("\"handle\"", json);
        Assert.Contains("123", json);
    }

    [Fact]
    public void SerializeSuccess_WithListOfPokemonSummary_ReturnsValidJson()
    {
        var list = new List<PokemonSummary>
        {
            new PokemonSummary(0, 0, 25, "Pikachu", 50, false, false),
            new PokemonSummary(0, 1, 6, "Charizard", 100, false, true)
        };
        
        var json = ApiHelpers.SerializeSuccess(list);
        
        Assert.NotNull(json);
        Assert.Contains("\"success\"", json);
        Assert.Contains("\"species\"", json);
        Assert.Contains("\"speciesName\"", json);
        Assert.Contains("Pikachu", json);
        Assert.Contains("Charizard", json);
    }

    [Fact]
    public void SerializeSuccess_WithSaveFileInfo_ReturnsValidJson()
    {
        var info = new SaveFileInfo("Gen 4", "Diamond", "ASH", 12345, 54321, 18);
        
        var json = ApiHelpers.SerializeSuccess(info);
        
        Assert.NotNull(json);
        Assert.Contains("\"success\"", json);
        Assert.Contains("\"generation\"", json);
        Assert.Contains("\"gameVersion\"", json);
        Assert.Contains("Diamond", json);
    }

    [Fact]
    public void SerializeError_ReturnsValidErrorJson()
    {
        var error = new ErrorResponse("Test error", "TEST_CODE");
        
        var json = JsonSerializer.Serialize(error);
        
        Assert.NotNull(json);
        Assert.Contains("\"error\"", json);
        Assert.Contains("\"code\"", json);
        Assert.Contains("Test error", json);
        Assert.Contains("TEST_CODE", json);
    }
}
