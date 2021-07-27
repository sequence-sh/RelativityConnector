using System.Collections.Generic;
using System.Threading;
using Moq;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityMoveFoldersTests : StepTestBase<RelativityMoveFolder, Entity>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                yield return new StepCase("Move Folder",
                        new Log<Entity>()
                        {
                            Value = new RelativityMoveFolder()
                            {
                                WorkspaceArtifactId = Constant(11),
                                DestinationFolderArtifactId = Constant(22),
                                FolderArtifactId = Constant(33)
                            }
                        }, Unit.Default,
                        "(TotalOperations: 1 ProcessState: \"Complete\" OperationsCompleted: 1)"
                    ).WithTestRelativitySettings()
                    .WithService(
                        new MockSetup<IFolderManager, FolderMoveResultSet>(
                            x => x.MoveFolderAsync(11, 33, 22, It.IsAny<CancellationToken>()),
                            new FolderMoveResultSet()
                            {
                                OperationsCompleted = 1,
                                TotalOperations = 1,
                                ProcessState = "Complete"
                            }
                        ));
            }
        }
    }
}