using System.Collections.Generic;
using Moq;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.TestHarness;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class RelativityUpdateFolderTests : StepTestBase<RelativityUpdateFolder, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Update Folder",
                    new RelativityUpdateFolder()
                    {
                        FolderName          = Constant("NewName"),
                        FolderId            = Constant(22),
                        WorkspaceArtifactId = Constant(11)
                    },
                    Unit.Default
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetupUnit<IFolderManager>(
                        manager =>
                            manager.UpdateSingleAsync(
                                11,
                                It.Is<Folder>(x => x.ArtifactID == 22 && x.Name == "NewName")
                            )
                    )
                );
        }
    }
}

}
