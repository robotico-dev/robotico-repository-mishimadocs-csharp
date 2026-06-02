using System.Globalization;
using CsCheck;
using Xunit;

namespace Robotico.Repository.MishimaDocs.Tests;

/// <summary>Property checks for stable Mishima document id string formatting.</summary>
public sealed class MishimaDocumentIdFormatterPropertyTests
{
    [Fact]
    public void Format_Guid_matches_invariant_D_string()
    {
        Gen.Guid.Sample(static g =>
        {
            string formatted = MishimaDocumentIdFormatter.Format(g);
            return formatted == g.ToString("D", CultureInfo.InvariantCulture);
        });
    }

    [Fact]
    public void Format_int_round_trips_with_parse_invariant()
    {
        Gen.Int.Sample(static i =>
        {
            string formatted = MishimaDocumentIdFormatter.Format(i);
            return int.TryParse(formatted, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed) && parsed == i;
        });
    }
}
