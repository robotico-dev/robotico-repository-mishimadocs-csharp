using System.Text.Json;
using MishimaDocs;
using Robotico.Domain;
using Robotico.Result.Errors;

namespace Robotico.Repository.MishimaDocs;

/// <summary>
/// MishimaDocs implementation of <see cref="IRepository{TEntity, TId}"/> using one named collection and JSON documents.
/// </summary>
/// <remarks>
/// <para>Each operation maps the entity to a JSON document and uses <see cref="IMishimaCollection"/> CRUD APIs. MishimaDocs commits each write immediately; use <see cref="MishimaDocsUnitOfWork"/> when the host expects an <see cref="IUnitOfWork"/> (CommitAsync is a no-op success for parity with other adapters).</para>
/// <para><typeparamref name="TId"/> is formatted with <see cref="MishimaDocumentIdFormatter"/>; ensure ids are unique and stable for your domain.</para>
/// <para><see cref="Robotico.Repository.IAsyncRepository{TEntity, TId}"/> uses <see cref="IMishimaAsyncCollection"/> when the database was opened as <see cref="IMishimaAsyncDatabase"/> with <see cref="IMishimaAsyncDatabase.HasAsyncPersistence"/>; otherwise async methods delegate to the synchronous implementation. Reads use MishimaDocs synchronous APIs (see engine documentation).</para>
/// </remarks>
/// <typeparam name="TEntity">The entity type (must implement <see cref="IEntity{TId}"/>).</typeparam>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public sealed partial class MishimaDocsRepository<TEntity, TId>
    : Robotico.Repository.IRepository<TEntity, TId>, Robotico.Repository.IAsyncRepository<TEntity, TId>
    where TEntity : IEntity<TId>
    where TId : notnull
{
    private readonly IMishimaDatabase _database;
    private readonly string _collectionName;
    private readonly IMishimaCollection _collection;
    private readonly IMishimaWriteBatch? _writeBatch;

    /// <summary>Initializes a new repository for the named collection.</summary>
    public MishimaDocsRepository(IMishimaDatabase database, string collectionName)
        : this(database, collectionName, null)
    {
    }

    /// <summary>Initializes a repository optionally bound to a MishimaDocs write batch for atomic multi-collection commits.</summary>
    public MishimaDocsRepository(IMishimaDatabase database, string collectionName, IMishimaWriteBatch? writeBatch)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _collectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
        if (collectionName.Length == 0)
        {
            throw new ArgumentException("Collection name must not be empty.", nameof(collectionName));
        }

        _writeBatch = writeBatch;
        _collection = GetCollection(database, collectionName);
    }

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

            TEntity? entity = JsonSerializer.Deserialize<TEntity>(element.Value, MishimaDocsRepositoryJsonOptions.Instance);
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
            return MishimaDocsRepositoryPersistenceRouter.MapAfterGetById<TEntity, TId>(ex, id);
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
            JsonElement payload = JsonSerializer.SerializeToElement(entity, MishimaDocsRepositoryJsonOptions.Instance);
            if (_writeBatch is null)
            {
                _collection.Insert(documentId, payload);
            }
            else
            {
                if (_collection.TryGetById(documentId) is not null)
                {
                    return Robotico.Result.Result.Error(new SimpleError($"Entity with id '{entity.Id}' already exists.", "DUPLICATE"));
                }

                _writeBatch.Upsert(_collectionName, documentId, payload);
            }

            return Robotico.Result.Result.Success();
        }
        catch (MishimaPersistenceException ex)
        {
            return MishimaDocsRepositoryPersistenceRouter.MapAfterAdd<TEntity, TId>(ex, entity);
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
            JsonElement payload = JsonSerializer.SerializeToElement(entity, MishimaDocsRepositoryJsonOptions.Instance);
            if (_writeBatch is null)
            {
                _collection.Replace(documentId, payload);
            }
            else
            {
                if (_collection.TryGetById(documentId) is null)
                {
                    return Robotico.Result.Result.Error(new SimpleError($"Entity with id '{entity.Id}' not found.", "NOT_FOUND"));
                }

                _writeBatch.Upsert(_collectionName, documentId, payload);
            }

            return Robotico.Result.Result.Success();
        }
        catch (MishimaPersistenceException ex)
        {
            return MishimaDocsRepositoryPersistenceRouter.MapAfterReplace<TEntity, TId>(ex, entity);
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
            if (_writeBatch is null)
            {
                _collection.Delete(documentId);
            }
            else
            {
                if (_collection.TryGetById(documentId) is null)
                {
                    return Robotico.Result.Result.Error(new SimpleError($"Entity with id '{entity.Id}' not found.", "NOT_FOUND"));
                }

                _writeBatch.Delete(_collectionName, documentId);
            }

            return Robotico.Result.Result.Success();
        }
        catch (MishimaPersistenceException ex)
        {
            return MishimaDocsRepositoryPersistenceRouter.MapAfterDelete<TEntity, TId>(ex, entity);
        }
    }

    private static IMishimaCollection GetCollection(IMishimaDatabase database, string collectionName)
    {
        ArgumentNullException.ThrowIfNull(database);
        return database.GetCollection(collectionName);
    }
}
