using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

public partial class
    RelativityRetrieveRootFolderTests : StepTestBase<RelativityRetrieveRootFolder, Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Retrieve Root Folder",
                    new Log<Entity>()
                    {
                        Value = new RelativityRetrieveRootFolder()
                        {
                            Workspace = new OneOfStep<int, StringStream>(Constant(42)),
                        }
                    },
                    Unit.Default,
                    @"('Name': ""MyFolder"" 'ArtifactID': 12345 'HasChildren': False 'SystemCreatedOn': 0001-01-01T00:00:00.0000000 'SystemLastModifiedOn': 0001-01-01T00:00:00.0000000 'Selected': False)"
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IFolderManager1, Folder>(
                        x => x.GetWorkspaceRootAsync(42),
                        new Folder() { Name = "MyFolder", ArtifactID = 12345 }
                    )
                );
        }
    }
}
