using Castle.Components.DictionaryAdapter;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

public partial class
    RelativityGetSubfoldersTests : StepTestBase<RelativityGetSubfolders, Array<Entity>>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                        "Get Subfolders",
                        new ForEach<Entity>
                        {
                            Array = new RelativityGetSubfolders
                            {
                                Workspace        = new OneOfStep<SCLInt, StringStream>(Constant(11)),
                                FolderArtifactId = Constant(22)
                            },
                            Action = new LambdaFunction<Entity, Unit>(
                                null,
                                new Log { Value = new GetAutomaticVariable<Entity>() }
                            )
                        },
                        Unit.Default,
                        @"('Name': ""SubFolder 1"" 'ArtifactID': 101 'HasChildren': False 'SystemCreatedOn': 0001-01-01T00:00:00.0000000 'SystemLastModifiedOn': 0001-01-01T00:00:00.0000000 'Selected': False)"
                       ,
                        @"('Name': ""SubFolder 2"" 'ArtifactID': 102 'HasChildren': False 'SystemCreatedOn': 0001-01-01T00:00:00.0000000 'SystemLastModifiedOn': 0001-01-01T00:00:00.0000000 'Selected': False)"
                            
                    ).WithTestRelativitySettings()
                    .WithService(
                        new MockSetup<IFolderManager1, List<Folder>>(
                            x => x.GetChildrenAsync(11, 22),
                            new EditableList<Folder>
                            {
                                new()
                                {
                                    ArtifactID = 101,
                                    Name       = "SubFolder 1",
                                    ParentFolder = new FolderRef
                                    {
                                        Name = "MyFolder", ArtifactID = 22
                                    }
                                },
                                new()
                                {
                                    ArtifactID = 102,
                                    Name       = "SubFolder 2",
                                    ParentFolder = new FolderRef
                                    {
                                        Name = "MyFolder", ArtifactID = 22
                                    }
                                },
                            }
                        )
                    )
                ;
        }
    }
}
