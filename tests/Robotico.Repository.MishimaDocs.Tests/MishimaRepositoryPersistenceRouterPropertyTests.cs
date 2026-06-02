using CsCheck;
using MishimaDocs;
using Robotico.Result.Errors;
using Xunit;

namespace Robotico.Repository.MishimaDocs.Tests;

/// <summary>Property-based checks for <see cref="MishimaDocsRepositoryPersistenceRouter"/> classification.</summary>
public sealed class MishimaDocsRepositoryPersistenceRouterPropertyTests
{
    [Fact]
    public void MapAfterAdd_DocumentAlreadyExists_always_has_code_DUPLICATE()
    {
        Gen.Guid.Sample(static entityId =>
        {
            MishimaPersistenceException ex = new MishimaPersistenceException(MishimaPersistenceErrorCode.DocumentAlreadyExists, "dup");
            SampleEntity entity = new SampleEntity { Id = entityId, Name = "e" };
            Robotico.Result.Result r = MishimaDocsRepositoryPersistenceRouter.MapAfterAdd<SampleEntity, Guid>(ex, entity);
            return r.IsError(out IError? err) && err!.Code == "DUPLICATE";
        });
    }

    [Fact]
    public void MapAfterReplace_DocumentNotFound_always_has_code_NOT_FOUND()
    {
        Gen.Guid.Sample(static entityId =>
        {
            MishimaPersistenceException ex = new MishimaPersistenceException(MishimaPersistenceErrorCode.DocumentNotFound, "nf");
            SampleEntity entity = new SampleEntity { Id = entityId, Name = "e" };
            Robotico.Result.Result r = MishimaDocsRepositoryPersistenceRouter.MapAfterReplace<SampleEntity, Guid>(ex, entity);
            return r.IsError(out IError? err) && err!.Code == "NOT_FOUND";
        });
    }

    [Fact]
    public void MapAfterDelete_DocumentNotFound_always_has_code_NOT_FOUND()
    {
        Gen.Guid.Sample(static entityId =>
        {
            MishimaPersistenceException ex = new MishimaPersistenceException(MishimaPersistenceErrorCode.DocumentNotFound, "nf");
            SampleEntity entity = new SampleEntity { Id = entityId, Name = "e" };
            Robotico.Result.Result r = MishimaDocsRepositoryPersistenceRouter.MapAfterDelete<SampleEntity, Guid>(ex, entity);
            return r.IsError(out IError? err) && err!.Code == "NOT_FOUND";
        });
    }

    [Fact]
    public void MapAfterGetById_any_code_sample_returns_error_result()
    {
        Gen.Guid.Sample(static id =>
        {
            MishimaPersistenceException ex = new MishimaPersistenceException(MishimaPersistenceErrorCode.DocumentNotFound, "x");
            Robotico.Result.Result<SampleEntity> r =
                MishimaDocsRepositoryPersistenceRouter.MapAfterGetById<SampleEntity, Guid>(ex, id);
            return r.IsError();
        });
    }
}
