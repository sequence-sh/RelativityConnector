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
            var expectedEntityString =
                "('Client': \"\" 'ClientNumber': \"\" 'DownloadHandlerUrl': \"TestURL\" 'EnableDataGrid': False 'Matter': \"\" 'MatterNumber': \"\" 'ProductionRestrictions': \"\" 'ResourcePool': \"\" 'DefaultFileRepository': \"\" 'DataGridFileRepository': \"\" 'DefaultCacheLocation': \"\" 'SqlServer': \"\" 'AzureCredentials': \"\" 'AzureFileSystemCredentials': \"\" 'SqlFullTextLanguage': \"\" 'Status': \"\" 'WorkspaceAdminGroup': \"\" 'Keywords': \"\" 'Notes': \"\" 'CreatedOn': 0001-01-01T00:00:00.0000000 'CreatedBy': \"\" 'LastModifiedBy': \"\" 'LastModifiedOn': 0001-01-01T00:00:00.0000000 'Meta': \"\" 'Actions': \"\" 'Name': \"MyNewWorkspace\" 'ArtifactID': 0 'Guids': \"\")";

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
                                Name = "MyNewWorkspace", DownloadHandlerUrl = "TestURL"
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

    public const string ResponseJson = @"{
  ""Client"": {
    ""Secured"": false,
    ""Value"": {
      ""Name"": ""Relativity"",
      ""ArtifactID"": 1015644,
      ""Guids"": []
    }
  },
  ""ClientNumber"": {
    ""Secured"": false,
    ""Value"": ""Relativity""
  },
  ""DownloadHandlerUrl"": ""Relativity.Distributed"",
  ""EnableDataGrid"": false,
  ""Matter"": {
    ""Secured"": false,
    ""Value"": {
      ""Name"": ""Salt vs. Pepper"",
      ""ArtifactID"": 1016816,
      ""Guids"": []
    }
  },
  ""MatterNumber"": {
    ""Secured"": false,
    ""Value"": ""1""
  },
  ""ProductionRestrictions"": {
    ""Secured"": false,
    ""Value"": {
      ""Name"": ""Saved Search 1"",
      ""ArtifactID"": 1117132,
      ""Guids"": []
    }
  },
  ""ResourcePool"": {
    ""Secured"": false,
    ""Value"": {
      ""Name"": ""Default"",
      ""ArtifactID"": 1015040,
      ""Guids"": []
    }
  },
  ""DefaultFileRepository"": {
    ""Secured"": false,
    ""Value"": {
      ""Name"": ""\\\\A-BC-DE-FGHIJK\\fileshare\\"",
      ""ArtifactID"": 1014887,
      ""Guids"": []
    }
  },
  ""DefaultCacheLocation"": {
    ""Secured"": false,
    ""Value"": {
      ""Name"": ""Default Cache Location"",
      ""ArtifactID"": 1015534,
      ""Guids"": []
    }
  },
  ""SqlServer"": {
    ""Secured"": false,
    ""Value"": {
      ""Name"": ""A-BC-DE-FGHIJK\\EDDSINSTANCE001"",
      ""ArtifactID"": 1015096,
      ""Guids"": []
    }
  },
  ""SqlFullTextLanguage"": {
    ""Name"": ""English"",
    ""ID"": 1033
  },
  ""Status"": {
    ""Name"": ""Active"",
    ""ArtifactID"": 675,
    ""Guids"": []
  },
  ""Keywords"": ""Sample keywords for a workspace"",
  ""Notes"": ""Sample notes for a workspace"",
  ""CreatedOn"": ""2018-04-24T15:19:57.677"",
  ""CreatedBy"": {
    ""Name"": ""Admin, Relativity"",
    ""ArtifactID"": 9,
    ""Guids"": []
  },
  ""LastModifiedBy"": {
    ""Name"": ""Admin, Relativity"",
    ""ArtifactID"": 9,
    ""Guids"": []
  },
  ""LastModifiedOn"": ""2020-01-09T15:51:39.13"",
  ""Name"": ""Sample workspace"",
  ""ArtifactID"": 1017266,
  ""Guids"": []
}";
}

}
