using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Util;

namespace Reductech.Connectors.Relativity
{
    /// <summary>
    /// Imports a load file into Relativity using the desktop client
    /// </summary>
    public sealed class RelativityImportStep : CompoundStep<Unit>
    {
        //SEE: https://help.relativity.com/RelativityOne/Content/System_Guides/Command_line_import.htm



        /// <inheritdoc />
        public override Result<Unit, IRunErrors> Run(StateMonad stateMonad)
        {
            var settingsResult = stateMonad.GetSettings<IRelativitySettings>(nameof(RelativityImportStep));

            if (settingsResult.IsFailure)
                 return settingsResult.ConvertFailure<Unit>();

            var arguments = TryGetArguments(settingsResult.Value)
                .Bind(settingsArguments => TryGetArguments(stateMonad, settingsArguments));

            if (arguments.IsFailure)
                return arguments.ConvertFailure<Unit>();



            stateMonad.Logger.LogInformation("Starting Import");

            var result =
            stateMonad.ExternalProcessRunner.RunExternalProcess(settingsResult.Value.DesktopClientPath,
                stateMonad.Logger, nameof(RelativityImportStep), IgnoreNoneErrorHandler.Instance, arguments.Value).Result;

            if(result.IsSuccess)
                stateMonad.Logger.LogInformation("Import Successful");

            return result;
        }

        /// <inheritdoc />
        public override IStepFactory StepFactory => RelativityImportStepFactory.Instance;





        private static Result<Maybe<KeyValuePair<string, string>> , IRunErrors> TryMakeArgument<T>(
            IStep<T>? step,
            bool required,
            StateMonad stateMonad, string flag,
            string argumentName,
            Func<T, string> map)
        {
            if (step == null)
            {
                if(required)
                    return Result.Failure<Maybe<KeyValuePair<string, string>> , IRunErrors>(
                    new RunError($"'{argumentName}' must be set.",
                        nameof(RelativityImportStep),
                        null, ErrorCode.MissingParameter));

                return Maybe<KeyValuePair<string, string>>.None;
            }


            var r = step.Run(stateMonad)
                .Map(map).Map(x=> new KeyValuePair<string,string>(flag,x))
                .Map(Maybe<KeyValuePair<string, string>>.From);

            return r;
        }

        private Result<IReadOnlyCollection<KeyValuePair<string, string>>, IRunErrors> TryGetArguments(
            IRelativitySettings relativitySettings)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("u", relativitySettings.RelativityUsername),
                new KeyValuePair<string, string>("p", relativitySettings.RelativityPassword),
            };

            return pairs;

        }

        private Result<List<string>, IRunErrors> TryGetArguments(StateMonad stateMonad, IEnumerable<KeyValuePair<string, string>> otherArguments)
        {
            var argsResults = new[]
            {
                TryMakeArgument(FilePath, true, stateMonad, "f", nameof(FilePath), x=>x),
                TryMakeArgument(WorkspaceId, true, stateMonad, "c", nameof(WorkspaceId), x=>x.ToString()),
                TryMakeArgument(FileImportType, true, stateMonad, "m", nameof(FileImportType), GetFileImportTypeString),
                TryMakeArgument(SettingsFilePath, true, stateMonad, "k", nameof(SettingsFilePath), x=>x),
                TryMakeArgument(StartLineNumber, false, stateMonad, "s", nameof(StartLineNumber), x=>x.ToString())
            };

            var r=  argsResults.Combine(RunErrorList.Combine)
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
        public IStep<string> FilePath { get; set; } = null!; //flag -f

        /// <summary>
        /// The id of the workspace to import into
        /// </summary>
        [StepProperty]
        [Required]
        [Example("1234567")]
        public IStep<int> WorkspaceId { get;set; } = null!; //flag -c

        /// <summary>
        /// The file import type
        /// </summary>
        [StepProperty]
        [Required]
        [Example("Native")]
        public IStep<FileImportType> FileImportType { get;set; } = null!; //flag -m

        //TODO oauth

        /// <summary>
        /// The path to the settings file (.kwe or .kwi)
        /// </summary>
        [StepProperty]
        [Required]
        [Example("C:/Settings.kwe")]
        public IStep<string> SettingsFilePath { get;set; } = null!; //flag -k


        //TODO load file encoding -e
        //TODO full text encoding -x
        //TODO destination folder -d


        /// <summary>
        /// The starting line number of the load file for the upload.
        /// </summary>
        [StepProperty]
        [DefaultValueExplanation("0")]
        public IStep<int>? StartLineNumber { get;set; } = null!; //flag -s

        //[StepProperty]
        //[DefaultValueExplanation("No Error line is created")]
        //public IStep<string>? ErrorLineFilePath { get; } = null!; //flag -ef

        //TODO Error line file path -ef
        //TODO Error report file path -er


        private static string GetFileImportTypeString(FileImportType fileImportType)
        {
            return fileImportType switch
            {
                Relativity.FileImportType.Native => "n",
                Relativity.FileImportType.Image => "i",
                Relativity.FileImportType.Object => "o",
                _ => throw new ArgumentOutOfRangeException(nameof(fileImportType), fileImportType, null)
            };
        }

    }

    public enum FileImportType
    {
        [Display(Name = "native")]
        Native, //n
        [Display(Name = "image")]
        Image, //i
        [Display(Name = "object")]
        Object //o
    }

    /// <summary>
    /// Imports a load file into Relativity using the desktop client
    /// </summary>
    public sealed class RelativityImportStepFactory : SimpleStepFactory<RelativityImportStep, Unit>
    {
        private RelativityImportStepFactory() { }

        public static SimpleStepFactory<RelativityImportStep, Unit> Instance { get; } = new RelativityImportStepFactory();
    }
}
