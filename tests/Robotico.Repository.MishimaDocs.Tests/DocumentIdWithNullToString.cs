namespace Robotico.Repository.MishimaDocs.Tests;

/// <summary>Used to hit the formatter fallback when <c>Convert.ToString</c> yields null.</summary>
public sealed class DocumentIdWithNullToString
{
    public override string? ToString() => null;
}
