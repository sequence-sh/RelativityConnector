using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Flurl.Http.Testing;
using Reductech.EDR.Core;
using Reductech.EDR.Core.TestHarness;
using Xunit;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests
{
    public partial class RelativityExportTests : StepTestBase<RelativityExport, Array<StringStream>>
    {

        [Fact]
        public async void CheckFlurl()
        {
            using var httpTest = new HttpTest();

            httpTest.ForCallsTo(
                "http://a/cb"
            ).RespondWith("abc");

            var flurlClient = httpTest.GetFlurlClient();

            var urlSuffix = $"/Relativity.REST/api/Relativity.Objects/workspace/12345/object/initializeexport";

            var url = Url.Combine("http://TestRelativityServer", urlSuffix);

            var r = await url
                    .WithBasicAuth("UName", "PWord")
                    .WithHeader("X-CSRF-Header", "-")
                .WithClient(flurlClient)
                .PostJsonAsync("my data", CancellationToken.None)
                .ReceiveJson<RelativityExportHelpers.ExportResult>()
                ;

            r.RunID.Should().Be("ab");
        }

        [Fact]
        public async void CheckFlurl2()
        {
            var test = StepCases.Single();

            await test.RunAsync(TestOutputHelper);
        }

        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                using var httpTest = new HttpTest();

                httpTest.RespondWith("Test Error", (int) HttpStatusCode.ExpectationFailed);

                //httpTest.ForCallsTo(
                //    "http://TestRelativityServer/Relativity.REST/api/Relativity.Objects/workspace/12345/object/initializeexport"
                //).RespondWith("abc");

                var flurlClient = httpTest.GetFlurlClient();
                

                yield return new StepCase(
                            "Export with condition",
                            new RelativityExport()
                            {
                                WorkspaceId = Constant(12345),
                                Condition = Constant("'Extracted Text' ISSET "),
                                FieldNames = Array("Field1", "Field2"),
                                BatchSize = Constant(10)
                            },
                            new EagerArray<StringStream>(
                                new List<StringStream>()
                                {
                                    "ABC"
                                }
                            )
                        )

                        .WithRelativitySettings<RelativityExport, Array<StringStream>, StepCase>(
                            new RelativitySettings()
                            {
                                RelativityUsername = "UN",
                                RelativityPassword = "PW",
                                Url = "http://TestRelativityServer"
                            }
                        )
                        .WithContext(
                            ConnectorInjection.FlurlClientKey,
                            flurlClient
                        )
                    ;
            }
        }


        /// <inheritdoc />
        protected override IEnumerable<ErrorCase> ErrorCases
        {
            get
            {
                foreach (var errorCase in base.ErrorCases)
                {
                    yield return
                        errorCase.WithRelativitySettings<RelativityExport, Array<StringStream>, ErrorCase>(
                            new RelativitySettings()
                            {
                                RelativityUsername = "Username",
                                RelativityPassword = "Passport",
                            }
                        )
                        ;
                }
            }
        }
    }
}