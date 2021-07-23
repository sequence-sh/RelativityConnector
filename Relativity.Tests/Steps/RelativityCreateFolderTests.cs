using System;
using System.Collections.Generic;
using Moq;
using Reductech.EDR.Connectors.Relativity.Steps;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.TestHarness;
using Relativity.Services.Folder;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps
{
    public partial class RelativityCreateFolderTests : StepTestBase<RelativityCreateFolder, int>
    {
        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                Action<Mock<IFolderManager>> setupFolderManager = mock =>
                {
                    mock.Setup(x => x.CreateSingleAsync(13,
                        It.Is<Folder>(folder => folder.Name == "MyNewFolder" && folder.ParentFolder.ArtifactID == 14)
                    )).ReturnsAsync(42);

                    mock.Setup(x => x.Dispose());
                };


                yield return new StepCase(
                        "Create a folder with a parent folder",
                        new RelativityCreateFolder()
                        {
                            FolderName = Constant("MyNewFolder"),
                            ParentFolderId = Constant(14),
                            WorkspaceArtifactId = Constant(13)
                        },
                        42
                    ).WithTestRelativitySettings()
                    .WithService(setupFolderManager);
            }
        }
    }
}