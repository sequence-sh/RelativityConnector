using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Reductech.EDR.ConnectorManagement.Base;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.Abstractions;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Serialization;
using Reductech.EDR.Core.TestHarness;
using Xunit;
using Xunit.Abstractions;

namespace Reductech.EDR.Connectors.Relativity.Tests
{
    [AutoTheory.UseTestOutputHelper]
    public partial class ExampleTests
    {

        public const string CreateMatterSCL = @"- <matterId> = RelativityCreateMatter
    ClientId: 1018410
    StatusId: ((RelativityGetMatterStatuses)[0]['ArtifactID'])
    MatterName: 'Test Matter'
    Number: '10'
    Keywords: 'Test Keywords'
    Notes: 'Test Notes'
- log <matterId>";

        public const string CreateWorkspaceSCL = @"
- <matterId> = RelativityCreateMatter
    ClientId: ((RelativityGetClients)[0]['ArtifactID'])
    StatusId: ((RelativityGetMatterStatuses)[0]['ArtifactID'])
    MatterName: 'Test Matter'
    Number: '10'
    Keywords: 'Test Keywords'
    Notes: 'Test Notes'

- <workspaceId> = RelativityCreateWorkspace
    WorkspaceName: 'Test Workspace'
    MatterId: <matterId>
    StatusId: ((RelativityGetMatterStatuses)[0]['ArtifactID'])

- log <matterId>
- RelativityDeleteMatter <matterId>
- RelativityDeleteWorkspace <workspaceId>
";

        public const string CreateRDOsSCL = @"
RelativityCreateDynamicObjects
    WorkspaceArtifactId: 1003663
    Entities: [(Name: 'My Entity')]
    ArtifactTypeId: 10

";


        //[Theory(Skip = "Manual")]
        [Theory]
        [Trait("Category", "Integration")]
        //[InlineData("Log (RelativityGetClients)")]
        //[InlineData(CreateMatterSCL)]
        //[InlineData("RelativityDeleteWorkspace 1003663")]
        [InlineData(CreateRDOsSCL)]
        public async Task RunSCLSequence(string scl)
        {
            var logger =
                TestOutputHelper.BuildLogger(new LoggingConfig() { LogLevel = LogLevel.Information });

            var sfs = StepFactoryStore.Create(new ConnectorData(
                new ConnectorSettings()
                {
                    Id = "Reductech.EDR.Connectors.Relativity",
                    Enable = true,
                    Version = "0.10.0",
                    Settings = new RelativitySettings()
                    {
                        RelativityUsername = "Mark@reduc.tech", //TODO maybe change this
                        RelativityPassword = "Test1234!",
                        Url = "http://relativitydevvm/",
                        APIVersionNumber = 1
                    }.ToDictionary()
                },
                typeof(RelativityGetClients).Assembly
            ));

            var runner = new SCLRunner(
                logger,
                sfs,
                ExternalContext.Default with{ InjectedContexts = new ConnectorInjection().TryGetInjectedContexts().Value.ToArray()}
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