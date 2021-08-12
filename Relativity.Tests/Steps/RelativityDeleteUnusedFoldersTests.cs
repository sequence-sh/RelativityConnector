using System.Collections.Generic;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.TestHarness;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{

public partial class
    RelativityDeleteUnusedFoldersTests : StepTestBase<RelativityDeleteUnusedFolders, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                    "Delete Unused Folders",
                    new RelativityDeleteUnusedFolders()
                    {
                        Workspace = new OneOfStep<int, StringStream>(Constant(42)),
                    },
                    Unit.Default
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IFolderManager1, FolderResultSet>(
                        x => x.DeleteUnusedFoldersAsync(42),
                        new FolderResultSet() { Success = true }
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
                    "Result unsuccessful",
                    new RelativityDeleteUnusedFolders()
                    {
                        Workspace = new OneOfStep<int, StringStream>(Constant(42)),
                    },
                    ErrorCode_Relativity.Unsuccessful.ToErrorBuilder("Test Error Message")
                ).WithTestRelativitySettings()
                .WithService(
                    new MockSetup<IFolderManager1, FolderResultSet>(
                        x => x.DeleteUnusedFoldersAsync(42),
                        new FolderResultSet() { Success = false, Message = "Test Error Message" }
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
