//using System.Collections.Generic;
//using Sequence.Connectors.Relativity.Steps;
//using Sequence.Core;
//using Sequence.Core.TestHarness;
//using Sequence.Core.Util;
//using Relativity.Services.Interfaces.Field;
//using Relativity.Services.Interfaces.Shared.Models;

//namespace Sequence.Connectors.Relativity.Tests.Steps
//{

//public partial class
//    RelativityGetAvailableObjectTypesTests : StepTestBase<RelativityGetAvailableObjectTypes,
//        Array<Entity>>
//{
//    /// <inheritdoc />
//    protected override IEnumerable<StepCase> StepCases
//    {
//        get
//        {
//            yield return new StepCase(
//                    "Get available object types",
//                    TestHelpers.LogAllEntities(
//                        new RelativityGetAvailableObjectTypes()
//                        {
//                            WorkspaceArtifactId = StaticHelpers.Constant(42)
//                        }
//                    ),
//                    Unit.Default,
//                    "(Name: \"Field 1\" ArtifactTypeID: 0 ArtifactID: 1 Guids: \"\")",
//                    "(Name: \"Field 2\" ArtifactTypeID: 0 ArtifactID: 2 Guids: \"\")"
//                )
//                .WithTestRelativitySettings()
//                .WithService(
//                    new MockSetup<IFieldManager, List<ObjectTypeIdentifier>>(
//                        x => x.GetAvailableObjectTypesAsync(42),
//                        new List<ObjectTypeIdentifier>()
//                        {
//                            new() { Name = "Field 1", ArtifactID = 1 },
//                            new() { Name = "Field 2", ArtifactID = 2 }
//                        }
//                    )
//                );
//        }
//    }
//}

//}
