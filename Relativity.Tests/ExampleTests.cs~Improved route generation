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

        public const string CreateMatterSCL = @"
- <matterId> = RelativityCreateMatter
    ClientId: ((RelativityGetClients)[0]['ArtifactID'])
    StatusId: ((RelativityGetMatterStatuses)[0]['ArtifactID'])
    MatterName: 'Test Matter'
    Number: '10'
    Keywords: 'Test Keywords'
    Notes: 'Test Notes'
- log <matterId>
- RelativityDeleteMatter <matterId>
";


        //[Theory(Skip = "Manual")]
        [Theory]
        [Trait("Category", "Integration")]
        [InlineData("Log (RelativityGetClients)")]
        [InlineData(CreateMatterSCL)]
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