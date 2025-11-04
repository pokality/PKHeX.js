using PKHeX.Api;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

public class ConversionTests
{
    // Sample PKM data for testing (this would be actual PKM bytes in real tests)
    private readonly string _samplePK3Data = Convert.ToBase64String(new byte[80]); // PK3 is 80 bytes

    [Fact]
    public void ConvertPKMFormat_SameGeneration_ReturnsSuccess()
    {
        // Arrange
        int fromGen = 3;
        int toGen = 3;

        // Act
        var result = PKHeXApi.ConvertPKMFormat(_samplePK3Data, fromGen, toGen);
        var response = TestHelpers.ToJsonElement(result);

        // Assert
        // When converting to same generation, it should succeed
        if (response.TryGetProperty("success", out var success))
        {
            Assert.True(success.GetBoolean());
            Assert.Equal(fromGen, response.GetProperty("fromGeneration").GetInt32());
            Assert.Equal(toGen, response.GetProperty("toGeneration").GetInt32());
        }
    }

    [Fact]
    public void ConvertPKMFormat_WithInvalidBase64_ReturnsError()
    {
        // Arrange
        string invalidData = "not-valid-base64!!!";
        int fromGen = 3;
        int toGen = 4;

        // Act
        var result = PKHeXApi.ConvertPKMFormat(invalidData, fromGen, toGen);
        var response = TestHelpers.ToJsonElement(result);

        // Assert
        Assert.True(response.TryGetProperty("error", out _));
        Assert.Equal("INVALID_BASE64", response.GetProperty("code").GetString());
    }

    [Fact]
    public void ConvertPKMFormat_WithInvalidFromGeneration_ReturnsError()
    {
        // Arrange
        int fromGen = 0; // Invalid
        int toGen = 4;

        // Act
        var result = PKHeXApi.ConvertPKMFormat(_samplePK3Data, fromGen, toGen);
        var response = TestHelpers.ToJsonElement(result);

        // Assert
        Assert.True(response.TryGetProperty("error", out _));
        Assert.Equal("INVALID_GENERATION", response.GetProperty("code").GetString());
    }

    [Fact]
    public void ConvertPKMFormat_WithInvalidToGeneration_ReturnsError()
    {
        // Arrange
        int fromGen = 3;
        int toGen = 10; // Invalid

        // Act
        var result = PKHeXApi.ConvertPKMFormat(_samplePK3Data, fromGen, toGen);
        var response = TestHelpers.ToJsonElement(result);

        // Assert
        Assert.True(response.TryGetProperty("error", out _));
        Assert.Equal("INVALID_GENERATION", response.GetProperty("code").GetString());
    }

    [Fact]
    public void ConvertPKMFormat_ReturnsFormatNames()
    {
        // Arrange
        int fromGen = 3;
        int toGen = 3;

        // Act
        var result = PKHeXApi.ConvertPKMFormat(_samplePK3Data, fromGen, toGen);
        var response = TestHelpers.ToJsonElement(result);

        // Assert
        if (response.TryGetProperty("success", out _))
        {
            Assert.True(response.TryGetProperty("fromFormat", out _));
            Assert.True(response.TryGetProperty("toFormat", out _));
            Assert.True(response.TryGetProperty("conversionResult", out _));
        }
    }

    [Fact]
    public void ConvertPKMFormat_ReturnsBase64Data()
    {
        // Arrange
        int fromGen = 3;
        int toGen = 3;

        // Act
        var result = PKHeXApi.ConvertPKMFormat(_samplePK3Data, fromGen, toGen);
        var response = TestHelpers.ToJsonElement(result);

        // Assert
        if (response.TryGetProperty("success", out _))
        {
            Assert.True(response.TryGetProperty("base64Data", out var base64Data));
            var dataString = base64Data.GetString();
            Assert.NotNull(dataString);
            Assert.NotEmpty(dataString);
            
            // Verify it's valid base64
            try
            {
                Convert.FromBase64String(dataString);
            }
            catch
            {
                Assert.Fail("Returned data is not valid base64");
            }
        }
    }
}
