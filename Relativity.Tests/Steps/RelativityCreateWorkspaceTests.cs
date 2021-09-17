using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Workspace.Models;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class
    RelativityCreateWorkspaceTests : StepTestBase<RelativityCreateWorkspace, Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            var expectedEntityString = "('Name': \"MyNewWorkspace\" 'ArtifactID': 123 'Notes': \"TestNotes\" 'CreatedOn': 1970-01-01T00:00:00.0000000Z 'DownloadHandlerUrl': \"TestURL\")";

            yield return new StepCase(
                        "Export with condition",
                        new Log<Entity>()
                        {
                            Value = new RelativityCreateWorkspace()
                            {
                                WorkspaceName           = Constant("MyNewWorkspace"),
                                DefaultCacheLocationId  = Constant(1),
                                DefaultFileRepositoryId = Constant(2),
                                Matter                  = new OneOfStep<int, StringStream>(Constant(3)),
                                ResourcePoolId          = new OneOfStep<int, StringStream>(Constant(4)) ,
                                SqlServerId             = Constant(5),
                                StatusId                = Constant(6),
                                TemplateId              = new OneOfStep<int, StringStream>(Constant(7)) 
                            }
                        },
                        Unit.Default,
                        expectedEntityString
                    )
                    .WithTestRelativitySettings()
                    .WithService(
                        new MockSetup<IWorkspaceManager1, WorkspaceResponse>(
                            x => x.CreateAsync(
                                It.IsAny<WorkspaceRequest>(),
                                It.IsAny<CancellationToken>()
                            ),
                            new WorkspaceResponse()
                            {
                                Name = "MyNewWorkspace", DownloadHandlerUrl = "TestURL", ArtifactID = 123, Notes = "TestNotes", CreatedOn = DateTime.UnixEpoch
                            }
                        ),
                        new MockSetup<IWorkspaceManager1, string>(
                            x => x.GetDefaultDownloadHandlerURLAsync(),
                            "TestURL"
                        )
                    )
                ;
        }
    }
}

}
