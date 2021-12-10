using Moq;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

public partial class RelativityCreateFieldTests : StepTestBase<RelativityCreateField, int>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Create a folder with a parent folder",
                    new RelativityCreateField()
                    {
                        Workspace = new OneOfStep<int, StringStream>(StaticHelpers.Constant(13)),
                        FieldName = StaticHelpers.Constant("MyField")
                    },
                    42
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IFieldManager1, int>(
                        x => x.CreateFixedLengthFieldAsync(
                            13,
                            It.Is<FixedLengthFieldRequest1>(
                                field => field.Name == "MyField"
                                      && field.Length == 100
                            )
                        ),
                        42
                    )
                );
        }
    }
}
