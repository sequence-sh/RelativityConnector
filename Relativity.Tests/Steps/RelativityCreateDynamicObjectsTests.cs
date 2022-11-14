using System.Net.Http;
using Moq;
using Sequence.Connectors.Relativity.Errors;
using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Objects.DataContracts;

namespace Sequence.Connectors.Relativity.Tests.Steps;

public partial class
    RelativityCreateDynamicObjectsTests : StepTestBase<RelativityCreateDynamicObjects, Array<SCLInt>>
{
    static bool CheckCreateRequest(MassCreateRequest createRequest)
    {
        if (createRequest.ObjectType.ArtifactTypeID != 10)
            return false;

        if (!createRequest.Fields.Select(x => x.Name).SequenceEqual(new[] { "alpha", "beta" }))
            return false;

        var expectedValues = "1, null;null, 2;4, 3";

        var actualValues =
            string.Join(
                ";",
                createRequest.ValueLists.Select(
                    x =>
                        string.Join(", ", x.Select(v => v is null ? "null" : v.ToString()))
                )
            );

        if (actualValues != expectedValues)
            return false;

        return true;
    }

    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            var massCreateResult = new MassCreateResult
            {
                Success = true,
                Objects = new List<RelativityObjectRef>
                {
                    new() { ArtifactID = 100 },
                    new() { ArtifactID = 101 },
                    new() { ArtifactID = 102 },
                }
            };

            yield return new StepCase(
                    "Create Dynamic objects with mock service",
                    new RelativityCreateDynamicObjects
                    {
                        ArtifactType = new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(Constant(10)) ,
                        Workspace    = new OneOfStep<SCLInt, StringStream>(Constant(42)),
                        Entities = Array(
                            Entity.Create(("alpha", 1)),
                            Entity.Create(("beta", 2)),
                            Entity.Create(("beta", 3), ("alpha", 4))
                        )
                    },
                    new[] { 100, 101, 102 }.Select(x=>x.ConvertToSCLObject()).ToSCLArray()
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IObjectManager1, MassCreateResult>(
                        x => x.CreateAsync(
                            42,
                            It.Is<MassCreateRequest>(cr => CheckCreateRequest(cr)),
                            It.IsAny<CancellationToken>()
                        ),
                        massCreateResult
                    )
                );
            
            yield return new StepCase(
                    "Create Dynamic objects with mock service using artifactType",
                    new RelativityCreateDynamicObjects
                    {
                        ArtifactType        = new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(Constant(ArtifactType.Document)) ,
                        Workspace = new OneOfStep<SCLInt, StringStream>(Constant(42)),
                        Entities = Array(
                            Entity.Create(("alpha", 1)),
                            Entity.Create(("beta", 2)),
                            Entity.Create(("beta", 3), ("alpha", 4))
                        )
                    },
                    new[] { 100, 101, 102 }.Select(x=>x.ConvertToSCLObject()).ToSCLArray()
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IObjectManager1, MassCreateResult>(
                        x => x.CreateAsync(
                            42,
                            It.Is<MassCreateRequest>(cr => CheckCreateRequest(cr)),
                            It.IsAny<CancellationToken>()
                        ),
                        massCreateResult
                    )
                );

            yield return new StepCase(
                    "Create Dynamic objects with mock http",
                    new RelativityCreateDynamicObjects
                    {
                        ArtifactType = new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(Constant(10)),
                        Workspace    = new OneOfStep<SCLInt, StringStream>(Constant(42)),
                        Entities = Array(
                            Entity.Create(("alpha", 1)),
                            Entity.Create(("beta", 2)),
                            Entity.Create(("beta", 3), ("alpha", 4))
                        )
                    },
                    new[] { 100, 101, 102 }.Select(x=>x.ConvertToSCLObject()).ToSCLArray()
                ).WithTestRelativitySettings()
                .WithFlurlMocks(
                    x => x.ForCallsTo(
                            "http://TestRelativityServer/Relativity.REST/api/Relativity.Objects/workspace/42/object/create"
                        )
                        .WithVerb(HttpMethod.Post)
                        .RespondWithJson(massCreateResult)
                );
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            yield return new ErrorCase(
                    "Not Success",
                    new RelativityCreateDynamicObjects
                    {
                        ArtifactType = new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(Constant(10)),
                        Workspace    = new OneOfStep<SCLInt, StringStream>(Constant(42)),
                        Entities = Array(
                            Entity.Create(("alpha", 1)),
                            Entity.Create(("beta", 2)),
                            Entity.Create(("beta", 3), ("alpha", 4))
                        )
                    },
                    ErrorCode_Relativity.Unsuccessful.ToErrorBuilder("Test Error")
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IObjectManager1, MassCreateResult>(
                        x => x.CreateAsync(
                            42,
                            It.IsAny<MassCreateRequest>(),
                            It.IsAny<CancellationToken>()
                        ),
                        new MassCreateResult { Success = false, Message = "Test Error" }
                    )
                );

            foreach (var errorCase in base.ErrorCases)
            {
                yield return errorCase;
            }
        }
    }
}
