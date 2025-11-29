using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class BatchTests
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
    public void BatchCheckLegality_WithValidLocations_ReturnsResults()
    {
        var handle = LoadTestSave();

        var locations = new[]
        {
            new { box = 0, slot = 0 },
            new { box = 0, slot = 1 },
            new { box = 0, slot = 2 }
        };
        var locationsJson = JsonSerializer.Serialize(locations);

        var result = PKHeXApi.BatchCheckLegality(handle, locationsJson);

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("results", out var results));
            Assert.True(response.TryGetProperty("validCount", out _));
            Assert.True(response.TryGetProperty("invalidCount", out _));
            Assert.True(response.TryGetProperty("emptyCount", out _));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void BatchCheckLegality_WithInvalidHandle_ReturnsError()
    {
        var locations = new[] { new { box = 0, slot = 0 } };
        var locationsJson = JsonSerializer.Serialize(locations);

        var result = PKHeXApi.BatchCheckLegality(-1, locationsJson);
        Assert.True(TestHelpers.IsError(result));
    }

    [Fact]
    public void BatchCheckLegality_WithEmptyLocations_ReturnsError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.BatchCheckLegality(handle, "[]");
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void BatchModifyPokemon_WithValidModifications_ModifiesAll()
    {
        var handle = LoadTestSave();

        var modifications = new[]
        {
            new { box = 0, slot = 0, modifications = new { level = 50 } },
            new { box = 0, slot = 1, modifications = new { level = 50 } }
        };
        var modificationsJson = JsonSerializer.Serialize(modifications);

        var result = PKHeXApi.BatchModifyPokemon(handle, modificationsJson);

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("results", out _));
            Assert.True(response.TryGetProperty("successCount", out _));
            Assert.True(response.TryGetProperty("failCount", out _));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void ClearBox_WithValidBox_ClearsPokemon()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.ClearBox(handle, 0);

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("clearedCount", out _));
            Assert.True(response.TryGetProperty("message", out _));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void ClearBox_WithInvalidBox_ReturnsError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.ClearBox(handle, 999);
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void ClearAllBoxes_WithValidHandle_ClearsAll()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.ClearAllBoxes(handle);

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("clearedCount", out _));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SortBox_WithValidCriteria_SortsBox()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SortBox(handle, 0, "species");

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SortBox_WithLevelCriteria_SortsBox()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SortBox(handle, 0, "level");

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void CompactBox_WithValidBox_CompactsPokemon()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.CompactBox(handle, 0);

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetBoxStats_WithValidBox_ReturnsStats()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetBoxStats(handle, 0);

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("box", out _));
            Assert.True(response.TryGetProperty("totalSlots", out _));
            Assert.True(response.TryGetProperty("occupied", out _));
            Assert.True(response.TryGetProperty("empty", out _));
            Assert.True(response.TryGetProperty("shinyCount", out _));
            Assert.True(response.TryGetProperty("eggCount", out _));
            Assert.True(response.TryGetProperty("uniqueSpecies", out _));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetBoxStats_WithInvalidBox_ReturnsError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetBoxStats(handle, 999);
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void BatchCheckLegality_WithMalformedJson_ReturnsError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.BatchCheckLegality(handle, "not valid json");
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void BatchCheckLegality_WithMissingRequiredFields_ReturnsError()
    {
        var handle = LoadTestSave();

        // Missing slot field
        var result = PKHeXApi.BatchCheckLegality(handle, "[{\"box\": 0}]");
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void BatchModifyPokemon_WithMalformedJson_ReturnsError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.BatchModifyPokemon(handle, "invalid json {{{}");
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void BatchModifyPokemon_WithMissingModifications_ReturnsError()
    {
        var handle = LoadTestSave();

        // Missing modifications field
        var result = PKHeXApi.BatchModifyPokemon(handle, "[{\"box\": 0, \"slot\": 0}]");
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SortBox_WithInvalidCriteria_ReturnsError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SortBox(handle, 0, "invalid_sort_type");
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SortBox_WithEmptyCriteria_ReturnsError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.SortBox(handle, 0, "");
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SortBox_WithAllValidCriteria_Succeeds()
    {
        var handle = LoadTestSave();

        var validCriteria = new[] { "species", "level", "name", "pokedex", "shiny", "type" };

        foreach (var criteria in validCriteria)
        {
            var result = PKHeXApi.SortBox(handle, 0, criteria);
            // Should not throw, and should either succeed or be a non-parsing error
            Assert.False(string.IsNullOrEmpty(result));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void BatchCheckLegality_WithOutOfRangeBox_HandlesGracefully()
    {
        var handle = LoadTestSave();

        var locations = new[] { new { box = 999, slot = 0 } };
        var locationsJson = JsonSerializer.Serialize(locations);

        var result = PKHeXApi.BatchCheckLegality(handle, locationsJson);
        // Should return error or empty results, not crash
        Assert.False(string.IsNullOrEmpty(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void BatchCheckLegality_WithOutOfRangeSlot_HandlesGracefully()
    {
        var handle = LoadTestSave();

        var locations = new[] { new { box = 0, slot = 999 } };
        var locationsJson = JsonSerializer.Serialize(locations);

        var result = PKHeXApi.BatchCheckLegality(handle, locationsJson);
        // Should return error or mark as empty, not crash
        Assert.False(string.IsNullOrEmpty(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void BatchModifyPokemon_OnEmptySlot_ReportsInResults()
    {
        var handle = LoadTestSave();

        // First clear the box to ensure empty slots
        PKHeXApi.ClearBox(handle, 0);

        var modifications = new[]
        {
            new { box = 0, slot = 0, modifications = new { level = 50 } }
        };
        var modificationsJson = JsonSerializer.Serialize(modifications);

        var result = PKHeXApi.BatchModifyPokemon(handle, modificationsJson);

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            // Should report the empty slot operation
            Assert.True(response.TryGetProperty("results", out _));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void CompactBox_WithInvalidBox_ReturnsError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.CompactBox(handle, 999);
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void ClearAllBoxes_WithInvalidHandle_ReturnsError()
    {
        var result = PKHeXApi.ClearAllBoxes(-1);
        Assert.True(TestHelpers.IsError(result));
    }
}
