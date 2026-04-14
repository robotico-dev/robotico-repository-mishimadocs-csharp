using Robotico.Domain;

namespace Robotico.Repository.Mishima.Tests;

/// <summary>Test entity with <see cref="Guid"/> id.</summary>
public sealed class SampleEntity : IEntity<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";
}
