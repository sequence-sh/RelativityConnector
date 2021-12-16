using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.DataContracts.DTOs;
using Relativity.Services.Objects.DataContracts;
using Entity = Reductech.EDR.Core.Entity;
using SortEnum = Relativity.Services.Objects.DataContracts.SortEnum;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

public partial class
    RelativityQueryDocumentsTests : StepTestBase<RelativitySendQuery, Array<Entity>>
{
    static bool MatchQueryRequest(QueryRequest qr)
    {
        qr.Condition.Should().Be("Test Condition");
        qr.ObjectType.ArtifactTypeID.Should().Be((int)ArtifactType.View);
        qr.Fields.Select(x => x.ArtifactID).Should().BeEquivalentTo(new []{100, 101});
        qr.Sorts.Should().HaveCount(1);

        qr.Sorts.Single().Direction.Should().Be( SortEnum.Descending);
        qr.Sorts.Single().FieldIdentifier.ArtifactID.Should().Be(13);

        return true;
    }

    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Query Documents",
                    new ForEach<Entity>
                    {
                        Array = new RelativitySendQuery
                        {
                            Workspace = new OneOfStep<SCLInt, StringStream>(Constant(11)),
                            Condition = Constant("Test Condition"),
                            ArtifactType =
                                new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(Constant(ArtifactType.View)),
                            Fields =
                                new OneOfStep<Array<SCLInt>, Array<StringStream>>(Array(100, 101)),
                            Length         = Constant(50),
                            SortArtifactId = Constant(13),
                            SortDirection  = Constant(SortEnum.Descending),
                            Start          = Constant(10)
                        },
                        Action = new LambdaFunction<Entity, Unit>(
                            null,
                            new Log { Value = new GetAutomaticVariable<Entity>() }
                        )
                    },
                    Unit.Default,
                    "Progress Message 1",
                    @"('Name': ""Result 1"" 'ArtifactID': 11111 'Field a': ""Test Value 1a"" 'Field b': ""Test Value 1b"")",
                    @"('Name': ""Result 2"" 'ArtifactID': 22222 'Field a': ""Test Value 2a"" 'Field b': ""Test Value 2b"")"
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IObjectManager1, QueryResult>(
                        x =>
                            x.QueryAsync(
                                11,
                                It.Is<QueryRequest>(q => MatchQueryRequest(q)),
                                10,
                                50,
                                It.IsAny<CancellationToken>(),
                                It.IsAny<IProgress<ProgressReport>>()
                            ),
                        new QueryResult
                        {
                            CurrentStartIndex = 10,
                            ResultCount       = 50,
                            TotalCount        = 1000,
                            ObjectType        = new ObjectType { ArtifactID = 12 },
                            Objects = new List<RelativityObject>
                            {
                                new()
                                {
                                    ArtifactID = 11111,
                                    Name       = "Result 1",
                                    FieldValues = new List<FieldValuePair>
                                    {
                                        new()
                                        {
                                            Value = "Test Value 1a",
                                            Field = new Field
                                            {
                                                ArtifactID = 100,
                                                Name       = "Field a"
                                            }
                                        },
                                        new()
                                        {
                                            Value = "Test Value 1b",
                                            Field = new Field
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
                                    FieldValues = new List<FieldValuePair>
                                    {
                                        new()
                                        {
                                            Value = "Test Value 2a",
                                            Field = new Field
                                            {
                                                ArtifactID = 100,
                                                Name       = "Field a"
                                            }
                                        },
                                        new()
                                        {
                                            Value = "Test Value 2b",
                                            Field = new Field
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
                            Maybe<Action<ISetup<IObjectManager1, Task<QueryResult>>>>.From(
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
