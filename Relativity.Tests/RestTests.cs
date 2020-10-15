using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Xunit;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.Utilities.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Reductech.Connectors.Relativity.Tests
{

    public class Tests //These are integration tests. At some point we need to set up ci for them properly
    {
        public Tests(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
            Settings = new RelativitySettings
            {
                RelativityUsername = "relativity.admin@relativity.com",
                RelativityPassword = "Test1234!",
                Url = "http://172.20.0.11"
            };

            WorkspaceId = 1017936;
        }

        public IRelativitySettings Settings { get; }

        public ITestOutputHelper TestOutputHelper { get; }

        public int WorkspaceId { get; }

        [Fact(Skip = "integration")]
        public async Task TestFieldMapping()
        {
            var fields = await FieldMapping.GetMappableFields(Settings, false, WorkspaceId);

            fields.Should().NotBeEmpty();

            foreach (var mappableSourceField in fields)
            {
                TestOutputHelper.WriteLine(mappableSourceField.ToString());
            }
        }


        [Fact(Skip = "integration")]
        public async Task TestFirstDocument()
        {

            var r = await DocumentQueryHelper.GetDocumentQueryResultAsync(Settings, WorkspaceId);

            r.ShouldBeSuccessful();


            r.Value.Results.Should().NotBeEmpty().And.NotContainNulls().And

            .OnlyContain(x => x.ArtifactID > 0).And.OnlyContain(x => x.Location != null);

            foreach (var documentResult in r.Value.Results)
            {
                TestOutputHelper.WriteLine(documentResult.Location);
            }
        }

        [Fact(Skip = "integration")]
        public async Task TestRetrieve()
        {
            var r = await RestRetrieve.GetResultAsync(Settings, WorkspaceId);

            r.ShouldBeSuccessful();

            r.Value.Object.FieldValues.Should().NotBeEmpty();

            foreach (var fv in r.Value.Object.FieldValues)
            {
                var o = (fv.Field.Name, fv.Field.FieldType, fv.Field.ArtifactID, fv.Field.FieldCategory, fv.Value);


                TestOutputHelper.WriteLine(o.ToString());
            }
        }


        [Fact(Skip = "integration")]
        public async Task TestDownloadFile()
        {
            var result = await DocumentFileManager.DownloadFile(Settings, WorkspaceId, 1040848, CancellationToken.None);

            result.ShouldBeSuccessful(x => x.Message);

            result.Value.Should().NotBeNullOrWhiteSpace();

            TestOutputHelper.WriteLine(result.Value);
        }

        [Fact(Skip = "integration")]
        public void TestExport()
        {
            var fieldNames = new List<string>
            {
                //1003667,// Control number,
                //1035374, // File name
                //1035395, //Title
                //1003669, //md5 hash
                //1003672, //has images
                //1003673, //has native
                //1035352, //Date created
                "Title",
                "Extracted Text"

            };

            var condition = "'Extracted Text' ISSET ";


            var exportStep = new RelativityExportStep()
            {
                BatchSize = new Constant<int>(10),
                Condition = new Constant<string>(condition),
                FieldNames = new Constant<List<string>>(fieldNames),
                WorkspaceId = new Constant<int>(WorkspaceId)
            };

            var loggerFactory = new LoggerFactory(new[] { new XunitLoggerProvider(TestOutputHelper) });
            var logger = loggerFactory.CreateLogger("Export");

            var state = new StateMonad(logger, Settings, null);


            var result = exportStep.Run(state);

            result.ShouldBeSuccessful(x => x.AsString);

            result.Value.Count.Should().BeGreaterThan(0);

            foreach (var r in result.Value.Take(5))
            {
                TestOutputHelper.WriteLine(r);
            }

        }

    }

}
