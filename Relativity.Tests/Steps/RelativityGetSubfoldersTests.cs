using System;
using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using Moq;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityGetSubfoldersTests : StepTestBase<RelativityGetSubfolders, Array<Entity>>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                yield return new StepCase("Get Subfolders",
                        new ForEach<Entity>()
                        {
                            Array = new RelativityGetSubfolders()
                            {
                                WorkspaceArtifactId = Constant(11),
                                FolderArtifactId = Constant(22)
                            },
                            Action = new LambdaFunction<Entity, Unit>(null,
                                new Log<Entity>() { Value = new GetAutomaticVariable<Entity>() })
                        },
                        Unit.Default,
                        "(ParentFolder: (ArtifactID: 22 Name: \"MyFolder\") AccessControlListIsInherited: False SystemCreatedBy: \"\" SystemCreatedOn: 0001-01-01T00:00:00.0000000 SystemLastModifiedBy: \"\" SystemLastModifiedOn: 0001-01-01T00:00:00.0000000 Permissions: (add: False delete: False edit: False secure: False) Children: \"\" Selected: False HasChildren: False ArtifactID: 101 Name: \"SubFolder 1\")",
                        "(ParentFolder: (ArtifactID: 22 Name: \"MyFolder\") AccessControlListIsInherited: False SystemCreatedBy: \"\" SystemCreatedOn: 0001-01-01T00:00:00.0000000 SystemLastModifiedBy: \"\" SystemLastModifiedOn: 0001-01-01T00:00:00.0000000 Permissions: (add: False delete: False edit: False secure: False) Children: \"\" Selected: False HasChildren: False ArtifactID: 102 Name: \"SubFolder 2\")"
                    ).WithTestRelativitySettings()
                    .WithService(new Action<Mock<IFolderManager>>(m =>
                        m.Setup(x => x.GetChildrenAsync(11, 22))
                            .ReturnsAsync(new EditableList<Folder>()
                            {
                                new() { ArtifactID = 101, Name = "SubFolder 1", ParentFolder = new FolderRef(){Name = "MyFolder", ArtifactID = 22} },
                                new() { ArtifactID = 102, Name = "SubFolder 2", ParentFolder =new FolderRef(){Name = "MyFolder", ArtifactID = 22} },
                            })
                    ));
            }
        }
    }
}