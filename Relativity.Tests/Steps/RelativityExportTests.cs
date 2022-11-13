using System.IO;
using Moq;
using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Kepler.Transport;
using Relativity.Services.DataContracts.DTOs.Results;
using Relativity.Services.Field;
using Relativity.Services.Objects.DataContracts;
using FieldRef = Relativity.Services.Objects.DataContracts.FieldRef;

namespace Sequence.Connectors.Relativity.Tests.Steps;

public partial class RelativityExportTests : StepTestBase<RelativityExport, Array<Entity>>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            var runId = new Guid("8321b8c6-7b47-4cf7-a309-ad163ea907d0");

            yield return new StepCase(
                    "Export with condition",
                    new ForEach<Entity>
                    {
                        Array = new RelativityExport
                        {
                            Workspace  = new OneOfStep<SCLInt, StringStream>(Constant(12345)),
                            Condition  = Constant("'Extracted Text' ISSET "),
                            FieldNames = Array("ShortField", "LongField"),
                            BatchSize  = Constant(10)
                        },
                        Action = new LambdaFunction<Entity, Unit>(
                            null,
                            new Log
                            {
                                Value = new GetAutomaticVariable<Entity>()
                            }
                        )
                    },
                    Unit.Default,
                    "('ShortField': \"Hello\" 'LongField': \"Streamed Long Text\" 'NativeFile': \"My Native Text\")"
                )
                .WithTestRelativitySettings()
                .WithService(
                    //Initialize Export
                    new MockSetup<IObjectManager1, ExportInitializationResults>(
                        manager =>
                            manager.InitializeExportAsync(12345, It.IsAny<QueryRequest>(), 0),
                        new ExportInitializationResults
                        {
                            RecordCount = 10,
                            RunID       = runId,
                            FieldData =
                                new List<FieldMetadata>
                                {
                                    new() { Name = "ShortField" }, new() { Name = "LongField" },
                                }
                        }
                    ),

                    //Get results block
                    new MockSetup<IObjectManager1, RelativityObjectSlim[]>(
                        d => d.RetrieveNextResultsBlockFromExportAsync(12345, runId, 10),
                        new[]
                        {
                            new RelativityObjectSlim
                            {
                                ArtifactID = 111,
                                Values = new List<object>
                                {
                                    "Hello", RelativityExportHelpers.LongStringToken
                                }
                            }
                        }
                    ),

                    //Get long text
                    new MockSetup<IObjectManager1, IKeplerStream>(
                        d => d.StreamLongTextAsync(
                            12345,
                            It.Is<RelativityObjectRef>(x => x.ArtifactID == 111),
                            It.Is<FieldRef>(x => x.Name == "LongField")
                        ),
                        MakeKeplerStream("Streamed Long Text")
                    ),

                    //Get Native Text
                    new MockSetup<IDocumentFileManager1, string>(
                        d => d.DownloadDataAsync(12345, 111),
                        "My Native Text"
                    )
                );
        }
    }

    public static IKeplerStream MakeKeplerStream(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Seek(0, SeekOrigin.Begin);

        return new KeplerStream(stream);
    }

    /// <inheritdoc />
    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            foreach (var errorCase in base.ErrorCases)
            {
                yield return errorCase.WithTestRelativitySettings();
            }
        }
    }
}
