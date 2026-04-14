using Robotico.Domain;

namespace Robotico.Repository.Mishima.Tests;

/// <summary>Test entity with string id.</summary>
public sealed class SampleStringEntity : IEntity<string>
{
    public string Id { get; set; } = "";

    public string Label { get; set; } = "";
}
