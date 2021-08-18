using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Grpc.Core;
using OneOf;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using ReductechRelativityImport;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

public sealed class RelativityImportEntities : CompoundStep<Unit>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativityImportEntities, Unit>();

    /// <inheritdoc />
    protected override async Task<Result<Unit, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var settings = stateMonad.Settings.TryGetRelativitySettings();

        if (settings.IsFailure)
            return settings.MapError(x => x.WithLocation(this)).ConvertFailure<Unit>();

        var data = await stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(ArtifactType.Case, stateMonad, this),
            Entities,
            Schema.WrapEntityConversion<Schema>(this),
            ControlNumberField.WrapStringStream(),
            FilePathField.WrapStringStream(),
            FolderPathField.WrapStringStream(),
            cancellationToken
        );

        if (data.IsFailure)
            return data.ConvertFailure<Unit>();

        var (workspaceId, entities, schema, controlNumberField, filePathField, folderPathField) =
            data.Value;

        var startResult = stateMonad.ExternalContext.ExternalProcessRunner.StartExternalProcess(
                @"C:\Users\wainw\source\repos\Reductech\relativity\ImportClient\bin\Debug\ImportClient.exe",
                new List<string>(),
                new Dictionary<string, string>(),
                Encoding.Default,
                stateMonad,
                this
            )
            .MapError(x => x.WithLocation(this));


        if (startResult.IsFailure)
            return startResult.ConvertFailure<Unit>();

        using var _ = startResult.Value;

        Channel channel = new("127.0.0.1:30051", ChannelCredentials.Insecure);

        var client = new Reductech_Relativity_Import.Reductech_Relativity_ImportClient(channel);

        var importRequest = new StartImportCommand()
        {
            WorkspaceArtifactId = workspaceId,
            RelativityPassword  = settings.Value.RelativityPassword,
            RelativityUsername  = settings.Value.RelativityUsername,
            RelativityWebAPIUrl = Path.Combine(settings.Value.Url, "relativitywebapi"),
            ControlNumberField  = controlNumberField,
            FilePathField       = filePathField,
            FolderPathField     = folderPathField
        };



        foreach (var property in schema.Properties)
        {
            var dt = GetDataType(property.Value.Type);

            if (dt.IsFailure)
                return dt.ConvertFailure<Unit>().MapError(x => x.WithLocation(this));

            var dataField = new StartImportCommand.Types.DataField()
            {
                Name = property.Key, DataType = dt.Value
            };

            importRequest.DataFields.Add(dataField);
        }

        var startImportResult = await client.StartImportAsync(importRequest, cancellationToken:cancellationToken);

        if (!startImportResult.Success)
            return Result.Failure<Unit, IError>(
                ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(startImportResult.Message)
                    .WithLocation(this)
            );

        using var call = client.ImportData(new CallOptions());

        async ValueTask<Result<Unit, IError>> WriteEntity(Entity e, CancellationToken _1)
        {
            var importObject = Create(e);
            await call.RequestStream.WriteAsync(importObject);
            return Unit.Default;

            ImportObject Create(Entity entity)
            {
                var io = new ImportObject();

                foreach (var schemaProperty in schema.Properties)
                {
                    var v = entity.TryGetValue(schemaProperty.Key)
                        .Unwrap(x => x.GetPrimitiveString(), "");

                    io.Values.Add(v);
                }

                return io;
            }
        }

        var writeResult = await entities.ForEach(WriteEntity, cancellationToken);

        if (writeResult.IsFailure)
            return writeResult;

        await call.RequestStream.CompleteAsync();

        var response = await call.ResponseAsync;

        if (!response.Success)
            return Result.Failure<Unit, IError>(
                ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(response.Message)
                    .WithLocation(this)
            );

        return Unit.Default;
    }

    /// <summary>
    /// The Workspace to import into.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;

    [StepProperty(2)][Required] public IStep<Array<Entity>> Entities { get; set; } = null!;

    [StepProperty(3)][Required] public IStep<Entity> Schema { get; set; } = null!;

    [StepProperty(4)]
    [DefaultValueExplanation("Control Number")]
    public IStep<StringStream> ControlNumberField { get; set; } =
        new StringConstant("Control Number");

    [StepProperty(5)]
    [DefaultValueExplanation("No file path")]
    public IStep<StringStream> FilePathField { get; set; } = new StringConstant("");

    [StepProperty(6)]
    [DefaultValueExplanation("No folder path")]
    public IStep<StringStream> FolderPathField { get; set; } = new StringConstant("");

    static Result<StartImportCommand.Types.DataField.Types.DataType, IErrorBuilder>
        GetDataType(SCLType sclType)
    {
        return sclType switch
        {
            SCLType.String  => StartImportCommand.Types.DataField.Types.DataType.String,
            SCLType.Integer => StartImportCommand.Types.DataField.Types.DataType.Int,
            SCLType.Double  => StartImportCommand.Types.DataField.Types.DataType.Double,
            SCLType.Enum    => StartImportCommand.Types.DataField.Types.DataType.String,
            SCLType.Bool    => StartImportCommand.Types.DataField.Types.DataType.Bool,
            SCLType.Date    => StartImportCommand.Types.DataField.Types.DataType.Date,
            SCLType.Entity => ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(
                "Cannot import a nested entity"
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(sclType), sclType, null)
        };
    }
}

}
