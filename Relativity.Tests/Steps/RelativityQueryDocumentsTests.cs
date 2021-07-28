using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Moq;
using Moq.Language.Flow;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;
using Reductech.EDR.Core.Util;
using Relativity.Services.DataContracts.DTOs;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class
    RelativityQueryDocumentsTests : StepTestBase<RelativityQueryDocuments, Array<Entity>>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Query Documents",
                    new ForEach<Entity>()
                    {
                        Array = new RelativityQueryDocuments()
                        {
                            WorkspaceArtifactId = Constant(11),
                            Condition           = Constant("Test Condition"),
                            ArtifactTypeId      = Constant(12),
                            FieldArtifactIds    = Array(100, 101),
                            Length              = Constant(50),
                            SortArtifactId      = Constant(13),
                            SortDirection       = Constant(SortEnum.Descending),
                            Start               = Constant(10)
                        },
                        Action = new LambdaFunction<Entity, Unit>(
                            null,
                            new Log<Entity>() { Value = new GetAutomaticVariable<Entity>() }
                        )
                    },
                    Unit.Default,
                    "Progress Message 1",
                    "(ParentObject: \"\" Name: \"Result 1\" FieldValues: [(Value: \"Test Value 1a\" Field: (FieldCategory: 0 FieldType: 0 ViewFieldID: 0 ArtifactID: 100 Guids: \"\" Name: \"Field a\")), (Value: \"Test Value 1b\" Field: (FieldCategory: 0 FieldType: 0 ViewFieldID: 0 ArtifactID: 101 Guids: \"\" Name: \"Field b\"))] ArtifactID: 11111 Guids: \"\")",
                    "(ParentObject: \"\" Name: \"Result 2\" FieldValues: [(Value: \"Test Value 2a\" Field: (FieldCategory: 0 FieldType: 0 ViewFieldID: 0 ArtifactID: 100 Guids: \"\" Name: \"Field a\")), (Value: \"Test Value 2b\" Field: (FieldCategory: 0 FieldType: 0 ViewFieldID: 0 ArtifactID: 100 Guids: \"\" Name: \"Field b\"))] ArtifactID: 22222 Guids: \"\")"
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IObjectManager, QueryResult>(
                        x =>
                            x.QueryAsync(
                                11,
                                It.Is<QueryRequest>(qr => true),
                                10,
                                50,
                                It.IsAny<CancellationToken>(),
                                It.IsAny<IProgress<ProgressReport>>()
                            ),
                        new QueryResult()
                        {
                            CurrentStartIndex = 10,
                            ResultCount       = 50,
                            TotalCount        = 1000,
                            ObjectType        = new ObjectType() { ArtifactID = 12 },
                            Objects = new List<RelativityObject>()
                            {
                                new()
                                {
                                    ArtifactID = 11111,
                                    Name       = "Result 1",
                                    FieldValues = new List<FieldValuePair>()
                                    {
                                        new()
                                        {
                                            Value = "Test Value 1a",
                                            Field = new Field()
                                            {
                                                ArtifactID = 100,
                                                Name       = "Field a"
                                            }
                                        },
                                        new()
                                        {
                                            Value = "Test Value 1b",
                                            Field = new Field()
                                            {
                                                ArtifactID = 101,
                                                Name       = "Field b"
                                            }
                                        },
                                    }
                                },
                                new()
                                {
                                    ArtifactID = 22222,
                                    Name       = "Result 2",
                                    FieldValues = new List<FieldValuePair>()
                                    {
                                        new()
                                        {
                                            Value = "Test Value 2a",
                                            Field = new Field()
                                            {
                                                ArtifactID = 100,
                                                Name       = "Field a"
                                            }
                                        },
                                        new()
                                        {
                                            Value = "Test Value 2b",
                                            Field = new Field()
                                            {
                                                ArtifactID = 100,
                                                Name       = "Field b"
                                            }
                                        },
                                    }
                                },
                            }
                        }
                    )
                    {
                        AdditionalAction =
                            Maybe<Action<ISetup<IObjectManager, Task<QueryResult>>>>.From(
                                x => x.Callback(
                                    new Action<int, QueryRequest, int, int, CancellationToken,
                                        IProgress<ProgressReport>>(
                                        (
                                            _,
                                            _,
                                            _,
                                            _,
                                            _,
                                            progress) =>
                                        {
                                            progress.Report(
                                                new ProgressReport("Progress Message 1", null, null)
                                            );
                                        }
                                    )
                                )
                            )
                    }
                );
        }
    }
}

}
