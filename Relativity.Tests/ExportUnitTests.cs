//using System.Collections.Generic;
//using System.Linq;
//using Flurl.Http.Testing;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Xunit;
//using Reductech.EDR.Core;
//using Reductech.EDR.Core.Internal;
//using Xunit;
//using Xunit.Abstractions;

//namespace Reductech.Connectors.Relativity.Tests
//{

//    public class ExportUnitTests : ExportUnitTestCases
//    {
//        public ExportUnitTests(ITestOutputHelper testOutputHelper) => TestOutputHelper = testOutputHelper;

//        /// <inheritdoc />
//        [Theory]
//        [ClassData(typeof(ExportUnitTestCases))]
//        public override void Test(string key) => base.Test(key);
//    }

//    public class ExportUnitTestCases : TestBase
//    {
//        /// <inheritdoc />
//        protected override IEnumerable<ITestBaseCase> TestCases {
//            get
//            {
//                yield return new ExportTestCase("My Test Case");
//            }
//        }

//        private class ExportTestCase : ITestBaseCase
//        {
//            public ExportTestCase(string name)
//            {
//                Name = name;
//            }

//            /// <inheritdoc />
//            public string Name { get; }

//            /// <inheritdoc />
//            public void Execute(ITestOutputHelper testOutputHelper)
//            {

//                using var httpTest = new HttpTest();

//                httpTest.RespondWithJson(new RelativityExport.ExportResult()
//                {
//                    RecordCount = 6,
//                    RunID = "MyRun"
//                });

//                httpTest.RespondWithJson(new List<RelativityExport.ExportResultElement>()
//                {
//                    new RelativityExport.ExportResultElement()
//                    {
//                        ArtifactID = 12345,
//                        Values = new List<object>()
//                        {
//                            "Hello", "World"
//                        }
//                    }
//                });

//                //httpTest.Configure(x=>x.)

//                var settings = new RelativitySettings
//                {
//                    RelativityUsername = "fake@parody.com",
//                    RelativityPassword = "swordfish",
//                    Url = "http://123.45.6.7"
//                };

//                var workspaceId = 1017936;

//                var fieldNames = new List<string>
//                {
//                    //1003667,// Control number,
//                    //1035374, // File name
//                    //1035395, //Title
//                    //1003669, //md5 hash
//                    //1003672, //has images
//                    //1003673, //has native
//                    //1035352, //Date created
//                    "Title",
//                    "Extracted Text"

//                };

//                var condition = "'Extracted Text' ISSET ";

//                var exportStep = new RelativityExportStep()
//                {
//                    BatchSize = new Constant<int>(10),
//                    Condition = new Constant<string>(condition),
//                    FieldNames = new Constant<List<string>>(fieldNames),
//                    WorkspaceId = new Constant<int>(workspaceId)
//                };

//                var loggerFactory = new LoggerFactory(new[] { new XunitLoggerProvider(testOutputHelper) });
//                var logger = loggerFactory.CreateLogger("Export");

//                var state = new StateMonad(logger, settings, null);

//                var result = exportStep.Run(state);

//                result.ShouldBeSuccessful(x => x.AsString);

//                result.Value.Count.Should().BeGreaterThan(0);

//                foreach (var r in result.Value.Take(5))
//                {
//                    testOutputHelper.WriteLine(r);
//                }
//            }

//        }
//    }
//}


