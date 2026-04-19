using MishimaDocs;
using MishimaDocs.IO;
using Xunit;

namespace Robotico.Repository.Mishima.Tests;

public sealed class MishimaRepositoryAsyncTests
{
    [Fact]
    public async Task AddAsync_GetByIdAsync_round_trips_with_async_database()
    {
        string path = Path.Combine(Path.GetTempPath(), "robotico-mishima-async-" + Guid.NewGuid().ToString("N", null) + ".mishima");
        PhysicalFileAccess io = new();
        IMishimaDatabase db = await MishimaDatabaseFactory.OpenOrCreateAsync(
            new MishimaOpenOptions { DatabaseFilePath = path, CreateIfNotExists = true },
            io);

        try
        {
            IMishimaAsyncDatabase asyncDb = Assert.IsAssignableFrom<IMishimaAsyncDatabase>(db);
            Assert.True(asyncDb.HasAsyncPersistence);
            MishimaRepository<SampleEntity, Guid> repo = new MishimaRepository<SampleEntity, Guid>(db, "orders");
            Guid id = Guid.NewGuid();
            SampleEntity entity = new SampleEntity { Id = id, Name = "async" };

            Assert.True((await repo.AddAsync(entity)).IsSuccess());
            Robotico.Result.Result<SampleEntity> got = await repo.GetByIdAsync(id);
            Assert.True(got.IsSuccess(out SampleEntity? loaded));
            Assert.NotNull(loaded);
            Assert.Equal("async", loaded.Name);
        }
        finally
        {
            db.Dispose();
            TryDelete(path);
            TryDelete(path + ".journal");
        }
    }

    [Fact]
    public void MishimaUnitOfWork_capabilities_match_immediate_commit_semantics()
    {
        MishimaUnitOfWork uow = new MishimaUnitOfWork();
        Assert.Equal(global::Robotico.Repository.UnitOfWorkCommitMode.NoOpCommitSuccess, uow.Capabilities.CommitMode);
        Assert.False(uow.Capabilities.CommitCoordinatesDomainWrites);
        Assert.False(uow.Capabilities.SupportsTransactions);
    }

    [Fact]
    public void UnitOfWorkGuard_DeferredUntilCommit_throws_for_mishima_uow()
    {
        MishimaUnitOfWork uow = new MishimaUnitOfWork();
        Assert.Throws<InvalidOperationException>(() =>
            global::Robotico.Repository.UnitOfWorkGuard.Require(
                uow,
                global::Robotico.Repository.UnitOfWorkRequirement.DeferredUntilCommit));
    }

    private static void TryDelete(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
