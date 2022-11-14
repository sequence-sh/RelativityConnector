using Moq;
using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Objects.DataContracts;

namespace Sequence.Connectors.Relativity.Tests.Steps;

public partial class RelativityDeleteDocumentTests : StepTestBase<RelativityDeleteDocument, Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Delete Document",
                    new Log
                    {
                        Value = new RelativityDeleteDocument
                        {
                            Workspace        = new OneOfStep<SCLInt, StringStream>(Constant(11)),
                            ObjectArtifactId = Constant(22)
                        }
                    },
                    Unit.Default,
                    @"('Count': 1 'DeletedItems': [('ObjectTypeName': ""document"" 'Action': ""delete"" 'Count': 1 'Connection': ""object"")])"
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IObjectManager1, DeleteResult>(
                        x =>
                            x.DeleteAsync(
                                11,
                                It.Is<DeleteRequest>(dr => dr.Object.ArtifactID == 22),
                                It.IsAny<CancellationToken>()
                            ),
                        new DeleteResult
                        {
                            Report = new DeleteReport
                            {
                                DeletedItems = new List<DeleteItem>
                                {
                                    new()
                                    {
                                        Action         = "delete",
                                        Connection     = "object",
                                        Count          = 1,
                                        ObjectTypeName = "document"
                                    }
                                }
                            }
                        }
                    )
                );
        }
    }
}
