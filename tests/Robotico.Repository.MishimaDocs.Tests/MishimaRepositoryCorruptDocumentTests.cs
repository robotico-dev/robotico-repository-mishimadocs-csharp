using System.Text.Json;
using MishimaDocs;
using MishimaDocs.IO;
using Robotico.Result.Errors;
using Xunit;

namespace Robotico.Repository.MishimaDocs.Tests;

/// <summary>Stored JSON that does not deserialize to <typeparamref name="TEntity"/> yields CORRUPT.</summary>
public sealed class MishimaDocsRepositoryCorruptDocumentTests
{
    private static IMishimaDatabase CreateDatabase()
    {
        string path = Path.Combine(Path.GetTempPath(), "robotico-mishima-corrupt-" + Guid.NewGuid().ToString("N", null) + ".mishima");
        MishimaOpenOptions options = new() { DatabaseFilePath = path, CreateIfNotExists = true };
        return MishimaDatabaseFactory.OpenOrCreate(options, new PhysicalFileAccess());
    }

    [Fact]
    public void GetById_returns_CORRUPT_when_document_is_JSON_null()
    {
        using IMishimaDatabase db = CreateDatabase();
        MishimaDocsRepository<SampleEntity, Guid> repo = new MishimaDocsRepository<SampleEntity, Guid>(db, "corrupt");
        Guid id = Guid.NewGuid();
        string documentId = id.ToString("D");
        IMishimaCollection collection = db.GetCollection("corrupt");
        JsonElement nullPayload = JsonDocument.Parse("null").RootElement;
        collection.Insert(documentId, nullPayload);

        Robotico.Result.Result<SampleEntity> got = repo.GetById(id);

        Assert.True(got.IsError(out IError? err));
        Assert.Equal("CORRUPT", err!.Code);
    }
}
