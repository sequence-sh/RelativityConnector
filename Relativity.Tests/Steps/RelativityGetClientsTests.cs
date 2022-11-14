using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Shared.V1.Models;

namespace Sequence.Connectors.Relativity.Tests.Steps;

public partial class RelativityGetClientsTests : StepTestBase<RelativityGetClients, Array<Entity>>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Get Clients",
                    TestHelpers.LogAllEntities(new RelativityGetClients()),
                    Unit.Default,
                    "('Name': \"Client 1\" 'ArtifactID': 1 'Guids': [])",
                    "('Name': \"Client 2\" 'ArtifactID': 2 'Guids': [])"
                )
                .WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IMatterManager1, List<DisplayableObjectIdentifier>>(
                        x => x.GetEligibleClientsAsync(),
                        new List<DisplayableObjectIdentifier>
                        {
                            new() { ArtifactID = 1, Name = "Client 1" },
                            new() { ArtifactID = 2, Name = "Client 2" },
                        }
                    )
                );
        }
    }
}
