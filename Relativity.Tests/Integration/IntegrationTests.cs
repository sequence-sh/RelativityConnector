using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Reductech.EDR.ConnectorManagement.Base;
using Reductech.EDR.Connectors.Relativity;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Abstractions;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Serialization;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Services;
using Xunit;
using Xunit.Abstractions;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

internal static class TestSteps
{
    public static string TestMatterName = "Test Matter";
    public static string IntegrationTestWorkspaceName = "Integration Test Workspace";

    public static OneOfStep<int, StringStream> IntegrationTestWorkspace =
        new(Constant(IntegrationTestWorkspaceName));

    public static OneOfStep<int, StringStream> TestMatter =
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
            Workspace = new OneOfStep<int, StringStream>(Constant(-1)),
            ArtifactType =
                new OneOfStep<ArtifactType, int>(Constant(ArtifactType.Matter))
        },
        Action = new LambdaFunction<Entity, Unit>(
            null,
            new RelativityDeleteMatter
            {
                MatterArtifactId = new EntityGetValue<int>
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
            Workspace = new OneOfStep<int, StringStream>(Constant(-1)),
            ArtifactType =
                new OneOfStep<ArtifactType, int>(Constant(ArtifactType.Case))
        },
        Action = new LambdaFunction<Entity, Unit>(
            null,
            new RelativityDeleteWorkspace()
            {
                Workspace = new OneOfStep<int, StringStream>(
                    new EntityGetValue<int>()
                    {
                        Entity   = new GetAutomaticVariable<Entity>(),
                        Property = Constant("ArtifactId")
                    }
                )
            }
        )
    };

    public static IStep<Unit> AssertDocumentCount(int count) => new
        AssertEqual<int>()
        {
            Left = Constant(count),
            Right = new EntityGetValue<int>()
            {
                Entity = new RelativityRetrieveWorkspaceStatistics()
                {
                    Workspace = IntegrationTestWorkspace
                },
                Property = Constant("documentCount")
            }
        };

    public static IStep<Unit> LogWorkspaceStatistics = new Log<Entity>()
    {
        Value = new RelativityRetrieveWorkspaceStatistics()
        {
            Workspace = IntegrationTestWorkspace
        }
    };

    public static IStep<Unit> MaybeCreateTestMatter = new If()
    {
        Condition = new ArrayIsEmpty<Entity>()
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
                Workspace = new OneOfStep<int, StringStream>(Constant(-1)),
                ArtifactType =
                    new OneOfStep<ArtifactType, int>(Constant(ArtifactType.Matter))
            }
        },
        Else =
            new Log<StringStream>()
            {
                Value = Constant("Test Matter already exists, skip creating it.")
            },
        Then = new Sequence<Unit>()
        {
            InitialSteps = new List<IStep<Unit>>()
            {
                new Log<StringStream>()
                {
                    Value = Constant("Test Matter does not exist, creating it.")
                },
                new RunStep<int>()
                {
                    Step = new RelativityCreateMatter()
                    {
                        Client =
                            new OneOfStep<int, StringStream>(Constant("Test Client")),
                        Status =
                            new OneOfStep<int, MatterStatus>(Constant(671)),
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
                new OneOfStep<ArtifactType, int>(Constant(ArtifactType.Document))
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
                        Property = Constant("ArtifactId")
                    },
                    Workspace = IntegrationTestWorkspace
                }
            }
        )
    };

    public static IStep<Unit> AssertFolderCount(int expectedCount) => new AssertEqual<int>()
    {
        Left = Constant(expectedCount),
        Right = new ArrayLength<Entity>()
        {
            Array = new RelativitySendQuery()
            {
                Condition = Constant(""),
                Workspace = IntegrationTestWorkspace,
                ArtifactType =
                    new OneOfStep<ArtifactType, int>(Constant(ArtifactType.Folder))
            }
        }
    };

    public static IStep<Unit> ImportEntities(string filePath)
    {
        return
            new RelativityImportEntities
            {
                Workspace = TestSteps.IntegrationTestWorkspace,
                Entities = Array(
                    Entity.Create(
                        ("Control Number", "12345"),
                        ("Title", ("Test Document")),
                        ("File Path", filePath),
                        ("Folder Path", (@"TestFolder"))
                    )
                ),
                Schema = Constant(
                    new Schema()
                    {
                        Name = "Test Schema",
                        Properties = new Dictionary<string, SchemaProperty>()
                        {
                            {
                                "Control Number",
                                new SchemaProperty()
                                {
                                    Type         = SCLType.String,
                                    Multiplicity = Multiplicity.ExactlyOne
                                }
                            },
                            {
                                "Title",
                                new SchemaProperty()
                                {
                                    Type         = SCLType.String,
                                    Multiplicity = Multiplicity.ExactlyOne
                                }
                            },
                            {
                                "File Path",
                                new SchemaProperty()
                                {
                                    Type         = SCLType.String,
                                    Multiplicity = Multiplicity.ExactlyOne
                                }
                            },
                            {
                                "Folder Path",
                                new SchemaProperty()
                                {
                                    Type         = SCLType.String,
                                    Multiplicity = Multiplicity.ExactlyOne
                                }
                            },
                        }.ToImmutableSortedDictionary()
                    }.ConvertToEntity()
                ),
                ControlNumberField = Constant("Control Number"),
                FilePathField      = Constant("File Path"),
                FolderPathField    = Constant("Folder Path")
            };
    }

    public static IStep<Unit> MaybeCreateIntegrationTestWorkspace = new If()
    {
        Condition = new ArrayIsEmpty<Entity>()
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
                Workspace = new OneOfStep<int, StringStream>(Constant(-1)),
                ArtifactType =
                    new OneOfStep<ArtifactType, int>(Constant(ArtifactType.Case))
            }
        },
        Else =
            new Log<StringStream>()
            {
                Value = Constant("Integration Test Workspace already exists, skip creating it.")
            },
        Then = new Sequence<Unit>()
        {
            InitialSteps = new List<IStep<Unit>>()
            {
                new Log<StringStream>()
                {
                    Value = Constant("Integration Test Workspace does not exist, creating it.")
                },
                new RunStep<Entity>()
                {
                    Step = new RelativityCreateWorkspace()
                    {
                        WorkspaceName = Constant(IntegrationTestWorkspaceName),
                        Matter        = TestMatter,
                        TemplateId =
                            new OneOfStep<int, StringStream>(
                                Constant("Relativity Starter Template")
                            ), // = Constant(1015024),
                        StatusId = Constant(675),
                        ResourcePoolId =
                            new OneOfStep<int, StringStream>(
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

namespace Reductech.EDR.Connectors.Relativity.Tests.Integration
{
    
public class IntegrationTests
{

    public IntegrationTests(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
    }
    public ITestOutputHelper TestOutputHelper { get; set; }

        public const string SkipAll = "";

    //[Fact(Skip = SkipAll)]
    //[Trait("Category", "Integration")]
    //public async void Test()
    //{

    //    await TestSCLSequence(step);
    //}

    [Fact(Skip = "Manual")]
    public async void TestDeleteAllIntegrationTestWorkspace()
    {
        await TestSCLSequence(TestSteps.DeleteAllIntegrationTestWorkspace);
    }

    [Fact(Skip = "Manual")]
    public async void TestDeleteAllTestMatter()
    {
        await TestSCLSequence(TestSteps.DeleteAllTestMatter);
    }

    [Fact(Skip = "Manual")]
    //[Fact]
    public async void TestImportEntities()
    {
        string filePath = Path.Combine(
            Assembly.GetAssembly(typeof(IntegrationTests))!.Location,
            "..",
            "Data",
            "TestDocument.pdf"
        );

        await TestSCLSequence(TestSteps.ImportEntities(filePath));
    }

    //[Fact(Skip = "Manual")]
    [Fact]
    public async void TestImportConcordance()
    {
        var step = new RelativityImport()
        {
            Workspace = TestSteps.IntegrationTestWorkspace,
            FilePath =
                Constant(
                    "D:\\Mark\\SampleData\\Concordance\\Carla2\\loadfile.dat"
                ),
            SettingsFilePath =
                Constant(
                    "D:\\Mark\\SampleData\\Concordance\\CarlaSettings.kwe"
                ),
            FileImportType = Constant(FileImportType.Object)
        };

        await TestSCLSequence(step);
    }

    //[Fact(Skip = "Manual")]
    [Fact]
    public async void TestExportEntities()
    {
        var exportStep = new RelativityExport()
        {
            Workspace  = TestSteps.IntegrationTestWorkspace,
            Condition  = Constant("'Title' LIKE 'Bond'"),
            FieldNames = Array("Title")
        };

        var step = new ForEach<Entity>()
        {
            Array = exportStep,
            Action = new LambdaFunction<Entity, Unit>(
                null,
                new Log<Entity>() { Value = GetEntityVariable }
            )
        };

        await TestSCLSequence(step);
    }

    [Fact(Skip = SkipAll)]
    [Trait("Category", "Integration")]
    public async void ShortIntegrationTest()
    {
        string filePath = Path.Combine(
            Assembly.GetAssembly(typeof(IntegrationTests))!.Location,
            "..",
            "Data",
            "TestDocument.pdf"
        );

        var step = new Sequence<Unit>()
        {
            InitialSteps = new List<IStep<Unit>>()
            {
                TestSteps.DeleteAllIntegrationTestWorkspace,
                TestSteps.MaybeCreateTestMatter,
                TestSteps.MaybeCreateIntegrationTestWorkspace,
                TestSteps.AssertDocumentCount(0),
                //TestSteps.AssertFolderCount(0),

                new RunStep<int>
                {
                    Step = new RelativityCreateFolder
                    {
                        FolderName = Constant("MyFolder"),
                        Workspace  = TestSteps.IntegrationTestWorkspace,
                    }
                },
                //TestSteps.AssertFolderCount(1),

                TestSteps.ImportEntities(filePath),
                TestSteps.AssertFolderCount(2),
                TestSteps.AssertDocumentCount(1),
                TestSteps.LogWorkspaceStatistics,
                TestSteps.DeleteDocuments("Test Document"),
                TestSteps.AssertDocumentCount(0),
                new RelativityDeleteUnusedFolders()
                {
                    Workspace = TestSteps.IntegrationTestWorkspace
                },
                //TestSteps.AssertFolderCount(0),
                TestSteps.DeleteAllIntegrationTestWorkspace,
            }
        };

        await TestSCLSequence(step);
    }

    [Fact(Skip = "Manual")]
    public async void TestSearchAndTag()
    {
        var step = new ForEach<Entity>()
        {
            Array = new RelativitySendQuery()
            {
                ArtifactType =
                    new OneOfStep<ArtifactType, int>(Constant(ArtifactType.Document)),
                Condition = Constant("'Title' LIKE 'Bond'"),
                Workspace = TestSteps.IntegrationTestWorkspace,
                Start     = Constant(0),
                Length    = Constant(100),
                Fields = new OneOfStep<Array<int>, Array<StringStream>>(
                    Array("Title", "Control Number")
                )
            },
            Action = new LambdaFunction<Entity, Unit>(
                null,
                new RelativityUpdateObject()
                {
                    ObjectArtifactId = new EntityGetValue<int>()
                    {
                        Entity   = new GetAutomaticVariable<Entity>(),
                        Property = Constant("ArtifactId")
                    },
                    Workspace   = TestSteps.IntegrationTestWorkspace,
                    FieldValues = Constant(Entity.Create(("Tags", "My Tag")))
                }
            )
        };

        await TestSCLSequence(step);
    }

    private async Task TestSCLSequence(IStep step)
    {
        var logger =
            TestOutputHelper.BuildLogger(new LoggingConfig() { LogLevel = LogLevel.Trace });

        var scl = step.Serialize();
        logger.LogInformation(scl);

        var sfs = StepFactoryStore.Create(
            new ConnectorData(
                new ConnectorSettings()
                {
                    Id      = "Reductech.EDR.Connectors.Relativity",
                    Enable  = true,
                    Version = "0.10.0",
                    Settings = new RelativitySettings
                    {
                        RelativityUsername = "relativity.admin@relativity.com",
                        //RelativityUsername = "mark@reduc.tech",
                        RelativityPassword = "Test1234!",
                        Url                = "http://relativitydevvm/",
                        APIVersionNumber   = 1,
                        ImportClientPath =
                            "C:\\Users\\wainw\\source\\repos\\Reductech\\entityimportclient\\EntityImportClient\\bin\\Debug\\EntityImportClient.exe",
                        DesktopClientPath =
                            @"D:\Shares\Software\RelativityDesktopClient\Relativity.Desktop.Client.exe"
                            //@"C:\Program Files\kCura Corporation\Relativity Desktop Client\Relativity.Desktop.Client.exe"
                    }.ToDictionary()
                },
                typeof(RelativityGetClients).Assembly
            )
        );

        var runner = new SCLRunner(
            logger,
            sfs,
            ExternalContext.Default with
            {
                InjectedContexts = new ConnectorInjection().TryGetInjectedContexts()
                    .Value.ToArray()
            }
        );

        var r = await runner.RunSequenceFromTextAsync(
            scl,
            new Dictionary<string, object>(),
            CancellationToken.None
        );

        r.ShouldBeSuccessful();
    }
}

}
