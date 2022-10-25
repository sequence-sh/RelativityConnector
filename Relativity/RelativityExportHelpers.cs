using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Reductech.Sequence.Connectors.Relativity.Errors;
using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Kepler.Transport;
using Relativity.Services.DataContracts.DTOs.Results;
using Relativity.Services.Objects.DataContracts;

#pragma warning disable CS1591

namespace Reductech.Sequence.Connectors.Relativity;

/// <summary>
/// Contains methods to help with Relativity export
/// </summary>
public static class RelativityExportHelpers
{
    public static async Task<Result<Array<Entity>, IError>> ExportAsync(
        int workspaceId,
        ArtifactType artifactType,
        IReadOnlyList<string> fieldNames,
        string condition,
        int start,
        int batchSize,
        IDocumentFileManager1 documentFileManager,
        IObjectManager1 objectManager,
        ErrorLocation errorLocation,
        CancellationToken cancellationToken)
    {
        var setupExportResult = await SetupExportAsync(
            workspaceId,
            artifactType,
            fieldNames,
            condition,
            start,
            objectManager
        );

        if (setupExportResult.IsFailure)
            return setupExportResult.MapError(x => x.WithLocation(errorLocation))
                .ConvertFailure<Array<Entity>>();

        var allResults = GetResultElements(
            setupExportResult.Value,
            workspaceId,
            batchSize,
            fieldNames,
            documentFileManager,
            objectManager,
            errorLocation,
            cancellationToken
        );

        var array = new LazyArray<Entity>(allResults);

        return array;
    }

    /// <summary>
    /// A token indicating a long string that should be downloaded separately
    /// </summary>
    public const string LongStringToken = "#KCURA99DF2F0FEB88420388879F1282A55760#";

    public static readonly EntityKey NativeFileKey = new("NativeFile");

    public static async IAsyncEnumerable<Entity> GetResultElements(
        ExportInitializationResults exportResult,
        int workspaceId,
        int batchSize,
        IReadOnlyList<string> fieldNames,
        IDocumentFileManager1 documentFileManager,
        IObjectManager1 objectManager,
        ErrorLocation errorLocation,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var fields = fieldNames.Select(x => new Field() { Name = x }).ToList();

        var current = 0;

        while (current < exportResult.RecordCount)
        {
            var resultElements =
                await objectManager.RetrieveNextResultsBlockFromExportAsync(
                    workspaceId,
                    exportResult.RunID,
                    batchSize
                );

            if (resultElements is not null)
            {
                foreach (var resultElement in resultElements)
                {
                    var headers = ImmutableArray.CreateBuilder<EntityKey>();
                    var values  = ImmutableArray.CreateBuilder<ISCLObject>();

                    var pairs = fields.Zip(resultElement.Values);

                    var order = 0;

                    foreach (var (field, fieldValue) in pairs)
                    {
                        if (fieldValue?.ToString() == LongStringToken)
                        {
                            var v = await
                                GetLongText(
                                    workspaceId,
                                    field.Name,
                                    resultElement.ArtifactID,
                                    objectManager
                                );

                            if (v.IsFailure)
                                throw new ErrorException(v.Error.WithLocation(errorLocation));

                            headers.Add(new EntityKey(field.Name));
                            values.Add(new StringStream(v.Value));
                        }
                        else
                        {
                            headers.Add(new EntityKey(field.Name));
                            values.Add(ISCLObject.CreateFromCSharpObject(fieldValue));
                        }

                        order++;
                    }

                    string data;

                    try
                    {
                        data =
                            await documentFileManager.DownloadDataAsync(
                                workspaceId,
                                resultElement.ArtifactID
                            );
                    }
                    catch (Exception e)
                    {
                        throw new ErrorException(
                            ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(e.Message)
                                .WithLocationSingle(errorLocation)
                        );
                    }

                    headers.Add(NativeFileKey);
                    values.Add(ISCLObject.CreateFromCSharpObject(new StringStream(data)));

                    var entity = new Entity(headers.ToImmutable(), values.ToImmutable());

                    yield return entity;
                }
            }

            current += batchSize;
        }
    }

    public static async Task<Result<ExportInitializationResults, IErrorBuilder>> SetupExportAsync(
        int workspaceId,
        ArtifactType artifactType,
        IReadOnlyList<string> fieldNames,
        string condition,
        int start,
        IObjectManager1 objectManager)
    {
        var request = new QueryRequest()
        {
            Condition  = condition,
            Fields     = fieldNames.Select(s => new FieldRef() { Name = s }).ToList(),
            ObjectType = new ObjectTypeRef() { ArtifactTypeID = (int)artifactType }
        };

        ExportInitializationResults exportResult;

        try
        {
            exportResult = await objectManager.InitializeExportAsync(workspaceId, request, start);
        }
        catch (Exception e)
        {
            return ErrorCode.Unknown.ToErrorBuilder(e);
        }

        return exportResult;
    }

    public static async Task<Result<string, IErrorBuilder>> GetLongText(
        int workspaceId,
        string fieldName,
        int artifactId,
        IObjectManager1 objectManager)
    {
        IKeplerStream keplerStream;

        try
        {
            keplerStream = await objectManager.StreamLongTextAsync(
                workspaceId,
                new RelativityObjectRef() { ArtifactID = artifactId },
                new FieldRef() { Name                  = fieldName }
            );
        }
        catch (Exception e)
        {
            return ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(e.Message);
        }

        var r = await ReadKeplerStream(keplerStream);
        keplerStream.Dispose();

        return r;
    }

    private static async Task<Result<string, IErrorBuilder>> ReadKeplerStream(
        IKeplerStream keplerStream)
    {
        try
        {
            await using var stream = await keplerStream.GetStreamAsync();

            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            using var streamReader = new StreamReader(stream);

            var t = await streamReader.ReadToEndAsync();
            return t;
        }
        catch (Exception e)
        {
            return ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(e.Message);
        }
    }
}
