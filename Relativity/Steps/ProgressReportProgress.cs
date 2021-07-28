using System;
using Microsoft.Extensions.Logging;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Relativity.Services.DataContracts.DTOs;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

public class ProgressReportProgress : IProgress<ProgressReport>
{
    private readonly IStateMonad _stateMonad;
    private readonly IStep _callingStep;

    public ProgressReportProgress(IStateMonad stateMonad, IStep callingStep)
    {
        _stateMonad  = stateMonad;
        _callingStep = callingStep;
    }

    /// <inheritdoc />
    public void Report(ProgressReport value)
    {
        _stateMonad.Log(
            LogLevel.Information,
            value.Message,
            _callingStep
        ); //TODO make a log situation
    }
}

}
