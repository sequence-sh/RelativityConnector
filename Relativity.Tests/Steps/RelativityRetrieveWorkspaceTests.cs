using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.Steps;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Shared.V1.Models;
using Action = Relativity.Shared.V1.Models.Action;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

public partial class
    RelativityRetrieveWorkspaceTests : StepTestBase<RelativityRetrieveWorkspace, Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Retrieve Workspace",
                    new Log<Entity>()
                    {
                        Value = new RelativityRetrieveWorkspace()
                        {
                            Workspace       = new OneOfStep<int, StringStream>(Constant(11)),
                            IncludeActions  = Constant(true),
                            IncludeMetadata = Constant(true),
                        }
                    },
                    Unit.Default,
                    @"('Name': ""My Workspace"" 'ArtifactID': 11 'Notes': ""My Notes"" 'CreatedOn': 0001-01-01T00:00:00.0000000 'DownloadHandlerUrl': null)"
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IWorkspaceManager1, WorkspaceResponse>(
                        a => a.ReadAsync(11, true, true),
                        new WorkspaceResponse()
                        {
                            ArtifactID = 11,
                            Name       = "My Workspace",
                            Notes      = "My Notes",
                            Actions = new List<Action>()
                            {
                                new()
                                {
                                    Name = "MyAction", IsAvailable = true, Verb = "Post"
                                }
                            },
                            Meta = new Meta()
                            {
                                ReadOnly = new List<string>() { "Meta", "Data" }
                            }
                        }
                    )
                );
        }
    }
}
