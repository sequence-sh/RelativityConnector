using System.Collections.Generic;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests
{
    public partial class RelativityExportTests : StepTestBase<RelativityExport, Array<StringStream>>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                var expectedExportResult = new RelativityExportHelpers.ExportResult()
                {
                    RecordCount = 10, RunID = "acbd"
                };

                var flurlClientFactory = new TestFlurlClientFactory();
                flurlClientFactory.HttpTest
                    .ForCallsTo(
                        "http://TestRelativityServer/Relativity.REST/api/Relativity.Objects/workspace/12345/object/initializeexport"
                    )
                    .RespondWithJson(expectedExportResult);


                flurlClientFactory.HttpTest
                    .ForCallsTo(
                        "http://TestRelativityServer/Relativity.REST/api/Relativity.Objects/workspace/12345/object/retrieveNextResultsBlockFromExport")
                    .RespondWithJson(
                        new List<RelativityExportHelpers.ExportResultElement>()
                        {
                            new()
                            {
                                ArtifactID = 5678,
                                Values = new List<object>()
                                {
                                    "Hello",
                                    RelativityExportHelpers.LongStringToken
                                }
                            }
                        }
                        
                        );

                flurlClientFactory.HttpTest
                    .ForCallsTo(
                        "http://TestRelativityServer/Relativity.REST/api/Relativity.Objects/workspace/12345/object/streamlongtext")
                    .RespondWith("Streamed Long Text");


                flurlClientFactory.HttpTest
                    .ForCallsTo(
                        "http://TestRelativityServer/Relativity.REST/api/Relativity.Document/workspace/12345/downloadnativefile/5678")
                    .RespondWith("My Native Text");

                flurlClientFactory.HttpTest.RespondWith(status: 417);
                //flurlClientFactory.HttpTest

                yield return new StepCase(
                            "Export with condition",
                            new ForEach<StringStream>()
                            {
                                Array = new RelativityExport()
                                {
                                    WorkspaceId = Constant(12345),
                                    Condition = Constant("'Extracted Text' ISSET "),
                                    FieldNames = Array("ShortField", "LongField"),
                                    BatchSize = Constant(10)
                                },
                                Action = new LambdaFunction<StringStream, Unit>(null,
                                    new Log<StringStream>()
                                    {
                                        Value = new GetAutomaticVariable<StringStream>()
                                    })
                            }
                            , Unit.Default, "LongField:Streamed Long Text ShortField:Hello "
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
                            ConnectorInjection.FlurlClientFactoryKey,
                            flurlClientFactory
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