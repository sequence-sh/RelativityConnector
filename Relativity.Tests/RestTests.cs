//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using FluentAssertions;
//using Flurl.Http;
//using Reductech.Sequence.Core.Internal.Errors;
//using Reductech.Sequence.Core.TestHarness;
//using Xunit;
//using Xunit.Abstractions;

//namespace Reductech.Sequence.Connectors.Relativity.Tests
//{
//    public class Tests //These are integration tests. At some point we need to set up ci for them properly
//    {
//        public Tests(ITestOutputHelper testOutputHelper)
//        {
//            TestOutputHelper = testOutputHelper;
//            Settings = new RelativitySettings
//            {
//                RelativityUsername = "relativity.admin@relativity.com",
//                RelativityPassword = "Test1234!",
//                Url = "http://172.20.0.11"
//            };

//            WorkspaceId = 1017936;
//        }

//        public RelativitySettings Settings { get; }

//        public ITestOutputHelper TestOutputHelper { get; }

//        public int WorkspaceId { get; }

//        [Fact(Skip = "integration")]
//        public async Task TestFieldMapping()
//        {
//            var fields = await FieldMapping.GetMappableFields(Settings, GetFlurlClient(), false, WorkspaceId);

//            fields.Should().NotBeEmpty();

//            foreach (var mappableSourceField in fields)
//            {
//                TestOutputHelper.WriteLine(mappableSourceField.ToString());
//            }
//        }

//        //[Fact(Skip = "integration")]
//        //public async Task TestFirstDocument()
//        //{
//        //    var r = await DocumentQueryHelper.GetDocumentQueryResultAsync(Settings, WorkspaceId, GetFlurlClient());

//        //    r.ShouldBeSuccessful();

//        //    r.Value.Results.Should().NotBeEmpty().And.NotContainNulls().And
//        //        .OnlyContain(x => x.ArtifactID > 0).And.OnlyContain(x => x.Location != null);

//        //    foreach (var documentResult in r.Value.Results)
//        //    {
//        //        TestOutputHelper.WriteLine(documentResult.Location);
//        //    }
//        //}

//        //[Fact(Skip = "integration")]
//        //public async Task TestRetrieve()
//        //{
//        //    var r = await RestRetrieve.GetResultAsync(Settings, GetFlurlClient(), WorkspaceId);

//        //    r.ShouldBeSuccessful();

//        //    r.Value.Object.FieldValues.Should().NotBeEmpty();

//        //    foreach (var fv in r.Value.Object.FieldValues)
//        //    {
//        //        var o = (fv.Field.Name, fv.Field.FieldType, fv.Field.ArtifactID, fv.Field.FieldCategory, fv.Value);

//        //        TestOutputHelper.WriteLine(o.ToString());
//        //    }
//        //}

//        [Fact(Skip = "integration")]
//        public async Task TestDownloadFile()
//        {
//            var result = await DocumentFileManager.DownloadFile(Settings,
//                GetFlurlClient(),
//                ErrorLocation.EmptyLocation,
//                WorkspaceId,
//                1040848,
//                CancellationToken.None);

//            result.ShouldBeSuccessful();

//            result.Value.Should().NotBeNullOrWhiteSpace();

//            TestOutputHelper.WriteLine(result.Value);
//        }

//        public static IFlurlClient GetFlurlClient() => new FlurlClient(new HttpClient());

//        //[Fact(Skip = "integration")]
//        //public void TestExport()
//        //{
//        //    var fieldNames = new List<string>
//        //    {
//        //        //1003667,// Control number,
//        //        //1035374, // File name
//        //        //1035395, //Title
//        //        //1003669, //md5 hash
//        //        //1003672, //has images
//        //        //1003673, //has native
//        //        //1035352, //Date created
//        //        "Title",
//        //        "Extracted Text"
//        //    };

//        //    var condition = "'Extracted Text' ISSET ";

//        //    var exportStep = new RelativityExportStep()
//        //    {
//        //        BatchSize = StaticHelpers.Constant(10),
//        //        Condition = StaticHelpers.Constant(condition),
//        //        FieldNames = StaticHelpers.Array(fieldNames.ToArray()),
//        //        WorkspaceId = Constant(WorkspaceId)
//        //    };

//        //    var loggerFactory = new LoggerFactory(new[] { new XunitLoggerProvider(TestOutputHelper) });
//        //    var logger = loggerFactory.CreateLogger("Export");

//        //    var state = new StateMonad(logger,  Settings, null);

//        //    var result = exportStep.Run(state);

//        //    result.ShouldBeSuccessful(x => x.AsString);

//        //    result.Value.Count.Should().BeGreaterThan(0);

//        //    foreach (var r in result.Value.Take(5))
//        //    {
//        //        TestOutputHelper.WriteLine(r);
//        //    }
//        //}
//    }
//}


