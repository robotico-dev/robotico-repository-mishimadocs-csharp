using System.Globalization;

namespace Robotico.Repository.Mishima;

/// <summary>
/// Maps entity identifier values to Mishima document id strings (collection primary keys).
/// </summary>
internal static class MishimaDocumentIdFormatter
{
    /// <summary>
    /// Formats an entity id for use as a Mishima document id.
    /// </summary>
    /// <typeparam name="TId">Identifier type.</typeparam>
    /// <param name="id">Non-null id.</param>
    /// <returns>Stable string key for the document.</returns>
    internal static string Format<TId>(TId id)
        where TId : notnull
    {
        return id switch
        {
            string s => s,
            Guid g => g.ToString("D", CultureInfo.InvariantCulture),
            int i => i.ToString(CultureInfo.InvariantCulture),
            long l => l.ToString(CultureInfo.InvariantCulture),
            uint ui => ui.ToString(CultureInfo.InvariantCulture),
            ulong ul => ul.ToString(CultureInfo.InvariantCulture),
            short sh => sh.ToString(CultureInfo.InvariantCulture),
            ushort ush => ush.ToString(CultureInfo.InvariantCulture),
            byte b => b.ToString(CultureInfo.InvariantCulture),
            sbyte sb => sb.ToString(CultureInfo.InvariantCulture),
            _ => Convert.ToString(id, CultureInfo.InvariantCulture)
                ?? throw new InvalidOperationException($"Cannot derive Mishima document id for type {typeof(TId).FullName}.")
        };
    }
}
