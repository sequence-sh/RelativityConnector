using System;
using System.Collections.Generic;
using Moq;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.TestHarness;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;
using Reductech.EDR.Core.Util;
using Relativity.Services.Folder;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityDeleteUnusedFoldersTests : StepTestBase<RelativityDeleteUnusedFolders, Unit>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                yield return new StepCase("Delete Unused Folders",
                        new RelativityDeleteUnusedFolders()
                        {
                            WorkspaceArtifactId = Constant(42)
                        },
                        Unit.Default
                    ).WithTestRelativitySettings()
                    .WithService(new Action<Mock<IFolderManager>>(
                        m =>
                            m.Setup(x => x.DeleteUnusedFoldersAsync(42))
                                .ReturnsAsync(new FolderResultSet() { Success = true })
                    ));
            }
        }


        /// <inheritdoc />
        protected override IEnumerable<ErrorCase> ErrorCases
        {
            get
            {
                yield return new ErrorCase("Result unsuccessful",
                        new RelativityDeleteUnusedFolders()
                        {
                            WorkspaceArtifactId = Constant(42)
                        },
                        ErrorCode_Relativity.Unsuccessful.ToErrorBuilder("Test Error Message")
                    ).WithTestRelativitySettings()
                    .WithService(new Action<Mock<IFolderManager>>(m =>
                        m.Setup(x => x.DeleteUnusedFoldersAsync(42))
                            .ReturnsAsync(new FolderResultSet() { Success = false, Message = "Test Error Message" })
                    ));


                foreach (var errorCase in base.ErrorCases)
                {
                    yield return errorCase;
                }
            }
        }
    }
}