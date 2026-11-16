using System.Text.Json;
using System.Text.Json.Serialization;

namespace MPServerProvider.Core.Json;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Json.Json</c> (decompile token 0x0200003b).
/// Centralizes the app's <see cref="JsonSerializerOptions"/> and file read/write helpers.
/// The decompiled static constructor configures:
/// <list type="bullet">
/// <item><see cref="JsonSerializerOptions.PropertyNamingPolicy"/> = <see cref="JsonNamingPolicy.SnakeCaseLower"/></item>
/// <item><see cref="JsonSerializerOptions.PropertyNameCaseInsensitive"/> = <c>true</c></item>
/// <item><see cref="JsonSerializerOptions.WriteIndented"/> = <c>true</c></item>
/// <item><see cref="JsonSerializerOptions.DefaultIgnoreCondition"/> = <see cref="JsonIgnoreCondition.WhenWritingNull"/></item>
/// <item><see cref="JsonSerializerOptions.ReadCommentHandling"/> = <see cref="JsonCommentHandling.Skip"/></item>
/// <item><see cref="JsonSerializerOptions.AllowTrailingCommas"/> = <c>true</c></item>
/// <item>A <see cref="JsonStringEnumConverter"/> registered so enums (e.g. <see cref="Models.ServerState"/>) serialize as snake_case strings.</item>
/// </list>
/// This single global policy is why none of the types in <c>MPServerProvider.Core.Models</c>
/// need per-property <c>[JsonPropertyName]</c> attributes: the naming policy alone produces the
/// exact on-disk field names confirmed in <c>data/providers/json/*.json</c> and
/// <c>data/settings.json</c>.
/// </summary>
public static class Json
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);

    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

    public static T? ReadFile<T>(string path)
    {
        if (!File.Exists(path))
        {
            return default;
        }

        return Deserialize<T>(File.ReadAllText(path));
    }

    /// <summary>
    /// Serializes <paramref name="value"/> and writes it with <see cref="WriteAllTextAtomic"/>, so
    /// a crash/power loss/full disk mid-write can never leave a truncated JSON file behind.
    /// </summary>
    public static void WriteFile<T>(string path, T value) => WriteAllTextAtomic(path, Serialize(value));

    /// <summary>
    /// Atomic replacement of <paramref name="path"/>'s contents: writes to a sibling temp file,
    /// flushes it all the way to disk, then renames it over the target (a same-volume rename is
    /// atomic on NTFS). Readers therefore only ever see either the old or the new complete file.
    /// The temp file is removed if anything fails before the rename.
    /// </summary>
    public static void WriteAllTextAtomic(string path, string contents)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var tmp = $"{path}.{Guid.NewGuid():N}.tmp";
        try
        {
            using (var stream = new FileStream(tmp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            {
                writer.Write(contents);
                writer.Flush();
                stream.Flush(flushToDisk: true);
            }

            File.Move(tmp, path, overwrite: true);
        }
        catch
        {
            try
            {
                if (File.Exists(tmp))
                {
                    File.Delete(tmp);
                }
            }
            catch
            {
                // Best-effort cleanup; the original error below is what matters.
            }

            throw;
        }
    }
}
