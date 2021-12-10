using System.Threading;
using Moq;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Relativity.Services.Folder;
using Relativity.Services.Objects.DataContracts;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

public partial class
    RelativityDeleteUnusedFoldersTests : StepTestBase<RelativityDeleteUnusedFolders, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Delete Unused Folders using workspace id",
                    new RelativityDeleteUnusedFolders()
                    {
                        Workspace = new OneOfStep<int, StringStream>(Constant(42)),
                    },
                    Unit.Default
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IFolderManager1, FolderResultSet>(
                        x => x.DeleteUnusedFoldersAsync(42),
                        new FolderResultSet() { Success = true }
                    )
                );

            yield return new StepCase(
                    "Delete Unused Folders using workspace name",
                    new RelativityDeleteUnusedFolders()
                    {
                        Workspace = new OneOfStep<int, StringStream>(Constant("My Workspace")),
                    },
                    Unit.Default
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IObjectManager1, QueryResultSlim>(
                        x => x.QuerySlimAsync(
                            -1,
                            It.Is<QueryRequest>(queryRequest => queryRequest.Condition == "'Name' == 'My Workspace'"),
                            0,
                            1,
                            It.IsAny<CancellationToken>()
                        ),
                        new QueryResultSlim()
                        {
                            Objects = new List<RelativityObjectSlim>()
                            {
                                new() { ArtifactID = 42 }
                            }
                        }
                    ),
                    new MockSetup<IFolderManager1, FolderResultSet>(
                        x => x.DeleteUnusedFoldersAsync(42),
                        new FolderResultSet() { Success = true }
                    )
                );
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            yield return new ErrorCase(
                    "Result unsuccessful",
                    new RelativityDeleteUnusedFolders()
                    {
                        Workspace = new OneOfStep<int, StringStream>(Constant(42)),
                    },
                    ErrorCode_Relativity.Unsuccessful.ToErrorBuilder("Test Error Message")
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IFolderManager1, FolderResultSet>(
                        x => x.DeleteUnusedFoldersAsync(42),
                        new FolderResultSet() { Success = false, Message = "Test Error Message" }
                    )
                );

            yield return new ErrorCase(
                    "Workspace search unsuccessful",
                    new RelativityDeleteUnusedFolders()
                    {
                        Workspace = new OneOfStep<int, StringStream>(Constant("Test Workspace")),
                    },
                    ErrorCode_Relativity.ObjectNotFound.ToErrorBuilder("Case", "Test Workspace")
                ).WithTestRelativitySettings()
                .WithFlurlMocks(x => x.RespondWith(status: 400));

            yield return new ErrorCase(
                        "Workspace does not exist",
                        new RelativityDeleteUnusedFolders()
                        {
                            Workspace = new OneOfStep<int, StringStream>(Constant("Test Workspace")),
                        },
                        ErrorCode_Relativity.ObjectNotFound.ToErrorBuilder("Case", "Test Workspace")
                    ).WithTestRelativitySettings()
                    .WithFlurlMocks(
                        x => x.RespondWithJson(
                            new QueryResultSlim { Objects = new List<RelativityObjectSlim> { } }
                        )
                    )
                ;

            foreach (var errorCase in base.ErrorCases)
            {
                yield return errorCase;
            }
        }
    }
}
