using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using OneOf;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.ExternalProcesses;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

/// <summary>
/// Imports a load file into Relativity using the desktop client
/// </summary>
public sealed class RelativityImport : CompoundStep<Unit>
{
    //SEE: https://help.relativity.com/RelativityOne/Content/System_Guides/Command_line_import.htm

    /// <inheritdoc />
    protected override async Task<Result<Unit, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellation)
    {
        var settingsResult = stateMonad.Settings.TryGetRelativitySettings();

        if (settingsResult.IsFailure)
            return settingsResult.MapError(x => x.WithLocation(this)).ConvertFailure<Unit>();

        var arguments = await TryGetArguments(settingsResult.Value)
            .Bind(
                settingsArguments => TryGetArguments(
                    stateMonad,
                    settingsArguments,
                    this,
                    cancellation
                )
            );

        if (arguments.IsFailure)
            return arguments.ConvertFailure<Unit>();

        stateMonad.Logger.LogInformation("Starting Import");

        var result = await
            stateMonad.ExternalContext.ExternalProcessRunner
                .RunExternalProcess(
                    settingsResult.Value.DesktopClientPath,
                    RDCErrorHandler.Instance,
                    arguments.Value,
                    ImmutableDictionary<string, string>.Empty,
                    Encoding.Default,
                    stateMonad,
                    this,
                    cancellation
                );

        if (result.IsSuccess)
            stateMonad.Logger.LogInformation("Import Successful");

        return result.MapError(x => x.WithLocation(this));
    }

    /// <inheritdoc />
    public override IStepFactory StepFactory => new SimpleStepFactory<RelativityImport, Unit>();

    private static async Task<Result<Maybe<KeyValuePair<string, string>>, IError>>
        TryMakeArgument<T>(
            IRunnableStep<T>? step,
            bool required,
            IStateMonad stateMonad,
            string flag,
            string argumentName,
            Func<T, string> map,
            IStep callingStep,
            CancellationToken cancellation)
    {
        if (step == null)
        {
            if (required)
                return Result.Failure<Maybe<KeyValuePair<string, string>>, IError>(
                    ErrorCode.MissingParameter.ToErrorBuilder(argumentName)
                        .WithLocation(callingStep)
                );

            return Maybe<KeyValuePair<string, string>>.None;
        }

        var r = await step.Run(stateMonad, cancellation)
            .Map(map)
            .Map(x => new KeyValuePair<string, string>(flag, x))
            .Map(Maybe<KeyValuePair<string, string>>.From);

        return r;
    }

    private static Result<IReadOnlyCollection<KeyValuePair<string, string>>, IError>
        TryGetArguments(RelativitySettings relativitySettings)
    {
        var pairs = new List<KeyValuePair<string, string>>
        {
            new("u", relativitySettings.RelativityUsername),
            new("p", relativitySettings.RelativityPassword),
        };

        return pairs;
    }

    private async Task<Result<List<string>, IError>> TryGetArguments(
        IStateMonad stateMonad,
        IEnumerable<KeyValuePair<string, string>> otherArguments,
        IStep callingStep,
        CancellationToken cancellation)
    {
        var argsResults = new[]
        {
            await TryMakeArgument(
                FilePath,
                true,
                stateMonad,
                "f",
                nameof(FilePath),
                x => x.GetString(),
                callingStep,
                cancellation
            ),
            await TryMakeArgument(
                Workspace.WrapWorkspace(stateMonad, TextLocation),
                true,
                stateMonad,
                "c",
                nameof(Workspace),
                x => x.ToString(),
                callingStep,
                cancellation
            ),
            await TryMakeArgument(
                FileImportType,
                true,
                stateMonad,
                "m",
                nameof(FileImportType),
                GetFileImportTypeString,
                callingStep,
                cancellation
            ),
            await TryMakeArgument(
                SettingsFilePath,
                true,
                stateMonad,
                "k",
                nameof(SettingsFilePath),
                x => x.GetString(),
                callingStep,
                cancellation
            ),
            await TryMakeArgument(
                StartLineNumber,
                false,
                stateMonad,
                "s",
                nameof(StartLineNumber),
                x => x.ToString(),
                callingStep,
                cancellation
            ),
            await TryMakeArgument(
                DestinationFolder,
                false,
                stateMonad,
                "d",
                nameof(DestinationFolder),
                x => x.ToString(),
                callingStep,
                cancellation
            ),
            await TryMakeArgument(
                LoadFileEncoding,
                false,
                stateMonad,
                "e",
                nameof(LoadFileEncoding),
                x => x.ToString(),
                callingStep,
                cancellation
            ),
            await TryMakeArgument(
                FullTextFileEncoding,
                false,
                stateMonad,
                "x",
                nameof(FullTextFileEncoding),
                x => x.ToString(),
                callingStep,
                cancellation
            ),
        };

        var r = argsResults.Combine(ErrorList.Combine)
            .Map(x => x.Choose(y => y))
            .Map(x => x.Concat(otherArguments).Select(y => $"-{y.Key}:{y.Value}").ToList());

        return r;
    }

    /// <summary>
    /// The path to the file to import. This should be a .csv, .dat, or .opt file.
    /// </summary>
    [StepProperty]
    [Required]
    [Example("C:/Data/Load.csv")]
    public IStep<StringStream> FilePath { get; set; } = null!; //flag -f

    /// <summary>
    /// The Workspace to import into.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!; //flag -c

    /// <summary>
    /// The file import type
    /// </summary>
    [StepProperty]
    [Required]
    [Example("Native")]
    public IStep<FileImportType> FileImportType { get; set; } = null!; //flag -m

    //TODO oauth

    /// <summary>
    /// The path to the settings file (.kwe or .kwi)
    /// </summary>
    [StepProperty]
    [Required]
    [Example("C:/Settings.kwe")]
    public IStep<StringStream> SettingsFilePath { get; set; } = null!; //flag -k

    /// <summary>
    /// Set the encoding of the load file to a particular code page id.
    /// </summary>
    [StepProperty]
    [Example("1252")]
    [DefaultValueExplanation("Windows' default encoding")]
    public IStep<int>? LoadFileEncoding { get; set; } = null!; //flag -e

    /// <summary>
    /// Sets the encoding of any full text files to a particular code page id.
    /// </summary>
    [StepProperty]
    [Example("1252")]
    [DefaultValueExplanation("Windows' default encoding")]
    public IStep<int>? FullTextFileEncoding { get; set; } = null!; //flag -x

    /// <summary>
    /// Sets the destination folder for the upload.
    /// </summary>
    [StepProperty]
    [Example("1476826")]
    [DefaultValueExplanation("The case root folder.")]
    public IStep<int>? DestinationFolder { get; set; } = null!; //flag -d

    /// <summary>
    /// The starting line number of the load file for the upload.
    /// </summary>
    [StepProperty]
    [DefaultValueExplanation("0")]
    public IStep<int>? StartLineNumber { get; set; } = null!; //flag -s

    //[StepProperty]
    //[DefaultValueExplanation("No Error line is created")]
    //public IStep<string>? ErrorLineFilePath { get; } = null!; //flag -ef

    //TODO Error line file path -ef
    //TODO Error report file path -er

    private static string GetFileImportTypeString(FileImportType fileImportType)
    {
        return fileImportType switch
        {
            Steps.FileImportType.Native => "n",
            Steps.FileImportType.Image => "i",
            Steps.FileImportType.Object => "o",
            _ => throw new ArgumentOutOfRangeException(nameof(fileImportType), fileImportType, null)
        };
    }

    /// <summary>
    /// Error handler for the relativity desktop client
    /// </summary>
    public sealed class RDCErrorHandler : IErrorHandler
    {
        private RDCErrorHandler() { }

        public static IErrorHandler Instance { get; } = new RDCErrorHandler();

        /// <inheritdoc />
        public bool ShouldIgnoreError(string s) => !s.StartsWith("ERROR:");
    }
}

public enum FileImportType
{
    [Display(Name = "native")] Native, //n
    [Display(Name = "image")] Image,   //i
    [Display(Name = "object")] Object  //o
}

}
