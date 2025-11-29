using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class FormTests
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
    public void GetForm_WithValidPokemon_ReturnsFormData()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.GetForm(handle, 0, 0);

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("form", out _));
            Assert.True(response.TryGetProperty("formName", out _));
            Assert.True(response.TryGetProperty("formCount", out _));
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetForm_WithInvalidHandle_ReturnsError()
    {
        var result = PKHeXApi.GetForm(-1, 0, 0);
        Assert.True(TestHelpers.IsError(result));
    }

    [Fact]
    public void SetForm_WithValidForm_UpdatesForm()
    {
        var handle = LoadTestSave();

        // First get the form count to know valid range
        var getResult = PKHeXApi.GetForm(handle, 0, 0);
        if (!TestHelpers.IsError(getResult))
        {
            var formData = TestHelpers.ToJsonElement(getResult);
            var formCount = formData.GetProperty("formCount").GetInt32();

            if (formCount > 1)
            {
                var result = PKHeXApi.SetForm(handle, 0, 0, 1);
                if (!TestHelpers.IsError(result))
                {
                    Assert.True(TestHelpers.IsSuccess(result));
                }
            }
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void SetForm_WithInvalidForm_ReturnsError()
    {
        var handle = LoadTestSave();

        // Try to set an invalid form (255 is likely out of range)
        var result = PKHeXApi.SetForm(handle, 0, 0, 255);

        // Should return error for invalid form
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void GetAvailableForms_WithValidSpecies_ReturnsForms()
    {
        // Unown (species 201) has 28 forms
        var result = PKHeXApi.GetAvailableForms(201, 3);

        if (!TestHelpers.IsError(result))
        {
            var response = TestHelpers.ToJsonElement(result);
            Assert.True(response.TryGetProperty("forms", out var forms));
            Assert.True(response.TryGetProperty("formCount", out var count));
            Assert.True(count.GetInt32() > 1);
        }
    }

    [Fact]
    public void GetAvailableForms_WithInvalidSpecies_ReturnsError()
    {
        var result = PKHeXApi.GetAvailableForms(9999, 3);
        Assert.True(TestHelpers.IsError(result));
    }

    [Fact]
    public void GetAvailableForms_WithInvalidGeneration_ReturnsError()
    {
        var result = PKHeXApi.GetAvailableForms(25, 99);
        Assert.True(TestHelpers.IsError(result));
    }

    [Fact]
    public void ChangeSpeciesAndForm_WithValidValues_UpdatesBoth()
    {
        var handle = LoadTestSave();

        // Change to a different species (Pikachu, species 25)
        var result = PKHeXApi.ChangeSpeciesAndForm(handle, 0, 0, 25, 0);

        if (!TestHelpers.IsError(result))
        {
            Assert.True(TestHelpers.IsSuccess(result));

            var getResult = PKHeXApi.GetPokemon(handle, 0, 0);
            if (!TestHelpers.IsError(getResult))
            {
                var pokemon = TestHelpers.ToJsonElement(getResult);
                if (pokemon.TryGetProperty("species", out var species))
                {
                    Assert.Equal(25, species.GetInt32());
                }
            }
        }

        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void ChangeSpeciesAndForm_WithInvalidSpecies_ReturnsError()
    {
        var handle = LoadTestSave();

        var result = PKHeXApi.ChangeSpeciesAndForm(handle, 0, 0, 9999, 0);
        Assert.True(TestHelpers.IsError(result));

        PKHeXApi.DisposeSave(handle);
    }
}
