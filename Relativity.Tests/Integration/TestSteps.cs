using static Reductech.EDR.Core.TestHarness.SchemaHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests.Integration;

internal static class TestSteps
{
    public static string TestMatterName = "Test Matter";
    public static string IntegrationTestWorkspaceName = "Integration Test Workspace";

    public static OneOfStep<int, StringStream> IntegrationTestWorkspace =
        new(StaticHelpers.Constant(IntegrationTestWorkspaceName));

    public static OneOfStep<int, StringStream> TestMatter =
        new(StaticHelpers.Constant(TestMatterName));

    public static IStep<Unit> DeleteAllTestMatter = new ForEach<Entity>
    {
        Array = new RelativitySendQuery
        {
            Condition = StaticHelpers.Constant(
                new TextCondition(
                        "Name",
                        TextConditionEnum.EqualTo,
                        TestMatterName
                    )
                    .ToQueryString()
            ),
            Workspace = new OneOfStep<int, StringStream>(StaticHelpers.Constant(-1)),
            ArtifactType =
                new OneOfStep<ArtifactType, int>(StaticHelpers.Constant(ArtifactType.Matter))
        },
        Action = new LambdaFunction<Entity, Unit>(
            null,
            new RelativityDeleteMatter
            {
                MatterArtifactId = new EntityGetValue<int>
                {
                    Entity   = new GetAutomaticVariable<Entity>(),
                    Property = StaticHelpers.Constant("ArtifactId")
                }
            }
        )
    };

    public static IStep<Unit> DeleteAllIntegrationTestWorkspace = new ForEach<Entity>
    {
        Array = new RelativitySendQuery
        {
            Condition = StaticHelpers.Constant(
                new TextCondition(
                        "Name",
                        TextConditionEnum.EqualTo,
                        IntegrationTestWorkspaceName
                    )
                    .ToQueryString()
            ),
            Workspace = new OneOfStep<int, StringStream>(StaticHelpers.Constant(-1)),
            ArtifactType =
                new OneOfStep<ArtifactType, int>(StaticHelpers.Constant(ArtifactType.Case))
        },
        Action = new LambdaFunction<Entity, Unit>(
            null,
            new RelativityDeleteWorkspace()
            {
                Workspace = new OneOfStep<int, StringStream>(
                    new EntityGetValue<int>()
                    {
                        Entity   = new GetAutomaticVariable<Entity>(),
                        Property = StaticHelpers.Constant("ArtifactId")
                    }
                )
            }
        )
    };

    public static IStep<Unit> AssertDocumentCount(int count) => new
        AssertEqual<int>()
        {
            Left = StaticHelpers.Constant(count),
            Right = new EntityGetValue<int>()
            {
                Entity = new RelativityRetrieveWorkspaceStatistics()
                {
                    Workspace = IntegrationTestWorkspace
                },
                Property = StaticHelpers.Constant("documentCount")
            }
        };

    public static IStep<Unit> LogWorkspaceStatistics = new Log<Entity>()
    {
        Value = new RelativityRetrieveWorkspaceStatistics()
        {
            Workspace = IntegrationTestWorkspace
        }
    };

    public static IStep<Unit> MaybeCreateTestMatter = new If<Unit>()
    {
        Condition = new ArrayIsEmpty<Entity>()
        {
            Array = new RelativitySendQuery
            {
                Condition = StaticHelpers.Constant(
                    new TextCondition(
                            "Name",
                            TextConditionEnum.EqualTo,
                            TestMatterName
                        )
                        .ToQueryString()
                ),
                Workspace = new OneOfStep<int, StringStream>(StaticHelpers.Constant(-1)),
                ArtifactType =
                    new OneOfStep<ArtifactType, int>(StaticHelpers.Constant(ArtifactType.Matter))
            }
        },
        Else =
            new Log<StringStream>()
            {
                Value = StaticHelpers.Constant("Test Matter already exists, skip creating it.")
            },
        Then = new Sequence<Unit>()
        {
            InitialSteps = new List<IStep<Unit>>()
            {
                new Log<StringStream>()
                {
                    Value = StaticHelpers.Constant("Test Matter does not exist, creating it.")
                },
                new RunStep<int>()
                {
                    Step = new RelativityCreateMatter()
                    {
                        Client =
                            new OneOfStep<int, StringStream>(StaticHelpers.Constant("Test Client")),
                        Status =
                            new OneOfStep<int, MatterStatus>(StaticHelpers.Constant(671)),
                        MatterName = StaticHelpers.Constant(TestMatterName),
                        Number     = StaticHelpers.Constant("Ten"),
                        Keywords   = StaticHelpers.Constant("Test Keywords"),
                        Notes      = StaticHelpers.Constant("Test Notes")
                    }
                }
            }
        }
    };

    public static IStep<Unit> DeleteDocuments(string title) => new ForEach<Entity>
    {
        Array = new RelativitySendQuery
        {
            Condition = StaticHelpers.Constant(
                new TextCondition(
                        "Title",
                        TextConditionEnum.EqualTo,
                        "Test Document"
                    )
                    .ToQueryString()
            ),
            Workspace = IntegrationTestWorkspace,
            ArtifactType =
                new OneOfStep<ArtifactType, int>(StaticHelpers.Constant(ArtifactType.Document))
        },
        Action = new LambdaFunction<Entity, Unit>(
            null,
            new RunStep<Entity>()
            {
                Step = new RelativityDeleteDocument()
                {
                    ObjectArtifactId = new EntityGetValue<int>()
                    {
                        Entity   = new GetAutomaticVariable<Entity>(),
                        Property = StaticHelpers.Constant("ArtifactId")
                    },
                    Workspace = IntegrationTestWorkspace
                }
            }
        )
    };

    public static IStep<Unit> AssertFolderCount(int expectedCount) => new AssertEqual<int>()
    {
        Left = StaticHelpers.Constant(expectedCount),
        Right = new ArrayLength<Entity>()
        {
            Array = new RelativitySendQuery()
            {
                Condition = StaticHelpers.Constant(""),
                Workspace = IntegrationTestWorkspace,
                ArtifactType =
                    new OneOfStep<ArtifactType, int>(StaticHelpers.Constant(ArtifactType.Folder))
            }
        }
    };

    public static RelativityImportEntities ImportEntities(string filePath)
    {
        return
            new RelativityImportEntities
            {
                Workspace = TestSteps.IntegrationTestWorkspace,
                Entities = StaticHelpers.Array(
                    Entity.Create(
                        ("Control Number", "12345"),
                        ("Title", ("Test Document")),
                        ("File Path", filePath),
                        ("Folder Path", (@"TestFolder"))
                    )
                ),
                Schema = StaticHelpers.Constant(
                    new JsonSchemaBuilder()
                        .Title("Test Schema")
                        .Properties(
                            ("Control Number", AnyString),
                            ("Title", AnyString),
                            ("File Path", AnyString),
                            ("Folder Path", AnyString)
                            
                        ).Build().ConvertToEntity()
                ),
                ControlNumberField = StaticHelpers.Constant("Control Number"),
                FilePathField      = StaticHelpers.Constant("File Path"),
                FolderPathField    = StaticHelpers.Constant("Folder Path")
            };
    }

    public static IStep<Unit> MaybeCreateIntegrationTestWorkspace = new If<Unit>()
    {
        Condition = new ArrayIsEmpty<Entity>()
        {
            Array = new RelativitySendQuery
            {
                Condition = StaticHelpers.Constant(
                    new TextCondition(
                            "Name",
                            TextConditionEnum.EqualTo,
                            IntegrationTestWorkspaceName
                        )
                        .ToQueryString()
                ),
                Workspace = new OneOfStep<int, StringStream>(StaticHelpers.Constant(-1)),
                ArtifactType =
                    new OneOfStep<ArtifactType, int>(StaticHelpers.Constant(ArtifactType.Case))
            }
        },
        Else =
            new Log<StringStream>()
            {
                Value = StaticHelpers.Constant("Integration Test Workspace already exists, skip creating it.")
            },
        Then = new Sequence<Unit>()
        {
            InitialSteps = new List<IStep<Unit>>()
            {
                new Log<StringStream>()
                {
                    Value = StaticHelpers.Constant("Integration Test Workspace does not exist, creating it.")
                },
                new RunStep<Entity>()
                {
                    Step = new RelativityCreateWorkspace()
                    {
                        WorkspaceName = StaticHelpers.Constant(IntegrationTestWorkspaceName),
                        Matter        = TestMatter,
                        TemplateId =
                            new OneOfStep<int, StringStream>(
                                StaticHelpers.Constant("Relativity Starter Template")
                            ), // = Constant(1015024),
                        StatusId = StaticHelpers.Constant(675),
                        ResourcePoolId =
                            new OneOfStep<int, StringStream>(
                                StaticHelpers.Constant("Default")
                            ), // Constant(1015040),
                        SqlServerId             = StaticHelpers.Constant(1015096),
                        DefaultFileRepositoryId = StaticHelpers.Constant(1014887),
                        DefaultCacheLocationId  = StaticHelpers.Constant(1015534)
                    }
                }
            }
        }
    };
}
