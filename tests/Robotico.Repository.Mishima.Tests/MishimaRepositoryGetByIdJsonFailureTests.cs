using System.Text.Json;
using MishimaDocs;
using MishimaDocs.IO;
using Robotico.Result.Errors;
using Xunit;

namespace Robotico.Repository.Mishima.Tests;

/// <summary><see cref="Robotico.Repository.Mishima.MishimaRepository{TEntity, TId}"/> maps JSON deserialization failures to exception errors.</summary>
public sealed class MishimaRepositoryGetByIdJsonFailureTests
{
    private static IMishimaDatabase CreateDatabase()
    {
        string path = Path.Combine(Path.GetTempPath(), "robotico-mishima-jsonfail-" + Guid.NewGuid().ToString("N", null) + ".mishima");
        MishimaOpenOptions options = new() { DatabaseFilePath = path, CreateIfNotExists = true };
        return MishimaDatabaseFactory.OpenOrCreate(options, new PhysicalFileAccess());
    }

    [Fact]
    public void GetById_returns_error_when_payload_is_not_valid_entity_json()
    {
        using IMishimaDatabase db = CreateDatabase();
        MishimaRepository<SampleEntity, Guid> repo = new MishimaRepository<SampleEntity, Guid>(db, "badjson");
        Guid id = Guid.NewGuid();
        string documentId = MishimaDocumentIdFormatter.Format(id);
        IMishimaCollection collection = db.GetCollection("badjson");
        JsonElement badShape = JsonDocument.Parse("{\"Id\": \"not-a-guid\", \"Name\": \"x\"}").RootElement;
        collection.Insert(documentId, badShape);

        Robotico.Result.Result<SampleEntity> got = repo.GetById(id);

        Assert.True(got.IsError(out IError? err));
        Assert.IsType<ExceptionError>(err);
    }
}
