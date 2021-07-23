using System;
using System.Collections.Generic;
using Moq;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityRetrieveRootFolderTests : StepTestBase<RelativityRetrieveRootFolder, Entity>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                yield return new StepCase("Retrieve Root Folder",
                        new Log<Entity>()
                        {
                            Value = new RelativityRetrieveRootFolder()
                            {
                                WorkspaceArtifactId = Constant(42)
                            }
                        },
                        Unit.Default, 
                        "(ParentFolder: (ArtifactID: 0 Name: \"\") AccessControlListIsInherited: False SystemCreatedBy: \"\" SystemCreatedOn: 0001-01-01T00:00:00.0000000 SystemLastModifiedBy: \"\" SystemLastModifiedOn: 0001-01-01T00:00:00.0000000 Permissions: (add: False delete: False edit: False secure: False) Children: \"\" Selected: False HasChildren: False ArtifactID: 12345 Name: \"MyFolder\")"

                    ).WithTestRelativitySettings()
                    .WithService(new Action<Mock<IFolderManager>>(
                        m => m.Setup(x => x.GetWorkspaceRootAsync(42))
                            .ReturnsAsync(new Folder()
                            {
                                Name = "MyFolder",
                                ArtifactID = 12345
                            })
                    ));
            }
        }
    }
}