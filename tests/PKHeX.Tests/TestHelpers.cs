using System.Text.Json;

namespace PKHeX.Tests;

public static class TestHelpers
{
    /// <summary>
    /// Convert API response (JSON string or object) to JsonElement for testing
    /// </summary>
    public static JsonElement ToJsonElement(object response)
    {
        if (response is string jsonString)
        {
            return JsonSerializer.Deserialize<JsonElement>(jsonString);
        }
        
        var json = JsonSerializer.Serialize(response);
        return JsonSerializer.Deserialize<JsonElement>(json);
    }

    /// <summary>
    /// Check if response is an error
    /// </summary>
    public static bool IsError(object response)
    {
        var element = ToJsonElement(response);
        if (element.ValueKind != JsonValueKind.Object)
            return false;
        return element.TryGetProperty("error", out _);
    }

    /// <summary>
    /// Check if response is successful
    /// </summary>
    public static bool IsSuccess(object response)
    {
        var element = ToJsonElement(response);
        return element.TryGetProperty("success", out var success) && success.GetBoolean();
    }

    /// <summary>
    /// Get property from response
    /// </summary>
    public static bool TryGetProperty(object response, string propertyName, out JsonElement value)
    {
        var element = ToJsonElement(response);
        return element.TryGetProperty(propertyName, out value);
    }
}
