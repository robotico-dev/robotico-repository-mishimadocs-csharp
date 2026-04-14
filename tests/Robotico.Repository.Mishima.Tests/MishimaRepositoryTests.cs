using MishimaDocs;
using MishimaDocs.IO;
using Xunit;

namespace Robotico.Repository.Mishima.Tests;

/// <summary>Tests for <see cref="MishimaRepository{TEntity, TId}"/> against a temporary MishimaDocs file.</summary>
public sealed class MishimaRepositoryTests
{
    private static IMishimaDatabase CreateDatabase()
    {
        string path = Path.Combine(Path.GetTempPath(), "robotico-mishima-repo-" + Guid.NewGuid().ToString("N", null) + ".mishima");
        MishimaOpenOptions options = new() { DatabaseFilePath = path, CreateIfNotExists = true };
        return MishimaDatabaseFactory.OpenOrCreate(options, new PhysicalFileAccess());
    }

    [Fact]
    public void Add_then_GetById_round_trips()
    {
        using IMishimaDatabase db = CreateDatabase();
        var repo = new MishimaRepository<SampleEntity, Guid>(db, "orders");
        Guid id = Guid.NewGuid();
        var entity = new SampleEntity { Id = id, Name = "a" };

        Assert.True(repo.Add(entity).IsSuccess());
        Robotico.Result.Result<SampleEntity> got = repo.GetById(id);
        Assert.True(got.IsSuccess(out SampleEntity? loaded));
        Assert.NotNull(loaded);
        Assert.Equal("a", loaded.Name);
    }

    [Fact]
    public void GetById_returns_NOT_FOUND_when_missing()
    {
        using IMishimaDatabase db = CreateDatabase();
        var repo = new MishimaRepository<SampleEntity, Guid>(db, "orders");

        Robotico.Result.Result<SampleEntity> got = repo.GetById(Guid.NewGuid());

        Assert.False(got.IsSuccess());
    }

    [Fact]
    public void Add_duplicate_returns_DUPLICATE()
    {
        using IMishimaDatabase db = CreateDatabase();
        var repo = new MishimaRepository<SampleEntity, Guid>(db, "orders");
        Guid id = Guid.NewGuid();
        var entity = new SampleEntity { Id = id, Name = "x" };

        Assert.True(repo.Add(entity).IsSuccess());
        Robotico.Result.Result dup = repo.Add(new SampleEntity { Id = id, Name = "y" });

        Assert.False(dup.IsSuccess());
    }

    [Fact]
    public void Update_then_GetById_reflects_changes()
    {
        using IMishimaDatabase db = CreateDatabase();
        var repo = new MishimaRepository<SampleEntity, Guid>(db, "orders");
        Guid id = Guid.NewGuid();
        Assert.True(repo.Add(new SampleEntity { Id = id, Name = "v1" }).IsSuccess());
        Assert.True(repo.Update(new SampleEntity { Id = id, Name = "v2" }).IsSuccess());
        Robotico.Result.Result<SampleEntity> got = repo.GetById(id);
        Assert.True(got.IsSuccess(out SampleEntity? loaded));
        Assert.NotNull(loaded);
        Assert.Equal("v2", loaded.Name);
    }

    [Fact]
    public void Remove_then_GetById_fails()
    {
        using IMishimaDatabase db = CreateDatabase();
        var repo = new MishimaRepository<SampleEntity, Guid>(db, "orders");
        Guid id = Guid.NewGuid();
        var entity = new SampleEntity { Id = id, Name = "z" };
        Assert.True(repo.Add(entity).IsSuccess());
        Assert.True(repo.Remove(entity).IsSuccess());
        Assert.False(repo.GetById(id).IsSuccess());
    }

    [Fact]
    public void Remove_missing_returns_NOT_FOUND()
    {
        using IMishimaDatabase db = CreateDatabase();
        var repo = new MishimaRepository<SampleEntity, Guid>(db, "orders");
        Guid id = Guid.NewGuid();

        Robotico.Result.Result r = repo.Remove(new SampleEntity { Id = id, Name = "ghost" });

        Assert.False(r.IsSuccess());
    }

    [Fact]
    public void Update_missing_returns_NOT_FOUND()
    {
        using IMishimaDatabase db = CreateDatabase();
        var repo = new MishimaRepository<SampleEntity, Guid>(db, "orders");

        Robotico.Result.Result r = repo.Update(new SampleEntity { Id = Guid.NewGuid(), Name = "n" });

        Assert.False(r.IsSuccess());
    }

    [Fact]
    public void GetById_throws_when_id_null()
    {
        using IMishimaDatabase db = CreateDatabase();
        var repo = new MishimaRepository<SampleStringEntity, string>(db, "s");

        Assert.Throws<ArgumentNullException>(() => repo.GetById(null!));
    }

    [Fact]
    public async Task MishimaUnitOfWork_CommitAsync_succeeds()
    {
        var uow = new MishimaUnitOfWork();
        Robotico.Result.Result r = await uow.CommitAsync();
        Assert.True(r.IsSuccess());
    }
}
