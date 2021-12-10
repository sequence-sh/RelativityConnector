using Moq;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

public partial class RelativityMoveFoldersTests : StepTestBase<RelativityMoveFolder, Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Move Folder",
                    new Log<Entity>()
                    {
                        Value = new RelativityMoveFolder()
                        {
                            Workspace = new OneOfStep<int, StringStream>(Constant(11)),
                            DestinationFolderArtifactId = Constant(22),
                            FolderArtifactId            = Constant(33)
                        }
                    },
                    Unit.Default,
                    @"('TotalOperations': 1 'OperationsCompleted': 1 'ProcessState': ""Complete"")"
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IFolderManager1, FolderMoveResultSet>(
                        x => x.MoveFolderAsync(11, 33, 22, It.IsAny<CancellationToken>()),
                        new FolderMoveResultSet()
                        {
                            OperationsCompleted = 1,
                            TotalOperations     = 1,
                            ProcessState        = "Complete"
                        }
                    )
                );
        }
    }
}
