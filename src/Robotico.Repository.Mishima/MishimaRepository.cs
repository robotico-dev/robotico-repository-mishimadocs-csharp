using System.Text.Json;
using MishimaDocs;
using Robotico.Domain;
using Robotico.Repository;
using Robotico.Result.Errors;

namespace Robotico.Repository.Mishima;

/// <summary>
/// MishimaDocs implementation of <see cref="IRepository{TEntity, TId}"/> using one named collection and JSON documents.
/// </summary>
/// <remarks>
/// <para>Each operation maps the entity to a JSON document and uses <see cref="IMishimaCollection"/> CRUD APIs. MishimaDocs commits each write immediately; use <see cref="MishimaUnitOfWork"/> when the host expects an <see cref="IUnitOfWork"/> (CommitAsync is a no-op success for parity with other adapters).</para>
/// <para><typeparamref name="TId"/> is formatted with <see cref="MishimaDocumentIdFormatter"/>; ensure ids are unique and stable for your domain.</para>
/// </remarks>
/// <typeparam name="TEntity">The entity type (must implement <see cref="IEntity{TId}"/>).</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public sealed class MishimaRepository<TEntity, TId>(IMishimaDatabase database, string collectionName) : IRepository<TEntity, TId>
    where TEntity : IEntity<TId>
    where TId : notnull
{
    private readonly IMishimaCollection _collection = GetCollection(database, collectionName);

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null.</exception>
    public Robotico.Result.Result<TEntity> GetById(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        string documentId = MishimaDocumentIdFormatter.Format(id);
        try
        {
            JsonElement? element = _collection.TryGetById(documentId);
            if (element is null)
            {
                return Robotico.Result.Result.Error<TEntity>(new SimpleError($"Entity with id '{id}' not found.", "NOT_FOUND"));
            }

            TEntity? entity = JsonSerializer.Deserialize<TEntity>(element.Value, MishimaRepositoryJsonOptions.Instance);
            return entity is null
                ? Robotico.Result.Result.Error<TEntity>(new SimpleError("Stored document could not be deserialized.", "CORRUPT"))
                : Robotico.Result.Result.Success(entity);
        }
        catch (JsonException ex)
        {
            return Robotico.Result.Result.Error<TEntity>(new ExceptionError(ex));
        }
        catch (MishimaPersistenceException ex)
        {
            return MishimaRepositoryPersistenceRouter.MapAfterGetById<TEntity, TId>(ex, id);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
    public Robotico.Result.Result Add(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        string documentId = MishimaDocumentIdFormatter.Format(entity.Id);
        try
        {
            JsonElement payload = JsonSerializer.SerializeToElement(entity, MishimaRepositoryJsonOptions.Instance);
            _collection.Insert(documentId, payload);
            return Robotico.Result.Result.Success();
        }
        catch (MishimaPersistenceException ex)
        {
            return MishimaRepositoryPersistenceRouter.MapAfterAdd<TEntity, TId>(ex, entity);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
    public Robotico.Result.Result Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        string documentId = MishimaDocumentIdFormatter.Format(entity.Id);
        try
        {
            JsonElement payload = JsonSerializer.SerializeToElement(entity, MishimaRepositoryJsonOptions.Instance);
            _collection.Replace(documentId, payload);
            return Robotico.Result.Result.Success();
        }
        catch (MishimaPersistenceException ex)
        {
            return MishimaRepositoryPersistenceRouter.MapAfterReplace<TEntity, TId>(ex, entity);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
    public Robotico.Result.Result Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        string documentId = MishimaDocumentIdFormatter.Format(entity.Id);
        try
        {
            _collection.Delete(documentId);
            return Robotico.Result.Result.Success();
        }
        catch (MishimaPersistenceException ex)
        {
            return MishimaRepositoryPersistenceRouter.MapAfterDelete<TEntity, TId>(ex, entity);
        }
    }

    private static IMishimaCollection GetCollection(IMishimaDatabase database, string collectionName)
    {
        ArgumentNullException.ThrowIfNull(database);
        ArgumentNullException.ThrowIfNull(collectionName);
        if (collectionName.Length == 0)
        {
            throw new ArgumentException("Collection name must not be empty.", nameof(collectionName));
        }

        return database.GetCollection(collectionName);
    }
}
