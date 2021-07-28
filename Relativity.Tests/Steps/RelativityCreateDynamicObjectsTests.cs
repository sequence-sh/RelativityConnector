using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.TestHarness;
using Relativity.Services.Interfaces.Field;
using Relativity.Services.Interfaces.Shared.Models;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class
    RelativityCreateDynamicObjectsTests : StepTestBase<RelativityCreateDynamicObjects, Array<int>>
{
    static bool CheckCreateRequest(MassCreateRequest createRequest)
    {
        if (createRequest.ObjectType.ArtifactTypeID != 10)
            return false;

        if (!createRequest.Fields.Select(x => x.ArtifactID).SequenceEqual(new[] { 111, 222 }))
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
            yield return new StepCase(
                    "Create Dynamic objects",
                    new RelativityCreateDynamicObjects()
                    {
                        ArtifactTypeId      = Constant(10),
                        WorkspaceArtifactId = Constant(42),
                        Entities = Array(
                            Entity.Create(("alpha", 1)),
                            Entity.Create(("beta", 2)),
                            Entity.Create(("beta", 3), ("alpha", 4))
                        )
                    },
                    new[] { 100, 101, 102 }.ToSCLArray()
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IObjectManager, MassCreateResult>(
                        x => x.CreateAsync(
                            42,
                            It.Is<MassCreateRequest>(cr => CheckCreateRequest(cr)),
                            It.IsAny<CancellationToken>()
                        ),
                        new MassCreateResult()
                        {
                            Success = true,
                            Objects = new List<RelativityObjectRef>()
                            {
                                new() { ArtifactID = 100 },
                                new() { ArtifactID = 101 },
                                new() { ArtifactID = 102 },
                            }
                        }
                    ),
                    new MockSetup<IFieldManager, List<ObjectTypeIdentifier>>(
                        x => x.GetAvailableObjectTypesAsync(42),
                        new List<ObjectTypeIdentifier>()
                        {
                            new() { ArtifactID = 111, Name = "Alpha" },
                            new() { ArtifactID = 222, Name = "Beta" },
                            new() { ArtifactID = 333, Name = "Gamma" },
                        }
                    )
                );
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            yield return new ErrorCase(
                    "Missing Fields",
                    new RelativityCreateDynamicObjects()
                    {
                        ArtifactTypeId      = Constant(10),
                        WorkspaceArtifactId = Constant(42),
                        Entities = Array(
                            Entity.Create(("alpha", 1)),
                            Entity.Create(("beta", 2)),
                            Entity.Create(("beta", 3), ("alpha", 4))
                        )
                    },
                    ErrorCode_Relativity.MissingField.ToErrorBuilder("beta")
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IFieldManager, List<ObjectTypeIdentifier>>(
                        x => x.GetAvailableObjectTypesAsync(42),
                        new List<ObjectTypeIdentifier>()
                        {
                            new() { ArtifactID = 111, Name = "Alpha" },
                        }
                    )
                );

            yield return new ErrorCase(
                    "Not Success",
                    new RelativityCreateDynamicObjects()
                    {
                        ArtifactTypeId      = Constant(10),
                        WorkspaceArtifactId = Constant(42),
                        Entities = Array(
                            Entity.Create(("alpha", 1)),
                            Entity.Create(("beta", 2)),
                            Entity.Create(("beta", 3), ("alpha", 4))
                        )
                    },
                    ErrorCode_Relativity.Unsuccessful.ToErrorBuilder("Test Error")
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IObjectManager, MassCreateResult>(
                        x => x.CreateAsync(
                            42,
                            It.IsAny<MassCreateRequest>(),
                            It.IsAny<CancellationToken>()
                        ),
                        new MassCreateResult() { Success = false, Message = "Test Error" }
                    ),
                    new MockSetup<IFieldManager, List<ObjectTypeIdentifier>>(
                        x => x.GetAvailableObjectTypesAsync(42),
                        new List<ObjectTypeIdentifier>()
                        {
                            new() { ArtifactID = 111, Name = "Alpha" },
                            new() { ArtifactID = 222, Name = "Beta" },
                            new() { ArtifactID = 333, Name = "Gamma" },
                        }
                    )
                );

            foreach (var errorCase in base.ErrorCases)
            {
                yield return errorCase;
            }
        }
    }
}

}
