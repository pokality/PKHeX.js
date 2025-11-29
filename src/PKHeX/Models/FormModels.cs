using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record FormData(
    [property: JsonPropertyName("form")] int Form,
    [property: JsonPropertyName("formName")] string FormName,
    [property: JsonPropertyName("formCount")] byte FormCount,
    [property: JsonPropertyName("formArgument")] uint? FormArgument,
    [property: JsonPropertyName("formArgumentRemain")] uint? FormArgumentRemain,
    [property: JsonPropertyName("formArgumentElapsed")] uint? FormArgumentElapsed,
    [property: JsonPropertyName("formArgumentMaximum")] uint? FormArgumentMaximum
);

public record FormInfo(
    [property: JsonPropertyName("formIndex")] int FormIndex,
    [property: JsonPropertyName("formName")] string FormName
);

public record AvailableFormsData(
    [property: JsonPropertyName("species")] int Species,
    [property: JsonPropertyName("speciesName")] string SpeciesName,
    [property: JsonPropertyName("generation")] int Generation,
    [property: JsonPropertyName("forms")] List<FormInfo> Forms,
    [property: JsonPropertyName("formCount")] int FormCount
);
