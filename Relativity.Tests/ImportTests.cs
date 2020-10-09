using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Xunit;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.Utilities.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Reductech.Connectors.Relativity.Tests
{

    public class ImportTests : ImportTestCases
    {
        public ImportTests(ITestOutputHelper testOutputHelper) => TestOutputHelper = testOutputHelper;

        /// <inheritdoc />
        [Theory]
        [ClassData(typeof(ImportTestCases))]
        public override void Test(string key)
        {
            base.Test(key);
        }
    }


    public class ImportTestCases : TestBase
    {
        /// <inheritdoc />
        protected override IEnumerable<ITestBaseCase> TestCases {
            get
            {
                yield return  new TestCase();
            }
        }


        public class TestCase : ITestBaseCase
        {
            /// <inheritdoc />
            public void Execute(ITestOutputHelper testOutputHelper)
            {
                var settings = new RelativitySettings()
                {
                    DesktopClientPath = @"C:\Program Files\kCura Corporation\Relativity Desktop Client\Relativity.Desktop.Client.exe",
                    RelativityPassword = "Test1234!",
                    RelativityUsername = "relativity.admin@relativity.com"
                };


                var step = new RelativityImportStep()
                {
                    FilePath = new Constant<string>(@"C:\Users\wainw\source\repos\Examples\Concordance\Carla2\loadfile.dat"),
                    FileImportType = new Constant<FileImportType>(FileImportType.Object),
                    SettingsFilePath = new Constant<string>(@"C:\Users\wainw\source\repos\Examples\Concordance\LoadSettings.kwe"),
                    Configuration = null,
                    StartLineNumber = null,
                    WorkspaceId = new Constant<int>(1017936)
                };


                var loggerFactory = new LoggerFactory(new[] { new XunitLoggerProvider(testOutputHelper) });

                var logger = loggerFactory.CreateLogger(Name);
                var stateMonad = new StateMonad(logger, settings, ExternalProcessRunner.Instance);


                var r = step.Run(stateMonad);

                r.ShouldBeSuccessful(x=>x.AsString);
            }

            /// <inheritdoc />
            public string Name => "Import Test";
        }
    }

}
