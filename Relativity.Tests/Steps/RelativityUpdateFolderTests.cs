using System.Collections.Generic;
using System.Net.Http;
using Flurl.Http.Testing;
using Moq;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
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
                    "Update Folder with service mock",
                    new RelativityUpdateFolder()
                    {
                        FolderName          = Constant("NewName"),
                        FolderId            = Constant(22),
                        WorkspaceArtifactId = Constant(11)
                    },
                    Unit.Default
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetupUnit<IFolderManager1>(
                        manager =>
                            manager.UpdateSingleAsync(
                                11,
                                It.Is<Folder>(x => x.ArtifactID == 22 && x.Name == "NewName")
                            )
                    )
                );

            HttpTest httpTest = new ();

            httpTest.ForCallsTo("http://TestRelativityServer/Relativity.REST/api/Relativity.Services.Folder.IFolderModule/Folder%20Manager/UpdateSingleAsync")
                .WithVerb(HttpMethod.Post)
                .RespondWith();

            httpTest.RespondWith(status: 400);

            yield return new StepCase(
                    "Update Folder with http mock",
                    new RelativityUpdateFolder()
                    {
                        FolderName          = Constant("NewName"),
                        FolderId            = Constant(22),
                        WorkspaceArtifactId = Constant(11)
                    },
                    Unit.Default
                ).WithTestRelativitySettings()
                .WithFlurlMocks(httpTest);
        }
    }
}

}
