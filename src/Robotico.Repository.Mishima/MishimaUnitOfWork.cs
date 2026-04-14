using Robotico.Repository;

namespace Robotico.Repository.Mishima;

/// <summary>
/// MishimaDocs <see cref="IUnitOfWork"/> that performs no extra persistence: each <see cref="MishimaRepository{TEntity, TId}"/> operation already commits through MishimaDocs.
/// </summary>
/// <remarks>
/// <para>Use this type when application code expects an <see cref="IUnitOfWork"/> alongside repositories (same pattern as an in-memory no-op unit of work).</para>
/// <para>For atomic multi-collection writes, use MishimaDocs <c>IMishimaWriteBatch</c> from <c>IMishimaDatabase.BeginWriteBatch</c> in application code; this package does not yet coordinate repositories with a shared batch.</para>
/// </remarks>
public sealed class MishimaUnitOfWork : IUnitOfWork
{
    /// <inheritdoc />
    public Task<Robotico.Result.Result> CommitAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Robotico.Result.Result.Success());
    }
}
