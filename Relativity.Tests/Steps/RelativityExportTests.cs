using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Moq;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Shared.Models;
using Relativity.Kepler.Transport;
using Relativity.Services.DataContracts.DTOs.Results;
using Relativity.Services.Field;
using Relativity.Services.Interfaces.Document;
using Relativity.Services.Objects;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;
using QueryRequest = Relativity.Services.Objects.DataContracts.QueryRequest;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityExportTests : StepTestBase<RelativityExport, Array<Entity>>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                Guid runId = new Guid("8321b8c6-7b47-4cf7-a309-ad163ea907d0");
                //var expectedExportResult = new RelativityExportHelpers.ExportResult()
                //{
                //    RecordCount = 10, RunID = "acbd"
                //};

                //var flurlClientFactory = new TestFlurlClientFactory();
                //flurlClientFactory.HttpTest
                //    .ForCallsTo(
                //        "http://TestRelativityServer/Relativity.REST/api/Relativity.Objects/workspace/12345/object/initializeexport"
                //    )
                //    .RespondWithJson(expectedExportResult);


                //flurlClientFactory.HttpTest
                //    .ForCallsTo(
                //        "http://TestRelativityServer/Relativity.REST/api/Relativity.Objects/workspace/12345/object/retrieveNextResultsBlockFromExport")
                //    .RespondWithJson(
                //        new List<RelativityExportHelpers.ExportResultElement>()
                //        {
                //            new()
                //            {
                //                ArtifactID = 5678,
                //                Values = new List<object>()
                //                {
                //                    "Hello",
                //                    RelativityExportHelpers.LongStringToken
                //                }
                //            }
                //        }
                //    );

                //flurlClientFactory.HttpTest
                //    .ForCallsTo(
                //        "http://TestRelativityServer/Relativity.REST/api/Relativity.Objects/workspace/12345/object/streamlongtext")
                //    .RespondWith("Streamed Long Text");


                //flurlClientFactory.HttpTest
                //    .ForCallsTo(
                //        "http://TestRelativityServer/Relativity.REST/api/Relativity.Document/workspace/12345/downloadnativefile/5678")
                //    .RespondWith("My Native Text");

                //flurlClientFactory.HttpTest.RespondWith(status: 417);
                //flurlClientFactory.HttpTest

                yield return new StepCase(
                        "Export with condition",
                        new ForEach<Entity>()
                        {
                            Array = new RelativityExport()
                            {
                                WorkspaceId = Constant(12345),
                                Condition = Constant("'Extracted Text' ISSET "),
                                FieldNames = Array("ShortField", "LongField"),
                                BatchSize = Constant(10)
                            },
                            Action = new LambdaFunction<Entity, Unit>(null,
                                new Log<StringStream>()
                                {
                                    Value = new GetAutomaticVariable<StringStream>()
                                })
                        }
                        , Unit.Default,
                        "(ShortField: \"Hello\" LongField: \"Streamed Long Text\" NativeFile: \"My Native Text\")"
                    )
                    .WithTestRelativitySettings()
                    .WithService(
                        new MockSetup<IDocumentFileManager, IKeplerStream>
                        (d => d.DownloadNativeFileAsync(42, 111),
                            MakeKeplerStream("Long long long text")
                        ),
                        new MockSetup<IObjectManager, ExportInitializationResults>(manager =>
                                manager.InitializeExportAsync(42, It.IsAny<QueryRequest>(), 0),
                            new ExportInitializationResults()
                            {
                                RecordCount = 10,
                                RunID = runId,
                                FieldData =
                                    new List<FieldMetadata>()
                                    {
                                        new() { Name = "Field 1" }
                                    }
                            }
                        ));
            }
        }

        public static IKeplerStream MakeKeplerStream(string s)
        {
            return new KeplerStream(new MemoryStream(Encoding.UTF8.GetBytes(s)));
        }


        /// <inheritdoc />
        protected override IEnumerable<ErrorCase> ErrorCases
        {
            get
            {
                foreach (var errorCase in base.ErrorCases)
                {
                    yield return errorCase.WithTestRelativitySettings();
                }
            }
        }
    }
}