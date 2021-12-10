using Reductech.EDR.Connectors.Relativity.Steps;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

public partial class RelativityImportEntitiesTests : StepTestBase<RelativityImportEntities, Unit>
{

    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield break;
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            foreach (var errorCase in base.ErrorCases)
            {
                yield return new ErrorCase(
                    errorCase.Name,
                    errorCase.Step,
                    ErrorCode.MissingStepSettings.ToErrorBuilder(
                        "Reductech.EDR.Connectors.Relativity"
                    )
                );
            }
        }
    }
}
