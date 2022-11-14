using Moq;
using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Folder;

namespace Sequence.Connectors.Relativity.Tests.Steps;

public partial class RelativityCreateFolderTests : StepTestBase<RelativityCreateFolder, SCLInt>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Create a folder with a parent folder",
                    new RelativityCreateFolder
                    {
                        FolderName     = Constant("MyNewFolder"),
                        ParentFolderId = Constant(14),
                        Workspace      = new OneOfStep<SCLInt, StringStream>(Constant(13))
                    },
                    42.ConvertToSCLObject()
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IFolderManager1, int>(
                        x => x.CreateSingleAsync(
                            13,
                            It.Is<Folder>(
                                folder => folder.Name == "MyNewFolder"
                                       && folder.ParentFolder.ArtifactID == 14
                            )
                        ),
                        42
                    )
                );
        }
    }
}
