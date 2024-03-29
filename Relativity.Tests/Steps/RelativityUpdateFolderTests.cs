﻿using System.Net.Http;
using Moq;
using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Folder;

namespace Sequence.Connectors.Relativity.Tests.Steps;

public partial class RelativityUpdateFolderTests : StepTestBase<RelativityUpdateFolder, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Update Folder with service mock",
                    new RelativityUpdateFolder
                    {
                        FolderName = Constant("NewName"),
                        FolderId   = Constant(22),
                        Workspace  = new OneOfStep<SCLInt, StringStream>(Constant(11)),
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

            yield return new StepCase(
                    "Update Folder with http mock",
                    new RelativityUpdateFolder
                    {
                        FolderName = Constant("NewName"),
                        FolderId   = Constant(22),
                        Workspace  = new OneOfStep<SCLInt, StringStream>(Constant(11)),
                    },
                    Unit.Default
                ).WithTestRelativitySettings()
                .WithFlurlMocks(
                    x => x.ForCallsTo(
                            "http://TestRelativityServer/Relativity.REST/api/Relativity.Services.Folder.IFolderModule/Folder%20Manager/UpdateSingleAsync"
                        )
                        .WithVerb(HttpMethod.Post)
                        .RespondWith()
                );
        }
    }
}
