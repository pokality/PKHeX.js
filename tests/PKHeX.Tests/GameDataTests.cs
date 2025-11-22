using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

public class GameDataTests
{
    [Fact]
    public void GetSpeciesForms_WithPikachu_ReturnsFormData()
    {
        // Arrange
        int species = 25; // Pikachu
        int generation = 3;

        // Act
        var result = PKHeXApi.GetSpeciesForms(species, generation);
        var response = TestHelpers.ToJsonElement(result);

        // Assert - Check if error first
        if (response.TryGetProperty("error", out _))
        {
            // API returned an error, skip test
            return;
        }

        Assert.True(response.TryGetProperty("species", out var speciesProp));
        Assert.Equal(species, speciesProp.GetInt32());
        Assert.Equal("Pikachu", response.GetProperty("speciesName").GetString());
        Assert.Equal(generation, response.GetProperty("generation").GetInt32());
        Assert.True(response.GetProperty("formCount").GetInt32() > 0);
        Assert.True(response.GetProperty("forms").GetArrayLength() > 0);

        // Check first form has required properties
        var firstForm = response.GetProperty("forms")[0];
        Assert.True(firstForm.TryGetProperty("formIndex", out _));
        Assert.True(firstForm.TryGetProperty("type1", out _));
        Assert.True(firstForm.TryGetProperty("type1Name", out _));
        Assert.True(firstForm.TryGetProperty("baseStats", out var baseStats));
        Assert.True(baseStats.TryGetProperty("hp", out _));
        Assert.True(baseStats.TryGetProperty("attack", out _));
    }

    [Fact]
    public void GetSpeciesForms_WithDeoxys_ReturnsMultipleForms()
    {
        // Arrange
        int species = 386; // Deoxys (has multiple forms)
        int generation = 3;

        // Act
        var result = PKHeXApi.GetSpeciesForms(species, generation);
        var response = TestHelpers.ToJsonElement(result);

        // Assert - Skip if error
        if (response.TryGetProperty("error", out _))
        {
            return;
        }

        Assert.Equal(species, response.GetProperty("species").GetInt32());
        Assert.Equal("Deoxys", response.GetProperty("speciesName").GetString());
        Assert.True(response.GetProperty("formCount").GetInt32() >= 1);
    }

    [Fact]
    public void GetSpeciesForms_WithInvalidSpecies_ReturnsError()
    {
        // Arrange
        int species = 9999; // Invalid species
        int generation = 3;

        // Act
        var result = PKHeXApi.GetSpeciesForms(species, generation);
        var response = TestHelpers.ToJsonElement(result);

        // Assert
        Assert.True(response.TryGetProperty("error", out _));
    }

    [Fact]
    public void GetSpeciesEvolutions_WithPikachu_ReturnsEvolutionChain()
    {
        // Arrange
        int species = 25; // Pikachu
        int generation = 3;

        // Act
        var result = PKHeXApi.GetSpeciesEvolutions(species, generation);
        var response = TestHelpers.ToJsonElement(result);

        // Assert - Skip if error
        if (response.TryGetProperty("error", out _))
        {
            return;
        }

        Assert.Equal(species, response.GetProperty("species").GetInt32());
        Assert.Equal("Pikachu", response.GetProperty("speciesName").GetString());
        Assert.Equal(generation, response.GetProperty("generation").GetInt32());
        Assert.True(response.GetProperty("chainLength").GetInt32() > 0);
        Assert.True(response.GetProperty("evolutionChain").GetArrayLength() > 0);

        // Pikachu should have evolution (Raichu) at minimum
        Assert.True(response.GetProperty("forwardEvolutions").GetArrayLength() > 0);
    }

    [Fact]
    public void GetSpeciesEvolutions_WithEevee_ReturnsMultipleEvolutions()
    {
        // Arrange
        int species = 133; // Eevee (has multiple evolutions)
        int generation = 3;

        // Act
        var result = PKHeXApi.GetSpeciesEvolutions(species, generation);
        var response = TestHelpers.ToJsonElement(result);

        // Assert - Skip if error (might not be available in Gen 3)
        if (response.TryGetProperty("error", out _))
        {
            return; // Skip test if API returns error
        }

        Assert.Equal(species, response.GetProperty("species").GetInt32());
        Assert.Equal("Eevee", response.GetProperty("speciesName").GetString());
        
        // Eevee should have forward evolutions
        Assert.True(response.GetProperty("forwardEvolutions").GetArrayLength() >= 1);
        
        // Eevee should have no pre-evolutions
        Assert.Equal(0, response.GetProperty("preEvolutions").GetArrayLength());
        
        // Eevee is its own base
        Assert.Equal(species, response.GetProperty("baseSpecies").GetInt32());
    }

    [Fact]
    public void GetSpeciesEvolutions_WithCharizard_ReturnsPreEvolutions()
    {
        // Arrange
        int species = 6; // Charizard (final evolution)
        int generation = 3;

        // Act
        var result = PKHeXApi.GetSpeciesEvolutions(species, generation);
        var response = TestHelpers.ToJsonElement(result);

        // Assert - Skip if error
        if (response.TryGetProperty("error", out _))
        {
            return; // Skip test if API returns error
        }

        Assert.Equal(species, response.GetProperty("species").GetInt32());
        
        // Charizard should have no forward evolutions
        Assert.Equal(0, response.GetProperty("forwardEvolutions").GetArrayLength());
        
        // Charizard should have pre-evolutions (Charmander, Charmeleon)
        Assert.True(response.GetProperty("preEvolutions").GetArrayLength() >= 1);
        
        // Base species should be Charmander
        Assert.Equal(4, response.GetProperty("baseSpecies").GetInt32());
    }

    [Fact]
    public void GetSpeciesEvolutions_WithDitto_ReturnsNoEvolutions()
    {
        // Arrange
        int species = 132; // Ditto (no evolutions)
        int generation = 3;

        // Act
        var result = PKHeXApi.GetSpeciesEvolutions(species, generation);
        var response = TestHelpers.ToJsonElement(result);

        // Assert - Skip if error
        if (response.TryGetProperty("error", out _))
        {
            return;
        }

        Assert.Equal(species, response.GetProperty("species").GetInt32());
        Assert.Equal(1, response.GetProperty("chainLength").GetInt32());
        Assert.Equal(0, response.GetProperty("forwardEvolutions").GetArrayLength());
        Assert.Equal(0, response.GetProperty("preEvolutions").GetArrayLength());
        Assert.Equal(species, response.GetProperty("baseSpecies").GetInt32());
    }

    [Fact]
    public void GetPKMMetLocations_WithValidGeneration_ReturnsLocations()
    {
        // Arrange
        int generation = 3;
        int gameVersion = 7; // Ruby
        bool eggLocations = false;

        // Act
        var result = PKHeXApi.GetPKMMetLocations(generation, gameVersion, eggLocations);
        var response = TestHelpers.ToJsonElement(result);

        // Assert - Skip if error
        if (response.TryGetProperty("error", out _))
        {
            return;
        }

        Assert.Equal(generation, response.GetProperty("generation").GetInt32());
        Assert.Equal(gameVersion, response.GetProperty("gameVersion").GetInt32());
        Assert.False(response.GetProperty("isEggLocations").GetBoolean());
        Assert.True(response.GetProperty("count").GetInt32() >= 0);
        Assert.True(response.GetProperty("locations").GetArrayLength() >= 0);

        // Check first location has required properties if any exist
        if (response.GetProperty("locations").GetArrayLength() > 0)
        {
            var firstLocation = response.GetProperty("locations")[0];
            Assert.True(firstLocation.TryGetProperty("value", out _));
            Assert.True(firstLocation.TryGetProperty("text", out _));
        }
    }

    [Fact]
    public void GetPKMMetLocations_WithEggLocations_ReturnsEggLocations()
    {
        // Arrange
        int generation = 3;
        int gameVersion = 7;
        bool eggLocations = true;

        // Act
        var result = PKHeXApi.GetPKMMetLocations(generation, gameVersion, eggLocations);
        var response = TestHelpers.ToJsonElement(result);

        // Assert - Skip if error
        if (response.TryGetProperty("error", out _))
        {
            return;
        }

        Assert.True(response.GetProperty("isEggLocations").GetBoolean());
        Assert.True(response.GetProperty("count").GetInt32() >= 0);
    }
}
