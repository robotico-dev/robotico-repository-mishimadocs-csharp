using System.Text.Json;

namespace Robotico.Repository.Mishima;

/// <summary>
/// Shared JSON options for entity serialization to <see cref="System.Text.Json.JsonElement"/>.
/// </summary>
internal static class MishimaRepositoryJsonOptions
{
    internal static readonly JsonSerializerOptions Instance = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        PropertyNameCaseInsensitive = true,
    };
}
