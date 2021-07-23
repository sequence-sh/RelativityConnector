using System;
using System.Collections.Generic;
using Moq;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Workspace;
using Relativity.Environment.V1.Workspace.Models;
using Relativity.Shared.V1.Models;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;
using Action = Relativity.Shared.V1.Models.Action;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityRetrieveWorkspaceTests : StepTestBase<RelativityRetrieveWorkspace, Entity>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {

                yield return new StepCase("Retrieve Workspace",
                    new Log<Entity>()
                    {
                        Value = new RelativityRetrieveWorkspace()
                        {
                            WorkspaceId = Constant(11),
                            IncludeActions = Constant(true),
                            IncludeMetadata = Constant(true),
                        }
                    },
                    Unit.Default,
                    "(Client: \"\" ClientNumber: \"\" DownloadHandlerUrl: \"\" EnableDataGrid: False Matter: \"\" MatterNumber: \"\" ProductionRestrictions: \"\" ResourcePool: \"\" DefaultFileRepository: \"\" DataGridFileRepository: \"\" DefaultCacheLocation: \"\" SqlServer: \"\" AzureCredentials: \"\" AzureFileSystemCredentials: \"\" SqlFullTextLanguage: \"\" Status: \"\" WorkspaceAdminGroup: \"\" Keywords: \"\" Notes: \"\" CreatedOn: 0001-01-01T00:00:00.0000000 CreatedBy: \"\" LastModifiedBy: \"\" LastModifiedOn: 0001-01-01T00:00:00.0000000 Meta: (Unsupported: \"\" ReadOnly: [\"Meta\", \"Data\"]) Actions: [(Name: \"MyAction\" Href: \"\" Verb: \"Post\" IsAvailable: True Reason: \"\")] Name: \"\" ArtifactID: 11 Guids: \"\")"


                ).WithTestRelativitySettings().WithService(new Action<Mock<IWorkspaceManager>>(
                    m=>m.Setup(a=>a.ReadAsync(11,true,true))
                        .ReturnsAsync(new WorkspaceResponse()
                        {
                            ArtifactID = 11,Actions = new List<Action>()
                            {
                                new (){Name = "MyAction", IsAvailable = true, Verb = "Post"}
                            },
                            Meta = new Meta()
                            {
                                ReadOnly = new List<string>()
                                {
                                    "Meta", "Data"
                                }
                            }
                        })
                    
                    
                    ));
            }
        }
    }
}