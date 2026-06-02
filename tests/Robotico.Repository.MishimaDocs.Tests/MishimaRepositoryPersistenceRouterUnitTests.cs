using MishimaDocs;
using Robotico.Result.Errors;
using Xunit;

namespace Robotico.Repository.MishimaDocs.Tests;

/// <summary>Non-property branches on <see cref="MishimaDocsRepositoryPersistenceRouter"/>.</summary>
public sealed class MishimaDocsRepositoryPersistenceRouterUnitTests
{
    [Fact]
    public void MapAfterAdd_non_duplicate_maps_to_exception_error()
    {
        MishimaPersistenceException ex = new MishimaPersistenceException(MishimaPersistenceErrorCode.InvalidArgument, "x");
        SampleEntity entity = new SampleEntity { Id = Guid.NewGuid(), Name = "e" };

        Robotico.Result.Result r = MishimaDocsRepositoryPersistenceRouter.MapAfterAdd<SampleEntity, Guid>(ex, entity);

        Assert.True(r.IsError(out IError? err));
        Assert.IsType<ExceptionError>(err);
    }

    [Fact]
    public void MapAfterReplace_non_not_found_maps_to_exception_error()
    {
        MishimaPersistenceException ex = new MishimaPersistenceException(MishimaPersistenceErrorCode.InvalidArgument, "x");
        SampleEntity entity = new SampleEntity { Id = Guid.NewGuid(), Name = "e" };

        Robotico.Result.Result r = MishimaDocsRepositoryPersistenceRouter.MapAfterReplace<SampleEntity, Guid>(ex, entity);

        Assert.True(r.IsError(out IError? err));
        Assert.IsType<ExceptionError>(err);
    }

    [Fact]
    public void MapAfterDelete_non_not_found_maps_to_exception_error()
    {
        MishimaPersistenceException ex = new MishimaPersistenceException(MishimaPersistenceErrorCode.InvalidArgument, "x");
        SampleEntity entity = new SampleEntity { Id = Guid.NewGuid(), Name = "e" };

        Robotico.Result.Result r = MishimaDocsRepositoryPersistenceRouter.MapAfterDelete<SampleEntity, Guid>(ex, entity);

        Assert.True(r.IsError(out IError? err));
        Assert.IsType<ExceptionError>(err);
    }
}
