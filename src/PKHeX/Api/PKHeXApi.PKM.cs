using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Standalone PKM Operations (without save handle)
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetPKMShiny(string base64PkmData, int generation, int shinyType)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            if (shinyType < 0 || shinyType > 5)
                throw new ValidationException($"Shiny type {shinyType} is out of range (0-5)", INVALID_SHINY_TYPE);

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var type = (Shiny)shinyType;

            if (type == Shiny.Never)
            {
                pk.SetUnshiny();
            }
            else
            {
                CommonEdits.SetShiny(pk, type);
            }

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMPIDInfo(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var isShiny = pk.IsShiny;
            var shinyType = pk.ShinyXor == 0 ? "Square" : pk.IsShiny ? "Star" : "None";
            var nature = pk.Nature;
            var gender = pk.Gender;
            var genderName = gender == 0 ? "Male" : gender == 1 ? "Female" : "Genderless";

            return new PIDInfo(
                true,
                pk.PID,
                isShiny,
                shinyType,
                (int)nature,
                nature.ToString(),
                gender,
                genderName
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetPKMPID(string base64PkmData, int generation, int pid)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            if (pid < 0)
                throw new ValidationException("PID must be non-negative", INVALID_PID);

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            pk.PID = (uint)pid;
            pk.RefreshAbility(pk.AbilityNumber >> 1);

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMData(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            return CreatePokemonDetailObject(pk);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ModifyPKMData(string base64PkmData, int generation, string modificationsJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var mods = JsonSerializer.Deserialize<PokemonModifications>(modificationsJson, JsonOptions);
            if (mods == null)
                throw new ValidationException("Invalid modifications JSON", INVALID_JSON);

            if (mods.Species.HasValue)
                pk.Species = (ushort)mods.Species.Value;
            if (mods.Nickname != null)
                pk.Nickname = mods.Nickname;
            if (mods.Level.HasValue)
                pk.CurrentLevel = (byte)mods.Level.Value;
            if (mods.Nature.HasValue)
                pk.Nature = (Nature)mods.Nature.Value;
            if (mods.Ability.HasValue)
                pk.Ability = mods.Ability.Value;
            if (mods.HeldItem.HasValue)
                pk.HeldItem = mods.HeldItem.Value;
            if (mods.Gender.HasValue)
                pk.Gender = (byte)mods.Gender.Value;
            if (mods.IsShiny.HasValue && mods.IsShiny.Value && !pk.IsShiny)
                CommonEdits.SetShiny(pk, Shiny.AlwaysStar);
            if (mods.OT_Name != null)
                pk.OriginalTrainerName = mods.OT_Name;
            if (mods.Ball.HasValue)
                pk.Ball = (byte)mods.Ball.Value;

            if (mods.Moves != null && mods.Moves.Length > 0)
            {
                if (mods.Moves.Length > 0) pk.Move1 = (ushort)mods.Moves[0];
                if (mods.Moves.Length > 1) pk.Move2 = (ushort)mods.Moves[1];
                if (mods.Moves.Length > 2) pk.Move3 = (ushort)mods.Moves[2];
                if (mods.Moves.Length > 3) pk.Move4 = (ushort)mods.Moves[3];
            }

            if (mods.IVs != null && mods.IVs.Length == 6)
            {
                pk.IV_HP = mods.IVs[0];
                pk.IV_ATK = mods.IVs[1];
                pk.IV_DEF = mods.IVs[2];
                pk.IV_SPE = mods.IVs[3];
                pk.IV_SPA = mods.IVs[4];
                pk.IV_SPD = mods.IVs[5];
            }

            if (mods.EVs != null && mods.EVs.Length == 6)
            {
                pk.EV_HP = mods.EVs[0];
                pk.EV_ATK = mods.EVs[1];
                pk.EV_DEF = mods.EVs[2];
                pk.EV_SPE = mods.EVs[3];
                pk.EV_SPA = mods.EVs[4];
                pk.EV_SPD = mods.EVs[5];
            }

            pk.RefreshChecksum();

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string CheckPKMLegality(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var analysis = new LegalityAnalysis(pk);
            var errorList = new List<string>();
            var localizer = LegalityLocalizationContext.Create(analysis);

            foreach (var r in analysis.Results)
            {
                if (!r.Valid)
                    errorList.Add(localizer.Humanize(r));
            }

            return new
            {
                valid = analysis.Valid,
                errors = errorList.ToArray(),
                parsed = analysis.Report()
            };
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string LegalizePKMData(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            pk.SetMoveset();
            pk.Heal();
            pk.RefreshChecksum();

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ExportPKMShowdown(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var showdownText = ShowdownParsing.GetShowdownText(pk);
            return new ShowdownResponse(true, showdownText);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string CalculatePKMStats(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            return new
            {
                hp = pk.Stat_HPMax,
                attack = pk.Stat_ATK,
                defense = pk.Stat_DEF,
                spAttack = pk.Stat_SPA,
                spDefense = pk.Stat_SPD,
                speed = pk.Stat_SPE
            };
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMRibbons(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var ribbonInfo = RibbonInfo.GetRibbonInfo(pk);
            var ribbonList = ribbonInfo.Select(r => new
            {
                name = r.Name,
                hasRibbon = r.HasRibbon,
                ribbonCount = r.RibbonCount,
                type = r.Type.ToString()
            }).ToList();

            return ribbonList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetPKMRibbon(string base64PkmData, int generation, string ribbonName, bool value)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            if (string.IsNullOrWhiteSpace(ribbonName))
                throw new ValidationException("Ribbon name cannot be empty", "EMPTY_RIBBON_NAME");

            var property = pk.GetType().GetProperty(ribbonName);
            if (property == null)
                throw new ValidationException($"Ribbon '{ribbonName}' not found on this Pokemon", "INVALID_RIBBON");

            if (property.PropertyType == typeof(bool))
            {
                property.SetValue(pk, value);
            }
            else if (property.PropertyType == typeof(byte))
            {
                property.SetValue(pk, value ? (byte)1 : (byte)0);
            }
            else
            {
                throw new ValidationException($"Ribbon '{ribbonName}' has unsupported type", "INVALID_RIBBON_TYPE");
            }

            pk.RefreshChecksum();

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string RerollPKMEncryptionConstant(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var rnd = new Random();
            pk.EncryptionConstant = (uint)rnd.Next();
            pk.RefreshChecksum();

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMHiddenPower(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var hpType = pk.HPType;
            var hpPower = pk.HPPower;

            return new
            {
                type = (int)hpType,
                typeName = GameInfo.Strings.Types[(int)hpType],
                power = hpPower
            };
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMCharacteristic(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var characteristic = pk.Characteristic;
            var characteristics = GameInfo.Strings.characteristics;
            var characteristicText = characteristic < characteristics.Length ? characteristics[characteristic] : "";

            return new
            {
                index = characteristic,
                text = characteristicText
            };
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMMetLocations(int generation, int gameVersion, bool eggLocations)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var context = (EntityContext)generation;
            var version = (GameVersion)gameVersion;
            var locations = GameInfo.GetLocationList(version, context, eggLocations);

            var locationList = new List<object>();
            foreach (var loc in locations)
            {
                locationList.Add(new
                {
                    value = loc.Value,
                    text = loc.Text
                });
            }

            return new
            {
                generation,
                gameVersion,
                isEggLocations = eggLocations,
                locations = locationList,
                count = locationList.Count
            };
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.Any>]
    public static object ConvertPKMFormat(string base64PkmData, int fromGeneration, int toGeneration)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            ApiHelpers.ValidateNonNegative(fromGeneration, nameof(fromGeneration), "INVALID_GENERATION");
            ApiHelpers.ValidateNonNegative(toGeneration, nameof(toGeneration), "INVALID_GENERATION");

            if (fromGeneration < 1 || fromGeneration > 9)
                throw new ValidationException($"From generation {fromGeneration} is out of range (1-9)", "INVALID_GENERATION");

            if (toGeneration < 1 || toGeneration > 9)
                throw new ValidationException($"To generation {toGeneration} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)fromGeneration);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            Type? targetType = toGeneration switch
            {
                1 => typeof(PK1),
                2 => typeof(PK2),
                3 => typeof(PK3),
                4 => typeof(PK4),
                5 => typeof(PK5),
                6 => typeof(PK6),
                7 => typeof(PK7),
                8 => typeof(PK8),
                9 => typeof(PK9),
                _ => null
            };

            if (targetType == null)
                throw new ValidationException($"Invalid target generation {toGeneration}", "INVALID_GENERATION");

            var converted = EntityConverter.ConvertToType(pk, targetType, out var result);

            if (converted == null)
            {
                var errorMessage = result switch
                {
                    EntityConverterResult.NoTransferRoute => "No valid conversion route exists between these generations",
                    EntityConverterResult.IncompatibleForm => "Pokemon form is incompatible with target generation",
                    EntityConverterResult.IncompatibleSpecies => "Pokemon species is incompatible with target generation",
                    EntityConverterResult.IncompatibleLanguageGB => "Language incompatibility for GB transfer",
                    _ => $"Conversion failed: {result}"
                };
                throw new ValidationException(errorMessage, "CONVERSION_FAILED");
            }

            converted.RefreshChecksum();
            var convertedData = converted.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(convertedData);

            return new
            {
                success = true,
                base64Data = base64Result,
                fromGeneration,
                toGeneration,
                fromFormat = pk.GetType().Name,
                toFormat = converted.GetType().Name,
                conversionResult = result.ToString()
            };
        });
    }
}
