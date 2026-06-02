using System.Globalization;
using Xunit;

namespace Robotico.Repository.MishimaDocs.Tests;

/// <summary>Branch coverage for numeric and string id formatting.</summary>
public sealed class MishimaDocumentIdFormatterTests
{
    [Fact]
    public void Format_string_is_passthrough()
    {
        Assert.Equal("abc", MishimaDocumentIdFormatter.Format("abc"));
    }

    [Fact]
    public void Format_int_uses_invariant()
    {
        Assert.Equal("42", MishimaDocumentIdFormatter.Format(42));
    }

    [Fact]
    public void Format_long_uses_invariant()
    {
        Assert.Equal(long.MaxValue.ToString(CultureInfo.InvariantCulture), MishimaDocumentIdFormatter.Format(long.MaxValue));
    }

    [Fact]
    public void Format_uint_uses_invariant()
    {
        Assert.Equal(uint.MaxValue.ToString(CultureInfo.InvariantCulture), MishimaDocumentIdFormatter.Format(uint.MaxValue));
    }

    [Fact]
    public void Format_ulong_uses_invariant()
    {
        Assert.Equal(ulong.MaxValue.ToString(CultureInfo.InvariantCulture), MishimaDocumentIdFormatter.Format(ulong.MaxValue));
    }

    [Fact]
    public void Format_short_uses_invariant()
    {
        Assert.Equal(short.MinValue.ToString(CultureInfo.InvariantCulture), MishimaDocumentIdFormatter.Format(short.MinValue));
    }

    [Fact]
    public void Format_ushort_uses_invariant()
    {
        Assert.Equal(ushort.MaxValue.ToString(CultureInfo.InvariantCulture), MishimaDocumentIdFormatter.Format(ushort.MaxValue));
    }

    [Fact]
    public void Format_byte_uses_invariant()
    {
        Assert.Equal(byte.MaxValue.ToString(CultureInfo.InvariantCulture), MishimaDocumentIdFormatter.Format(byte.MaxValue));
    }

    [Fact]
    public void Format_sbyte_uses_invariant()
    {
        Assert.Equal(sbyte.MinValue.ToString(CultureInfo.InvariantCulture), MishimaDocumentIdFormatter.Format(sbyte.MinValue));
    }

    [Fact]
    public void Format_char_uses_default_branch()
    {
        Assert.Equal("z", MishimaDocumentIdFormatter.Format('z'));
    }

    [Fact]
    public void Format_throws_when_convert_to_string_yields_null()
    {
        Assert.Throws<InvalidOperationException>(() => MishimaDocumentIdFormatter.Format(new DocumentIdWithNullToString()));
    }
}
