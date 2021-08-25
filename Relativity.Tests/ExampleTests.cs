using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Reductech.EDR.ConnectorManagement.Base;
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

namespace Reductech.EDR.Connectors.Relativity.Tests
{

[AutoTheory.UseTestOutputHelper]
public partial class ExampleTests
{
    public static IEnumerable<(string name, IStep step)> Examples
    {
        get
        {

                yield return ("Search and Tag", //Note that the 'Tags' field must exist in your workspace for this to work
                    
                          new ForEach<Entity>()
                          {
                              Array = new RelativitySendQuery()
                              {
                                  ArtifactType =
                                      new OneOfStep<ArtifactType, int>(
                                          Constant(ArtifactType.Document)
                                      ),
                                  Condition = Constant("'Title' LIKE 'Bond'"),
                                  Workspace =
                                      new OneOfStep<int, StringStream>(
                                          Constant("Integration Test Workspace")
                                      ),
                                  Start  = Constant(0),
                                  Length = Constant(100),
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
                                          Entity = new GetAutomaticVariable<Entity>(),
                                          Property = Constant("ArtifactId")
                                      },
                                      Workspace =
                                          new OneOfStep<int, StringStream>(
                                              Constant("Integration Test Workspace")
                                          ),
                                      FieldValues = Constant(Entity.Create(("Tags","My Tag")))
                                  }
                              )
                          }
                          
                          );

            yield break;


            yield return ("Import Entities",
                          new RelativityImportEntities()
                          {
                              Workspace = new OneOfStep<int, StringStream>(Constant("Integration Test Workspace")),
                              Entities = Array(
                                  Entity.Create(
                                      ("Control Number", "12345"), 
                                      ("Title", ("Test Document")), 
                                      ("File Path", (@"C:\Users\wainw\Documents\Half of my Heart.pdf")),
                                      ("Folder Path", (@"songs"))
                                      
                                      )
                                  ),
                              Schema = Constant(new Schema()
                              {
                                  Name = "Test Schema",
                                  Properties = new Dictionary<string, SchemaProperty>()
                                  {
                                      {"Control Number", new SchemaProperty(){Type = SCLType.String, Multiplicity = Multiplicity.ExactlyOne}},
                                      {"Title", new SchemaProperty(){Type = SCLType.String, Multiplicity = Multiplicity.ExactlyOne}},
                                      {"File Path", new SchemaProperty(){Type = SCLType.String, Multiplicity = Multiplicity.ExactlyOne}},
                                      {"Folder Path", new SchemaProperty(){Type = SCLType.String, Multiplicity = Multiplicity.ExactlyOne}},
                                  }.ToImmutableSortedDictionary()
                              }.ConvertToEntity()),
                              ControlNumberField = Constant("Control Number"),
                              FilePathField = Constant("File Path"),
                              FolderPathField = Constant("Folder Path")
                          }
                );

            //yield break;

            yield return ("Delete 'Integration Test Workspace'",
                          new ForEach<Entity>
                          {
                              Array = new RelativitySendQuery
                              {
                                  Condition = Constant(
                                      new TextCondition(
                                              "Name",
                                              TextConditionEnum.EqualTo,
                                              "Integration Test Workspace"
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
                          }
                );

            yield return ("Delete 'Test Matter'",
                          new ForEach<Entity>
                          {
                              Array = new RelativitySendQuery
                              {
                                  Condition = Constant(
                                      new TextCondition(
                                              "Name",
                                              TextConditionEnum.EqualTo,
                                              "Test Matter"
                                          )
                                          .ToQueryString()
                                  ),
                                  Workspace = new OneOfStep<int, StringStream>(Constant(-1)),
                                  ArtifactType =
                                      new OneOfStep<ArtifactType, int>(
                                          Constant(ArtifactType.Matter)
                                      )
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
                          }
                );

            

            yield return ("Create a folder",
                          new RelativityCreateFolder()
                          {
                              FolderName = Constant("MyNewFolder"),
                              Workspace = new OneOfStep<int, StringStream>(
                                  Constant("Integration Test Workspace")
                              )
                          }
                );

            //yield break;

            yield return ("Query Documents",
                          new ForEach<Entity>()
                          {
                              Array = new RelativitySendQuery()
                              {
                                  ArtifactType =
                                      new OneOfStep<ArtifactType, int>(
                                          Constant(ArtifactType.Document)
                                      ),
                                  Condition = Constant("'Title' LIKE 'Bond'"),
                                  Workspace =
                                      new OneOfStep<int, StringStream>(
                                          Constant("Integration Test Workspace")
                                      ),
                                  Start  = Constant(0),
                                  Length = Constant(100),
                                  Fields = new OneOfStep<Array<int>, Array<StringStream>>(
                                      Array("Title", "Control Number")
                                  )
                              },
                              Action = new LambdaFunction<Entity, Unit>(
                                  null,
                                  new Log<Entity>() { Value = new GetAutomaticVariable<Entity>() }
                              )
                          }
                );

            //yield break;

            yield return ("Create a workspace and import data",
                          new Sequence<Unit>()
                          {
                              InitialSteps = new List<IStep<Unit>>()
                              {
                                  new SetVariable<int>()
                                  {
                                      Value = new RelativityCreateMatter()
                                      {
                                          Client =
                                              new OneOfStep<int, StringStream>(Constant("Test Client")),
                                          Status =
                                              new OneOfStep<int, MatterStatus>(Constant(671)),
                                          MatterName = Constant("Test Matter"),
                                          Number     = Constant("Ten"),
                                          Keywords   = Constant("Test Keywords"),
                                          Notes      = Constant("Test Notes")
                                      },
                                      Variable = new VariableName("MatterId")
                                  },
                                  new SetVariable<Entity>()
                                  {
                                      Variable = new VariableName("Workspace"),
                                      Value = new RelativityCreateWorkspace()
                                      {
                                          WorkspaceName = Constant("Integration Test Workspace"),
                                          Matter = new OneOfStep<int, StringStream>(
                                              GetVariable<int>("MatterId")
                                          ),
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
                                  },
                                  new SetVariable<int>()
                                  {
                                      Variable = new VariableName("WorkspaceId"),
                                      Value = new EntityGetValue<int>()
                                      {
                                          Property = Constant("ArtifactID"),
                                          Entity = new GetVariable<Entity>()
                                          {
                                              Variable = new VariableName("Workspace")
                                          }
                                      }
                                  },
                                  new SetVariable<int>()
                                  {
                                      Variable = new VariableName("FolderId"),
                                      Value = new RelativityCreateFolder()
                                      {
                                          FolderName = Constant("MyFolder"),
                                          Workspace =
                                              new OneOfStep<int, StringStream>(
                                                  GetVariable<int>("WorkspaceId")
                                              ),
                                      }
                                  },
                                  //new SetVariable<Array<int>>()
                                  //{
                                  //    Variable = new VariableName("DynamicObjectIds"),
                                  //    Value = new RelativityCreateDynamicObjects()
                                  //    {
                                  //        ArtifactType = new OneOfStep<ArtifactType, int>(Constant(10)),
                                  //        Workspace =
                                  //            new OneOfStep<int, StringStream>(GetVariable<int>("WorkspaceId")),
                                  //        Entities = Array(
                                  //            Entity.Create(
                                  //                ("Title", "Thing 1"),
                                  //                ("Title", "Thing 2")
                                  //            )
                                  //        ),
                                  //        ParentArtifactId = GetVariable<int>("FolderId"),
                                  //    },
                                  //},

                                  new RelativityImport()
                                  {
                                      FilePath =
                                          Constant(
                                              @"C:\Users\wainw\source\repos\Examples\Concordance\Carla2\loadfile.dat"
                                          ),
                                      FileImportType = Constant(FileImportType.Object),
                                      SettingsFilePath =
                                          Constant(
                                              @"C:\Users\wainw\source\repos\Examples\Concordance\CarlaSettings.kwe"
                                          ),
                                      StartLineNumber = null,
                                      Workspace = new OneOfStep<int, StringStream>(
                                          GetVariable<int>("WorkspaceId")
                                      )
                                  },
                                  //new RelativityDeleteWorkspace()
                                  //{
                                  //    Workspace = new OneOfStep<int, StringStream>(
                                  //        GetVariable<int>("WorkspaceId")
                                  //    )
                                  //},
                                  //new RelativityDeleteMatter()
                                  //{
                                  //    MatterArtifactId = GetVariable<int>("MatterId"),
                                  //}
                              }
                          }
                );
        }
    }

    public static IEnumerable<object[]> IntegrationTestCaseArgs =>
        Examples.Select(x => new object[] { x.name, x.step.Serialize() });

    [Theory(Skip = "Manual")]
    //[Theory]
    //[Trait("Category", "Integration")]
    [MemberData(nameof(IntegrationTestCaseArgs))]
    public async Task RunSCLSequence(string name, string scl)
    {
        var logger =
            TestOutputHelper.BuildLogger(new LoggingConfig() { LogLevel = LogLevel.Trace });

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
                        DesktopClientPath =
                            @"C:\Program Files\kCura Corporation\Relativity Desktop Client\Relativity.Desktop.Client.exe"
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
