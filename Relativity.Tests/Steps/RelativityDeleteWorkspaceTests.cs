using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Workspace;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityDeleteWorkspaceTests : StepTestBase<RelativityDeleteWorkspace, Unit>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                void SetupWorkspaceManager(Mock<IWorkspaceManager> workspaceManager)
                {
                    workspaceManager.Setup(x => x.DeleteAsync(42, CancellationToken.None))
                        .Returns(Task.CompletedTask);

                    workspaceManager.Setup(x => x.Dispose());
                }


                yield return new StepCase("Delete a Workspace",
                        new RelativityDeleteWorkspace()
                        {
                            WorkspaceId = StaticHelpers.Constant(42)
                        }, Unit.Default
                    ).WithTestRelativitySettings()
                    .WithService((Action<Mock<IWorkspaceManager>>)SetupWorkspaceManager);
            }
        }
    }
}