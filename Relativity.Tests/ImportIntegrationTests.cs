//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Divergic.Logging.Xunit;
//using Microsoft.Extensions.Logging;
//using Reductech.EDR.ConnectorManagement.Base;
//using Reductech.EDR.Connectors.Relativity.Steps;
//using Reductech.EDR.Core.Abstractions;
//using Reductech.EDR.Core.Internal;
//using Reductech.EDR.Core.Internal.Serialization;
//using Reductech.EDR.Core.TestHarness;
//using Xunit;
//using Xunit.Abstractions;
//using static Reductech.EDR.Core.TestHarness.StaticHelpers;

//namespace Reductech.EDR.Connectors.Relativity.Tests
//{
    

//    [AutoTheory.UseTestOutputHelper]
//    public partial class ImportIntegrationTestCases 
//    {
//        [Fact]
//        [Trait("Category", "Integration")]
//        public async Task RunSCLSequence()
//        {
//            var logger =
//                TestOutputHelper.BuildLogger(new LoggingConfig() { LogLevel = LogLevel.Information });

//            var step = ;

//            var scl = step.Serialize();

//            logger.LogInformation(scl);

//            var sfs = StepFactoryStore.Create(new ConnectorData(
//                new ConnectorSettings()
//                {
//                    Id = "Reductech.EDR.Connectors.Relativity",
//                    Enable = true,
//                    Version = "0.10.0",
//                    Settings = new RelativitySettings()
//                    {
//                        RelativityUsername = "relativity.admin@relativity.com", //TODO maybe change this
//                        RelativityPassword = "Test1234!",
//                        Url = "http://relativitydevvm/",
//                        APIVersionNumber = 1,
//                        DesktopClientPath = @"C:\Program Files\kCura Corporation\Relativity Desktop Client\Relativity.Desktop.Client.exe"
//                    }.ToDictionary()
//                },
//                typeof(RelativityGetClients).Assembly
//            ));

//            var runner = new SCLRunner(
//                logger,
//                sfs,
//                ExternalContext.Default with{ InjectedContexts = new ConnectorInjection().TryGetInjectedContexts().Value.ToArray()}
//            );

//            var r = await runner.RunSequenceFromTextAsync(
//                scl,
//                new Dictionary<string, object>(),
//                CancellationToken.None
//            );

//            r.ShouldBeSuccessful();
//        }
//    }

//}
