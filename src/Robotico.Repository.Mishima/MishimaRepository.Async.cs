using System.Text.Json;
using MishimaDocs;

#pragma warning disable MA0042 // Async methods delegate to sync paths when Mishima async collection API is not used.

namespace Robotico.Repository.Mishima;

public sealed partial class MishimaRepository<TEntity, TId>
{
    /// <inheritdoc />
    public Task<Robotico.Result.Result<TEntity>> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(GetById(id));
    }

    /// <inheritdoc />
    public async Task<Robotico.Result.Result> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (TryGetAsyncCollection(out IMishimaAsyncCollection? asyncCollection) && asyncCollection is not null)
        {
            string documentId = MishimaDocumentIdFormatter.Format(entity.Id);
            try
            {
                JsonElement payload = JsonSerializer.SerializeToElement(entity, MishimaRepositoryJsonOptions.Instance);
                await asyncCollection.InsertAsync(documentId, payload, cancellationToken).ConfigureAwait(false);
                return Robotico.Result.Result.Success();
            }
            catch (MishimaPersistenceException ex)
            {
                return MishimaRepositoryPersistenceRouter.MapAfterAdd<TEntity, TId>(ex, entity);
            }
        }

        return Add(entity);
    }

    /// <inheritdoc />
    public async Task<Robotico.Result.Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (TryGetAsyncCollection(out IMishimaAsyncCollection? asyncCollection) && asyncCollection is not null)
        {
            string documentId = MishimaDocumentIdFormatter.Format(entity.Id);
            try
            {
                JsonElement payload = JsonSerializer.SerializeToElement(entity, MishimaRepositoryJsonOptions.Instance);
                await asyncCollection.ReplaceAsync(documentId, payload, cancellationToken).ConfigureAwait(false);
                return Robotico.Result.Result.Success();
            }
            catch (MishimaPersistenceException ex)
            {
                return MishimaRepositoryPersistenceRouter.MapAfterReplace<TEntity, TId>(ex, entity);
            }
        }

        return Update(entity);
    }

    /// <inheritdoc />
    public async Task<Robotico.Result.Result> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (TryGetAsyncCollection(out IMishimaAsyncCollection? asyncCollection) && asyncCollection is not null)
        {
            string documentId = MishimaDocumentIdFormatter.Format(entity.Id);
            try
            {
                await asyncCollection.DeleteAsync(documentId, cancellationToken).ConfigureAwait(false);
                return Robotico.Result.Result.Success();
            }
            catch (MishimaPersistenceException ex)
            {
                return MishimaRepositoryPersistenceRouter.MapAfterDelete<TEntity, TId>(ex, entity);
            }
        }

        return Remove(entity);
    }

    private bool TryGetAsyncCollection(out IMishimaAsyncCollection? asyncCollection)
    {
        if (_database is IMishimaAsyncDatabase asyncDatabase && asyncDatabase.HasAsyncPersistence)
        {
            asyncCollection = asyncDatabase.GetAsyncCollection(_collectionName);
            return true;
        }

        asyncCollection = null;
        return false;
    }
}

#pragma warning restore MA0042
