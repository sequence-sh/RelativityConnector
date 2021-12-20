//using System;
//using System.Collections.Generic;
//using CSharpFunctionalExtensions;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Xunit;
//using Moq;
//using Reductech.Sequence.Core;
//using Reductech.Sequence.Core.Internal;
//using Reductech.Sequence.Core.Util;
//using Reductech.Utilities.Testing;
//using Xunit;
//using Xunit.Abstractions;

//namespace Reductech.Connectors.Relativity.Tests
//{
//    public class ImportUnitTestCases : ImportUnitTests
//    {

//        public ImportUnitTestCases(ITestOutputHelper testOutputHelper) => TestOutputHelper = testOutputHelper;

//        /// <inheritdoc />
//        [Theory]
//        [ClassData(typeof(ImportUnitTests))]
//        public override void Test(string key) => base.Test(key);
//    }

//    public class ImportUnitTests : TestBase
//    {
//        /// <inheritdoc />
//        protected override IEnumerable<ITestBaseCase> TestCases
//        {
//            get
//            {
//yield return new ImportTest(
//    new RelativityImportStep()
//    {
//        FilePath = new Constant<string>("C:/Data"),
//        WorkspaceId = new Constant<int>(1234),
//        SettingsFilePath = new Constant<string>("C:/Settings"),
//        FileImportType = new Constant<FileImportType>(FileImportType.Object),
//        LoadFileEncoding = new Constant<int>(456),
//        FullTextFileEncoding = new Constant<int>(567),
//        StartLineNumber = new Constant<int>(5),
//        DestinationFolder = new Constant<int>(789),
//    },
//    "-f:C:/Data", "-c:1234", "-m:o", "-k:C:/Settings", "-s:5", "-d:789", "-e:456", "-x:567", "-u:UN", "-p:PW"

//    );
//            }
//        }

//        public class ImportTest : ITestBaseCase
//        {
//            public ImportTest(RelativityImportStep importStep, params string[] expectedArgs)
//            {
//                ImportStep = importStep;
//                ExpectedArgs = expectedArgs;
//            }

//            /// <inheritdoc />
//            public string Name => string.Join(", ", ExpectedArgs);

//            public RelativityImportStep ImportStep { get; }
//            public string[] ExpectedArgs { get; }

//            /// <inheritdoc />
//            public void Execute(ITestOutputHelper testOutputHelper)
//            {
//                var settings = new RelativitySettings
//                {
//                    DesktopClientPath = @"C:\Relativity.Desktop.Client.exe",
//                    RelativityPassword = "PW",
//                    RelativityUsername = "UN"
//                };

//                var moq = new MockRepository(MockBehavior.Strict);

//                var externalProcessRunnerMock = moq.Create<IExternalProcessRunner>();

//                var loggerFactory = new LoggerFactory(new[] { new XunitLoggerProvider(testOutputHelper) });
//                var logger = loggerFactory.CreateLogger(Name);

//                externalProcessRunnerMock.Setup(x =>
//                    x.RunExternalProcess(@"C:\Relativity.Desktop.Client.exe",
//                        logger, nameof(RelativityImportStep), It.IsAny<IErrorHandler>(),
//                        ExpectedArgs)).ReturnsAsync(()=> Result.Success <Unit, IRunErrors>(Unit.Default));

//                var monad = new StateMonad(logger, settings, externalProcessRunnerMock.Object);

//                var result = ImportStep.Run(monad);

//                result.ShouldBeSuccessful(x=>x.AsString);
//            }

//        }
//    }
//}


