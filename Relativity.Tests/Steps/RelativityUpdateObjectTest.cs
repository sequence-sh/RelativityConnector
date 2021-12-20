using FluentAssertions;
using Moq;
using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Objects.DataContracts;

namespace Reductech.Sequence.Connectors.Relativity.Tests.Steps;

public partial class RelativityUpdateObjectTest : StepTestBase<RelativityUpdateObject, Unit>
{
    static bool CheckUpdateRequest(UpdateRequest updateRequest)
    {
        updateRequest.Object.ArtifactID.Should().Be(22);
        updateRequest.FieldValues.Should().HaveCount(1);
        updateRequest.FieldValues.First().Field.Name.Should().Be("a");
        updateRequest.FieldValues.First().Value.Should().Be(1);
        return true;
    }
    
    static bool CheckUpdateOptions(UpdateOptions updateOptions)
    {
        updateOptions.UpdateBehavior.Should().Be(FieldUpdateBehavior.Replace);
        return true;
    }

    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Test Update Field",
                    new RelativityUpdateObject
                    {
                        Workspace        = new OneOfStep<SCLInt, StringStream>(Constant(11)),
                        ObjectArtifactId = Constant(22),
                        FieldValues      = Constant(Entity.Create(("a", 1))),
                        UpdateBehaviour  = Constant(UpdateBehaviour.Replace)
                    },
                    Unit.Default
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IObjectManager1, UpdateResult>(
                        x=>x.UpdateAsync(11,
                                         It.Is<UpdateRequest>(a=> CheckUpdateRequest(a)),
                                         It.Is<UpdateOptions>(a=>CheckUpdateOptions(a))
                            
                        ),
                        new UpdateResult
                        {
                            EventHandlerStatuses = new List<EventHandlerStatus>
                            {
                                new EventHandlerStatus
                                {
                                    Success = true
                                }
                            }
                        }
                        
                    )
                    
                );
        }
    }
}
