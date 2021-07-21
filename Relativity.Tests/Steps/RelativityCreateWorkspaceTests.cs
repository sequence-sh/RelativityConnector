using System.Collections.Generic;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityCreateWorkspaceTests : StepTestBase<RelativityCreateWorkspace, Entity>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                var expectedEntityString =
                    "(Client: (Secured: False Value: (Name: \"Relativity\" ArtifactID: 1015644 Guids: \"\")) ClientNumber: (Secured: False Value: \"Relativity\") DownloadHandlerUrl: \"Relativity.Distributed\" EnableDataGrid: False Matter: (Secured: False Value: (Name: \"Salt vs. Pepper\" ArtifactID: 1016816 Guids: \"\")) MatterNumber: (Secured: False Value: \"1\") ProductionRestrictions: (Secured: False Value: (Name: \"Saved Search 1\" ArtifactID: 1117132 Guids: \"\")) ResourcePool: (Secured: False Value: (Name: \"Default\" ArtifactID: 1015040 Guids: \"\")) DefaultFileRepository: (Secured: False Value: (Name: \"\\\\\\\\A-BC-DE-FGHIJK\\\\fileshare\\\\\" ArtifactID: 1014887 Guids: \"\")) DataGridFileRepository: \"\" DefaultCacheLocation: (Secured: False Value: (Name: \"Default Cache Location\" ArtifactID: 1015534 Guids: \"\")) SqlServer: (Secured: False Value: (Name: \"A-BC-DE-FGHIJK\\\\EDDSINSTANCE001\" ArtifactID: 1015096 Guids: \"\")) AzureCredentials: \"\" AzureFileSystemCredentials: \"\" SqlFullTextLanguage: (Name: \"English\" ID: 1033) Status: (Name: \"Active\" ArtifactID: 675 Guids: \"\") WorkspaceAdminGroup: \"\" Keywords: \"Sample keywords for a workspace\" Notes: \"Sample notes for a workspace\" CreatedOn: 2018-04-24T15:19:57.6770000 CreatedBy: (Name: \"Admin, Relativity\" ArtifactID: 9 Guids: \"\") LastModifiedBy: (Name: \"Admin, Relativity\" ArtifactID: 9 Guids: \"\") LastModifiedOn: 2020-01-09T15:51:39.1300000 Meta: \"\" Actions: \"\" Name: \"Sample workspace\" ArtifactID: 1017266 Guids: \"\")";


                var flurlClientFactory = new TestFlurlClientFactory();

                flurlClientFactory.HttpTest
                    .ForCallsTo("http://TestRelativityServer/Relativity.Rest/API/relativity-environment/v1/workspace")
                    .RespondWith(ResponseJson);

                flurlClientFactory.HttpTest.RespondWith(status: 417);
                //flurlClientFactory.HttpTest

                yield return new StepCase(
                        "Export with condition",
                        new Log<Entity>()
                        {
                            Value = new RelativityCreateWorkspace()
                            {
                                WorkspaceName = Constant("MyNewWorkspace"),
                                DefaultCacheLocationId = Constant(1),
                                DefaultFileRepositoryId = Constant(2),
                                MatterId = Constant(3),
                                ResourcePoolId = Constant(4),
                                SqlServerId = Constant(5),
                                StatusId = Constant(6),
                                TemplateId = Constant(7)
                            }
                        }, Unit.Default,
                        expectedEntityString
                    )
                    .WithRelativitySettings<RelativityCreateWorkspace, Entity, StepCase>(
                        new RelativitySettings()
                        {
                            RelativityUsername = "UN",
                            RelativityPassword = "PW",
                            Url = "http://TestRelativityServer"
                        }
                    )
                    .WithContext(
                        ConnectorInjection.FlurlClientFactoryKey,
                        flurlClientFactory
                    );
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