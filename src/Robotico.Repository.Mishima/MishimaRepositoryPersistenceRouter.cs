using MishimaDocs;
using Robotico.Domain;
using Robotico.Result.Errors;

namespace Robotico.Repository.Mishima;

/// <summary>
/// Maps <see cref="MishimaPersistenceException"/> to <see cref="Robotico.Result.Result"/> failures.
/// </summary>
internal static class MishimaRepositoryPersistenceRouter
{
    internal static Robotico.Result.Result<TEntity> MapAfterGetById<TEntity, TId>(MishimaPersistenceException ex, TId id)
        where TEntity : IEntity<TId>
        where TId : notnull
    {
        _ = id;
        return Robotico.Result.Result.Error<TEntity>(Map(ex));
    }

    internal static Robotico.Result.Result MapAfterAdd<TEntity, TId>(MishimaPersistenceException ex, TEntity entity)
        where TEntity : IEntity<TId>
        where TId : notnull
    {
        if (ex.ErrorCode == MishimaPersistenceErrorCode.DocumentAlreadyExists)
        {
            return Robotico.Result.Result.Error(new SimpleError($"Entity with id '{entity.Id}' already exists.", "DUPLICATE"));
        }

        return Robotico.Result.Result.Error(Map(ex));
    }

    internal static Robotico.Result.Result MapAfterReplace<TEntity, TId>(MishimaPersistenceException ex, TEntity entity)
        where TEntity : IEntity<TId>
        where TId : notnull
    {
        if (ex.ErrorCode == MishimaPersistenceErrorCode.DocumentNotFound)
        {
            return Robotico.Result.Result.Error(new SimpleError($"Entity with id '{entity.Id}' not found.", "NOT_FOUND"));
        }

        return Robotico.Result.Result.Error(Map(ex));
    }

    internal static Robotico.Result.Result MapAfterDelete<TEntity, TId>(MishimaPersistenceException ex, TEntity entity)
        where TEntity : IEntity<TId>
        where TId : notnull
    {
        if (ex.ErrorCode == MishimaPersistenceErrorCode.DocumentNotFound)
        {
            return Robotico.Result.Result.Error(new SimpleError($"Entity with id '{entity.Id}' not found.", "NOT_FOUND"));
        }

        return Robotico.Result.Result.Error(Map(ex));
    }

    private static ExceptionError Map(MishimaPersistenceException ex) => new(ex);
}
