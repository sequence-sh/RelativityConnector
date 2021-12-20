using static Reductech.Sequence.Core.TestHarness.SchemaHelpers;

namespace Reductech.Sequence.Connectors.Relativity.Tests.Integration;

internal static class TestSteps
{
    public static string TestMatterName = "Test Matter";
    public static string IntegrationTestWorkspaceName = "Integration Test Workspace";

    public static OneOfStep<SCLInt, StringStream> IntegrationTestWorkspace =
        new(Constant(IntegrationTestWorkspaceName));

    public static OneOfStep<SCLInt, StringStream> TestMatter =
        new(Constant(TestMatterName));

    public static IStep<Unit> DeleteAllTestMatter = new ForEach<Entity>
    {
        Array = new RelativitySendQuery
        {
            Condition = Constant(
                new TextCondition(
                        "Name",
                        TextConditionEnum.EqualTo,
                        TestMatterName
                    )
                    .ToQueryString()
            ),
            Workspace = new OneOfStep<SCLInt, StringStream>(Constant(-1)),
            ArtifactType =
                new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(Constant(ArtifactType.Matter))
        },
        Action = new LambdaFunction<Entity, Unit>(
            null,
            new RelativityDeleteMatter
            {
                MatterArtifactId = new EntityGetValue<SCLInt>
                {
                    Entity   = new GetAutomaticVariable<Entity>(),
                    Property = Constant("ArtifactId")
                }
            }
        )
    };

    public static IStep<Unit> DeleteAllIntegrationTestWorkspace = new ForEach<Entity>
    {
        Array = new RelativitySendQuery
        {
            Condition = Constant(
                new TextCondition(
                        "Name",
                        TextConditionEnum.EqualTo,
                        IntegrationTestWorkspaceName
                    )
                    .ToQueryString()
            ),
            Workspace = new OneOfStep<SCLInt, StringStream>(Constant(-1)),
            ArtifactType =
                new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(Constant(ArtifactType.Case))
        },
        Action = new LambdaFunction<Entity, Unit>(
            null,
            new RelativityDeleteWorkspace
            {
                Workspace = new OneOfStep<SCLInt, StringStream>(
                    new EntityGetValue<SCLInt>
                    {
                        Entity   = new GetAutomaticVariable<Entity>(),
                        Property = Constant("ArtifactId")
                    }
                )
            }
        )
    };

    public static IStep<Unit> AssertDocumentCount(int count) => new
        AssertEqual<SCLInt>
        {
            Left = Constant(count),
            Right = new EntityGetValue<SCLInt>
            {
                Entity = new RelativityRetrieveWorkspaceStatistics
                {
                    Workspace = IntegrationTestWorkspace
                },
                Property = Constant("documentCount")
            }
        };

    public static IStep<Unit> LogWorkspaceStatistics = new Log
    {
        Value = new RelativityRetrieveWorkspaceStatistics
        {
            Workspace = IntegrationTestWorkspace
        }
    };

    public static IStep<Unit> MaybeCreateTestMatter = new If<Unit>
    {
        Condition = new ArrayIsEmpty<Entity>
        {
            Array = new RelativitySendQuery
            {
                Condition = Constant(
                    new TextCondition(
                            "Name",
                            TextConditionEnum.EqualTo,
                            TestMatterName
                        )
                        .ToQueryString()
                ),
                Workspace = new OneOfStep<SCLInt, StringStream>(Constant(-1)),
                ArtifactType =
                    new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(Constant(ArtifactType.Matter))
            }
        },
        Else =
            new Log
            {
                Value = Constant("Test Matter already exists, skip creating it.")
            },
        Then = new Sequence<Unit>
        {
            InitialSteps = new List<IStep<Unit>>
            {
                new Log
                {
                    Value = Constant("Test Matter does not exist, creating it.")
                },
                new RunStep<SCLInt>
                {
                    Step = new RelativityCreateMatter
                    {
                        Client =
                            new OneOfStep<SCLInt, StringStream>(Constant("Test Client")),
                        Status =
                            new OneOfStep<SCLInt, SCLEnum<MatterStatus>>(Constant(671)),
                        MatterName = Constant(TestMatterName),
                        Number     = Constant("Ten"),
                        Keywords   = Constant("Test Keywords"),
                        Notes      = Constant("Test Notes")
                    }
                }
            }
        }
    };

    public static IStep<Unit> DeleteDocuments(string title) => new ForEach<Entity>
    {
        Array = new RelativitySendQuery
        {
            Condition = Constant(
                new TextCondition(
                        "Title",
                        TextConditionEnum.EqualTo,
                        "Test Document"
                    )
                    .ToQueryString()
            ),
            Workspace = IntegrationTestWorkspace,
            ArtifactType =
                new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(Constant(ArtifactType.Document))
        },
        Action = new LambdaFunction<Entity, Unit>(
            null,
            new RunStep<Entity>
            {
                Step = new RelativityDeleteDocument
                {
                    ObjectArtifactId = new EntityGetValue<SCLInt>
                    {
                        Entity   = new GetAutomaticVariable<Entity>(),
                        Property = Constant("ArtifactId")
                    },
                    Workspace = IntegrationTestWorkspace
                }
            }
        )
    };

    public static IStep<Unit> AssertFolderCount(int expectedCount) => new AssertEqual<SCLInt>
    {
        Left = Constant(expectedCount),
        Right = new ArrayLength<Entity>
        {
            Array = new RelativitySendQuery
            {
                Condition = Constant(""),
                Workspace = IntegrationTestWorkspace,
                ArtifactType =
                    new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(Constant(ArtifactType.Folder))
            }
        }
    };

    public static RelativityImportEntities ImportEntities(string filePath)
    {
        return
            new RelativityImportEntities
            {
                Workspace = IntegrationTestWorkspace,
                Entities = Array(
                    Entity.Create(
                        ("Control Number", "12345"),
                        ("Title", ("Test Document")),
                        ("File Path", filePath),
                        ("Folder Path", (@"TestFolder"))
                    )
                ),
                Schema = Constant(
                    new JsonSchemaBuilder()
                        .Title("Test Schema")
                        .Properties(
                            ("Control Number", AnyString),
                            ("Title", AnyString),
                            ("File Path", AnyString),
                            ("Folder Path", AnyString)
                            
                        ).Build().ConvertToEntity()
                ),
                ControlNumberField = Constant("Control Number"),
                FilePathField      = Constant("File Path"),
                FolderPathField    = Constant("Folder Path")
            };
    }

    public static IStep<Unit> MaybeCreateIntegrationTestWorkspace = new If<Unit>
    {
        Condition = new ArrayIsEmpty<Entity>
        {
            Array = new RelativitySendQuery
            {
                Condition = Constant(
                    new TextCondition(
                            "Name",
                            TextConditionEnum.EqualTo,
                            IntegrationTestWorkspaceName
                        )
                        .ToQueryString()
                ),
                Workspace = new OneOfStep<SCLInt, StringStream>(Constant(-1)),
                ArtifactType =
                    new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(Constant(ArtifactType.Case))
            }
        },
        Else =
            new Log
            {
                Value = Constant("Integration Test Workspace already exists, skip creating it.")
            },
        Then = new Sequence<Unit>
        {
            InitialSteps = new List<IStep<Unit>>
            {
                new Log
                {
                    Value = Constant("Integration Test Workspace does not exist, creating it.")
                },
                new RunStep<Entity>
                {
                    Step = new RelativityCreateWorkspace
                    {
                        WorkspaceName = Constant(IntegrationTestWorkspaceName),
                        Matter        = TestMatter,
                        TemplateId =
                            new OneOfStep<SCLInt, StringStream>(
                                Constant("Relativity Starter Template")
                            ), // = Constant(1015024),
                        StatusId = Constant(675),
                        ResourcePoolId =
                            new OneOfStep<SCLInt, StringStream>(
                                Constant("Default")
                            ), // Constant(1015040),
                        SqlServerId             = Constant(1015096),
                        DefaultFileRepositoryId = Constant(1014887),
                        DefaultCacheLocationId  = Constant(1015534)
                    }
                }
            }
        }
    };
}
