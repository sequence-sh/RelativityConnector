using System.Collections.Generic;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Relativity.Services.Search;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityDeleteKeywordSearchTests : StepTestBase<RelativityDeleteKeywordSearch, Unit>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                yield return new StepCase("Delete Keyword Search",
                        new RelativityDeleteKeywordSearch()
                        {
                            WorkspaceId = StaticHelpers.Constant(123),
                            SearchId = StaticHelpers.Constant(456),
                        }, Unit.Default
                    ).WithTestRelativitySettings()
                    .WithService(new MockSetupUnit<IKeywordSearchManager>(
                        x => x.DeleteSingleAsync(123, 456)
                    ));
            }
        }
    }
}