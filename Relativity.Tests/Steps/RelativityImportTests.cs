using System.Text;
using Moq;
using Reductech.EDR.Core.ExternalProcesses;

namespace Reductech.EDR.Connectors.Relativity.Tests.Steps;

public partial class RelativityImportTests : StepTestBase<RelativityImport, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                        "Basic Import",
                        new RelativityImport
                        {
                            FilePath             = Constant("C:/Data"),
                            Workspace            = new OneOfStep<SCLInt, StringStream>(Constant(1234)),
                            SettingsFilePath     = Constant("C:/Settings"),
                            FileImportType       = Constant(FileImportType.Object),
                            LoadFileEncoding     = Constant(456),
                            FullTextFileEncoding = Constant(567),
                            StartLineNumber      = Constant(5),
                            DestinationFolder    = Constant(789),
                        },
                        Unit.Default,
                        "Starting Import",
                        "Import Successful"
                    ).WithTestRelativitySettings()
                    .WithExternalProcessAction(
                        mock => mock.Setup(
                                x => x.RunExternalProcess(
                                    "C:/DesktopClientPath",
                                    It.IsAny<IErrorHandler>(),
                                    new List<string>
                                    {
                                        "-f:C:/Data",
                                        "-c:1234",
                                        "-m:o",
                                        "-k:C:/Settings",
                                        "-s:5",
                                        "-d:789",
                                        "-e:456",
                                        "-x:567",
                                        "-u:UN",
                                        "-p:PW"
                                    },
                                    It.IsAny<IReadOnlyDictionary<string, string>>(),
                                    Encoding.Default,
                                    It.IsAny<StateMonad>(),
                                    It.IsAny<RelativityImport>(),
                                    It.IsAny<CancellationToken>()
                                )
                            )
                            .ReturnsAsync(Unit.Default)
                    )
                ;

            ;
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            foreach (var errorCase in base.ErrorCases)
            {
                yield return
                    errorCase.WithTestRelativitySettings()
                    ;
            }
        }
    }
}
